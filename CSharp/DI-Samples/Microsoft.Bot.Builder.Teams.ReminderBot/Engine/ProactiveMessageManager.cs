using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Engine
{
    public class ProactiveMessageManager : IProactiveMessageManager
    {
        private readonly BotFrameworkAdapter botFrameworkAdapter;

        private readonly ICredentialProvider credentialProvider;

        public ProactiveMessageManager(BotFrameworkAdapter botFrameworkAdapter, ICredentialProvider credentialProvider)
        {
            this.botFrameworkAdapter = botFrameworkAdapter;
            this.credentialProvider = credentialProvider;
        }

        public void QueueWorkItem(ITurnContext turnContext, string messageToSend, TimeSpan timeToWait)
        {
            Task.Run(async () =>
            {
                await Task.Delay(timeToWait);

                await this.botFrameworkAdapter.CreateConversationAsync(
                    turnContext.Activity.ChannelId, turnContext.Activity.ServiceUrl,
                    await this.GetMicrosoftAppCredentials(turnContext),
                    new ConversationParameters
                    {
                        Bot = turnContext.Activity.Recipient,
                        Members = new List<ChannelAccount> { turnContext.Activity.From },
                        ChannelData = JObject.FromObject(new TeamsChannelData
                        {
                            Tenant = new TenantInfo
                            {
                                Id = turnContext.Activity.GetChannelData<TeamsChannelData>()?.Tenant?.Id
                            }
                        },
                        JsonSerializer.Create(new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }))
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

                        await context.SendActivityAsync(activityToSend, cancellationToken);
                    },
                    CancellationToken.None);
                return Task.CompletedTask;
            });
        }

        private async Task<MicrosoftAppCredentials> GetMicrosoftAppCredentials(ITurnContext turnContext)
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
