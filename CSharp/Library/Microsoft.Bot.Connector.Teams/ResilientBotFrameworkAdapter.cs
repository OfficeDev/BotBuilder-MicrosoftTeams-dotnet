using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Extensions.Primitives;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;

namespace Microsoft.Bot.Connector.Teams
{
    public class ResilientBotFrameworkAdapter : ActivityAdapterBase
    {
        /// <summary>
        /// The minimum backoff time.
        /// </summary>
        private const double MinimumBackoffTime = 1.44;

        /// <summary>
        /// The maximum backoff time.
        /// </summary>
        private const double MaximumBackoffTime = 1.44;

        /// <summary>
        /// The delta factor for the exponential backoff.
        /// </summary>
        private const double DeltaBackoff = 1.44;

        /// <summary>
        /// The maximum number of retries
        /// </summary>
        private const int MaxNumberOfRetries = 5;

        /// <summary>
        /// The retry policy.
        /// </summary>
        private static RetryPolicy retryPolicy = new RetryPolicy(
            new BotFrameworkErrorDetectionStrategy(),
            MaxNumberOfRetries, 
            TimeSpan.FromSeconds(MinimumBackoffTime), 
            TimeSpan.FromSeconds(MaximumBackoffTime), 
            TimeSpan.FromSeconds(DeltaBackoff));

        private Dictionary<string, Dictionary<string, IConnectorClient>> connectorClientMap = new Dictionary<string, Dictionary<string, IConnectorClient>>();

        public ResilientBotFrameworkAdapter(ICredentialProvider credentialProvider)
        {
            this.CredentialProvider = credentialProvider;
        }

        public ResilientBotFrameworkAdapter(string appId, string appPassword)
        {
            this.CredentialProvider = new StaticCredentialProvider(appId, appPassword);
        }

        protected ICredentialProvider CredentialProvider { get; private set; }

        public async override Task Post(IList<IActivity> activities)
        {
            BotAssert.ActivityListNotNull(activities);

            foreach (Activity activity in activities)
            {
                if (activity.Type == "delay")
                {
                    // The Activity Schema doesn't have a delay type build in, so it's simulated
                    // here in the Bot. This matches the behavior in the Node connector. 
                    int delayMs = (int)activity.Value;
                    await Task.Delay(delayMs).ConfigureAwait(false);
                }
                else
                {
                    var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl), _credentials);
                    await connectorClient.Conversations.SendToConversationAsync(activity).ConfigureAwait(false);
                }
            }
        }

        public async Task Receive(IDictionary<string, StringValues> headers, Activity activity)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            BotAssert.ActivityNotNull(activity);

            if (this.OnReceive != null)
                await this.OnReceive(activity).ConfigureAwait(false);
        }
    }
}
