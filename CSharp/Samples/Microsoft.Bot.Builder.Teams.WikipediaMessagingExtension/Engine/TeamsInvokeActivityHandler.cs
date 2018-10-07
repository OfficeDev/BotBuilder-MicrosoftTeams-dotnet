// <copyright file="TeamsInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Handles Teams invoke activity.
    /// </summary>
    /// <seealso cref="ITeamsInvokeActivityHandler" />
    public class TeamsInvokeActivityHandler : ITeamsInvokeActivityHandler
    {
        /// <summary>
        /// The search handler
        /// </summary>
        private readonly ISearchHandler searchHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsInvokeActivityHandler"/> class.
        /// </summary>
        /// <param name="searchHandler">The search handler.</param>
        public TeamsInvokeActivityHandler(ISearchHandler searchHandler)
        {
            this.searchHandler = searchHandler;
        }

        /// <summary>
        /// Handles the messaging extension action asynchronously.
        /// </summary>
        /// <param name="messagingExtensionAction">The messaging extension action.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task<InvokeResponse> HandleMessagingExtensionActionAsync(MessagingExtensionActivityAction messagingExtensionAction)
        {
            try
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResponse
                    {
                        ComposeExtension = await this.searchHandler.GetSearchResultAsync(messagingExtensionAction).ConfigureAwait(false),
                    },
                    Status = 200,
                };
            }
            catch (Exception ex)
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResult
                    {
                        Text = "Failed to search " + ex.Message,
                        Type = "message",
                    },
                    Status = 200,
                };
            }
        }

        /// <summary>
        /// Handles the invoke task asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>
        /// Invoke response.
        /// </returns>
        public Task<InvokeResponse> HandleInvokeTaskAsync(ITurnContext turnContext)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        /// <summary>
        /// Handles the o365 connector card action asynchronously.
        /// </summary>
        /// <param name="o365ConnectorCardActionAction">The o365 connector card action action.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public Task<InvokeResponse> HandleO365ConnectorCardActionAsync(O365ConnectorCardActivityAction o365ConnectorCardActionAction)
        {
            return Task.FromResult<InvokeResponse>(null);
        }

        /// <summary>
        /// Handles the signin state verification action asynchronously.
        /// </summary>
        /// <param name="signinStateVerificationAction">The signin state verification action.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public Task<InvokeResponse> HandleSigninStateVerificationActionAsync(SigninStateVerificationActivityAction signinStateVerificationAction)
        {
            return Task.FromResult<InvokeResponse>(null);
        }
    }
}
