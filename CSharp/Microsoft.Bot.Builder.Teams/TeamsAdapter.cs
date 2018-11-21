namespace Microsoft.Bot.Builder.Teams
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Connector.Teams;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Logging;
    using Microsoft.Rest.TransientFaultHandling;

    /// <summary>
    /// A Teams bot adapter that can connect a bot to a service endpoint.
    /// </summary>
    /// <remarks>The Teams bot adapter encapsulates authentication processes and sends
    /// activities to and receives activities from the Bot Connector Service. When your
    /// bot receives an activity, the adapter creates a context object, passes it to your
    /// bot's application logic, and sends responses back to the user's channel.
    /// <para>Use <see cref="Use(IMiddleware)"/> to add <see cref="IMiddleware"/> objects
    /// to your adapter’s middleware collection. The adapter processes and directs
    /// incoming activities in through the bot middleware pipeline to your bot’s logic
    /// and then back out again. As each activity flows in and out of the bot, each piece
    /// of middleware can inspect or act upon the activity, both before and after the bot
    /// logic runs.</para>
    /// </remarks>
    /// <seealso cref="BotFrameworkAdapter"/>
    /// <seealso cref="ITurnContext"/>
    /// <seealso cref="IActivity"/>
    /// <seealso cref="IBot"/>
    /// <seealso cref="IMiddleware"/>
    public class TeamsAdapter : BotFrameworkAdapter
    {
        /// <summary>
        /// The application credential map. This is used to ensure we don't try to get tokens for Bot everytime.
        /// </summary>
        private readonly ConcurrentDictionary<string, MicrosoftAppCredentials> appCredentialMap =
            new ConcurrentDictionary<string, MicrosoftAppCredentials>();

        /// <summary>
        /// The credential provider.
        /// </summary>
        private readonly ICredentialProvider credentialProvider;

        /// <summary>
        /// The connector client retry policy.
        /// </summary>
        private readonly RetryPolicy connectorClientRetryPolicy;

        /// <summary>
        /// The delegating handler used to process requests.
        /// </summary>
        private readonly DelegatingHandler delegatingHandler;

        /// <summary>
        /// The ILogger implementation this adapter should use.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsAdapter"/> class,
        /// using a credential provider.
        /// </summary>
        /// <param name="credentialProvider">The credential provider.</param>
        /// <param name="channelProvider">The channel provider.</param>
        /// <param name="connectorClientRetryPolicy">Retry policy for retrying HTTP operations.</param>
        /// <param name="customHttpClient">The HTTP client.</param>
        /// <param name="middleware">The middleware to initially add to the adapter.</param>
        /// <param name="logger">The ILogger implementation this adapter should use.</param>
        /// <param name="delegatingHandler">The delegating handler.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="credentialProvider"/> is <c>null</c>.</exception>
        /// <remarks>Use a <see cref="MiddlewareSet"/> object to add multiple middleware
        /// components in the conustructor. Use the <see cref="Use(IMiddleware)"/> method to
        /// add additional middleware to the adapter after construction.
        /// </remarks>
        public TeamsAdapter(ICredentialProvider credentialProvider, IChannelProvider channelProvider = null, RetryPolicy connectorClientRetryPolicy = null, HttpClient customHttpClient = null, IMiddleware middleware = null, ILogger logger = null, DelegatingHandler delegatingHandler = null)
            : base(credentialProvider, channelProvider, connectorClientRetryPolicy, customHttpClient, middleware, logger)
        {
            this.credentialProvider = credentialProvider;
            this.connectorClientRetryPolicy = connectorClientRetryPolicy;
            this.delegatingHandler = delegatingHandler;
            this.logger = logger;
        }

        /// <summary>
        /// Fetch list of channels in a team.
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <param name="teamId">Id of the team.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of channels in the team. See <see cref="ConversationList"/>.</returns>
        public async Task<ConversationList> FetchChannelListAsync(ITurnContext turnContext, string teamId, CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(turnContext);

            using (var connectorClient = await this.CreateTeamsConnectorClientAsync(turnContext).ConfigureAwait(false))
            {
                return await connectorClient.Teams.FetchChannelListAsync(teamId, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Fetch details about specified team.
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <param name="teamId">Id of the team.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the team. See <see cref="TeamDetails"/>.</returns>
        public async Task<TeamDetails> FetchTeamDetailsAsync(ITurnContext turnContext, string teamId, CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(turnContext);

            using (var connectorClient = await this.CreateTeamsConnectorClientAsync(turnContext).ConfigureAwait(false))
            {
                return await connectorClient.Teams.FetchTeamDetailsAsync(teamId, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<ITeamsConnectorClient> CreateTeamsConnectorClientAsync(ITurnContext turnContext)
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity.ChannelId.Equals("msteams", StringComparison.OrdinalIgnoreCase))
            {
                // BotFrameworkAdapter when processing activity, post Auth adds BotIdentity into the context.
                ClaimsIdentity claimsIdentity = turnContext.TurnState.Get<ClaimsIdentity>("BotIdentity");

                // If we failed to find ClaimsIdentity, create a new AnonymousIdentity. This tells us that Auth is off.
                if (claimsIdentity == null)
                {
                    claimsIdentity = new ClaimsIdentity(new List<Claim>(), "anonymous");
                }

                return await this.CreateTeamsConnectorClientAsync(turnContext.Activity.ServiceUrl, claimsIdentity).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"{nameof(turnContext)} does not have the activity from msteams channel");
            }
        }

        /// <summary>
        /// Creates the teams connector client asynchronously.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="claimsIdentity">The claims identity.</param>
        /// <returns>ConnectorClient instance.</returns>
        /// <exception cref="NotSupportedException">ClaimsIdemtity cannot be null. Pass Anonymous ClaimsIdentity if authentication is turned off.</exception>
        private async Task<ITeamsConnectorClient> CreateTeamsConnectorClientAsync(string serviceUrl, ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null)
            {
                throw new NotSupportedException("ClaimsIdemtity cannot be null. Pass Anonymous ClaimsIdentity if authentication is turned off.");
            }

            // For requests from channel App Id is in Audience claim of JWT token. For emulator it is in AppId claim. For
            // unauthenticated requests we have anonymouse identity provided auth is disabled.
            Claim botAppIdClaim = claimsIdentity.Claims?.SingleOrDefault(claim => claim.Type == AuthenticationConstants.AudienceClaim)
                ??
                claimsIdentity.Claims?.SingleOrDefault(claim => claim.Type == AuthenticationConstants.AppIdClaim);

            // For anonymous requests (requests with no header) appId is not set in claims.
            if (botAppIdClaim != null)
            {
                string botId = botAppIdClaim.Value;
                MicrosoftAppCredentials appCredentials = await this.GetAppCredentialsAsync(botId).ConfigureAwait(false);
                return this.CreateTeamsConnectorClient(serviceUrl, appCredentials);
            }
            else
            {
                return this.CreateTeamsConnectorClient(serviceUrl);
            }
        }

        /// <summary>
        /// Gets the application credentials. App Credentials are cached so as to ensure we are not refreshing
        /// token everytime.
        /// </summary>
        /// <param name="appId">The application identifier (AAD Id for the bot).</param>
        /// <returns>App credentials.</returns>
        private async Task<MicrosoftAppCredentials> GetAppCredentialsAsync(string appId)
        {
            if (appId == null)
            {
                return MicrosoftAppCredentials.Empty;
            }

            if (!this.appCredentialMap.TryGetValue(appId, out MicrosoftAppCredentials appCredentials))
            {
                string appPassword = await this.credentialProvider.GetAppPasswordAsync(appId).ConfigureAwait(false);
                appCredentials = new MicrosoftAppCredentials(appId, appPassword);
                this.appCredentialMap[appId] = appCredentials;
            }

            return appCredentials;
        }

        /// <summary>
        /// Creates the teams connector client.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="appCredentials">The application credentials for the bot.</param>
        /// <returns>Connector client instance.</returns>
        private ITeamsConnectorClient CreateTeamsConnectorClient(string serviceUrl, MicrosoftAppCredentials appCredentials = null)
        {
            TeamsConnectorClient connectorClient;

            if (appCredentials == null)
            {
                appCredentials = new MicrosoftAppCredentials(appId: null, password: null);
            }

            if (this.delegatingHandler == null)
            {
                connectorClient = new TeamsConnectorClient(new Uri(serviceUrl), appCredentials);
            }
            else
            {
                connectorClient = new TeamsConnectorClient(new Uri(serviceUrl), appCredentials, this.delegatingHandler);
            }

            if (this.connectorClientRetryPolicy != null)
            {
                connectorClient.SetRetryPolicy(this.connectorClientRetryPolicy);
            }

            return connectorClient;
        }
    }
}
