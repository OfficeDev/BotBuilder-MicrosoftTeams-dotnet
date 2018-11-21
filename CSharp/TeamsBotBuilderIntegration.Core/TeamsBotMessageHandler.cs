// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TeamsBotBuilderIntegration.Core
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// BotMessageHandler for Teams.
    /// </summary>
    public class TeamsBotMessageHandler : TeamsBotMessageHandlerBase
    {
        /// <summary>
        /// Process message.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="adapter">Adapter.</param>
        /// <param name="botCallbackHandler">Callback handler.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Invoke Response.</returns>
        protected override async Task<InvokeResponse> ProcessMessageRequestAsync(HttpRequest request, IAdapterIntegration adapter, BotCallbackHandler botCallbackHandler, CancellationToken cancellationToken)
        {
            var activity = default(Activity);

            using (var bodyReader = new JsonTextReader(new StreamReader(request.Body, Encoding.UTF8)))
            {
                activity = TeamsBotMessageHandlerBase.BotMessageSerializer.Deserialize<Activity>(bodyReader);
            }

#pragma warning disable UseConfigureAwait // Use ConfigureAwait
            var invokeResponse = await adapter.ProcessActivityAsync(
                    request.Headers["Authorization"],
                    activity,
                    botCallbackHandler,
                    cancellationToken);
#pragma warning restore UseConfigureAwait // Use ConfigureAwait

            return invokeResponse;
        }
    }
}