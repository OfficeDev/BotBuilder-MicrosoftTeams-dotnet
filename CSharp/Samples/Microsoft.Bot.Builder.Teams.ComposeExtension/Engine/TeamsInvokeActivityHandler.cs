using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.ComposeExtension.Engine
{
    public class TeamsInvokeActivityHandler : ITeamsInvokeActivityHandler
    {
        private readonly ISearchHandler searchHandler;

        public TeamsInvokeActivityHandler(ISearchHandler searchHandler)
        {
            this.searchHandler = searchHandler;
        }

        public async Task<InvokeResponse> HandleComposeExtensionActionAsync(ComposeExtensionActivityAction composeExtensionAction)
        {
            try
            {
                return new InvokeResponse
                {
                    Body = new ComposeExtensionResponse
                    {
                        ComposeExtension = await this.searchHandler.GetSearchResultAsync(composeExtensionAction)
                    },
                    Status = 200,
                };
            }
            catch (Exception ex)
            {
                return new InvokeResponse
                {
                    Body = new ComposeExtensionResult
                    {
                        Text = "Failed to search " + ex.Message,
                        Type = "message"
                    },
                    Status = 200
                };
            }
        }

        public Task<InvokeResponse> HandleInvokeTask(ITurnContext turnContext)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        public Task<InvokeResponse> HandleO365ConnectorCardActionAsync(O365ConnectorCardActivityAction o365ConnectorCardActionAction)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        public Task<InvokeResponse> HandleSigninStateVerificationActionAsync(SigninStateVerificationActivityAction signinStateVerificationAction)
        {
            return Task.FromResult<InvokeResponse>(null);
        }
    }
}
