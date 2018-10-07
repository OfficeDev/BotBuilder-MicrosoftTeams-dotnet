// <copyright file="ProactiveMessageManager.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.RemindMeBot.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Proactive message manager.
    /// </summary>
    /// <seealso cref="IProactiveMessageManager" />
    public class ProactiveMessageManager : IProactiveMessageManager
    {
        private readonly BotFrameworkAdapter botFrameworkAdapter;

        private readonly ICredentialProvider credentialProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProactiveMessageManager"/> class.
        /// </summary>
        /// <param name="botFrameworkAdapter">The bot framework adapter.</param>
        /// <param name="credentialProvider">The credential provider.</param>
        public ProactiveMessageManager(BotFrameworkAdapter botFrameworkAdapter, ICredentialProvider credentialProvider)
        {
            this.botFrameworkAdapter = botFrameworkAdapter;
            this.credentialProvider = credentialProvider;
        }

        /// <summary>
        /// Queues the work item to be executed at a later point.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="messageToSend">The message to send.</param>
        /// <param name="timeToWait">The time to wait.</param>
        public void QueueWorkItem(ITurnContext turnContext, string messageToSend, TimeSpan timeToWait)
        {
            Task.Run(async () =>
            {
                await Task.Delay(timeToWait).ConfigureAwait(false);

                await this.botFrameworkAdapter.CreateConversationAsync(
                    turnContext.Activity.ChannelId,
                    turnContext.Activity.ServiceUrl,
                    await this.GetMicrosoftAppCredentialsAsync(turnContext).ConfigureAwait(false),
                    new ConversationParameters
                    {
                        Bot = turnContext.Activity.Recipient,
                        Members = new List<ChannelAccount> { turnContext.Activity.From },
                        ChannelData = JObject.FromObject(
                            new TeamsChannelData
                            {
                                Tenant = new TenantInfo
                                {
                                    Id = turnContext.Activity.GetChannelData<TeamsChannelData>()?.Tenant?.Id,
                                },
                            },
                            JsonSerializer.Create(new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                            })),
                    },
                    async (context, cancellationToken) =>
                    {
                        Activity activityToSend = new Activity
                        {
                            Conversation = context.Activity.Conversation,
                            From = context.Activity.Recipient,
                            Text = messageToSend,
                            ChannelId = context.Activity.ChannelId,
                            ServiceUrl = context.Activity.ServiceUrl,
                            Type = ActivityTypes.Message,
                        };

                        await context.SendActivityAsync(activityToSend, cancellationToken).ConfigureAwait(false);
                    },
                    CancellationToken.None).ConfigureAwait(false);
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Gets the microsoft application credentials asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Application credentials.</returns>
        private async Task<MicrosoftAppCredentials> GetMicrosoftAppCredentialsAsync(ITurnContext turnContext)
        {
            ClaimsIdentity claimsIdentity = turnContext.TurnState.Get<ClaimsIdentity>("BotIdentity");

            // For requests from channel App Id is in Audience claim of JWT token. For emulator it is in AppId claim. For
            // unauthenticated requests we have anonymouse identity provided auth is disabled.
            Claim botAppIdClaim = claimsIdentity.Claims?.SingleOrDefault(claim => claim.Type == AuthenticationConstants.AudienceClaim)
                ??
                claimsIdentity.Claims?.SingleOrDefault(claim => claim.Type == AuthenticationConstants.AppIdClaim);

            string appPassword = await this.credentialProvider.GetAppPasswordAsync(botAppIdClaim.Value).ConfigureAwait(false);
            return new MicrosoftAppCredentials(botAppIdClaim.Value, appPassword);
        }
    }
}
