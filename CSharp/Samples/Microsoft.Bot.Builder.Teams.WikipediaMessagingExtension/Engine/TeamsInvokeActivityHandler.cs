// <copyright file="TeamsInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Handles Teams invoke activity.
    /// </summary>
    /// <seealso cref="ITeamsInvokeActivityHandler" />
    public class TeamsInvokeActivityHandler : TeamsInvokeActivityHandlerBase
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
        /// <param name="turnContext">The turn context</param>
        /// <param name="query">The invoke query object</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task<InvokeResponse> HandleMessagingExtensionQueryAsync(TurnContext turnContext, MessagingExtensionQuery query)
        {
            try
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResponse
                    {
                        ComposeExtension = await this.searchHandler.GetSearchResultAsync(query).ConfigureAwait(false),
                    },
                    Status = 200,
                };
            }
            catch (Exception ex)
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResponse
                    {
                        ComposeExtension = new MessagingExtensionResult
                        {
                            Text = "Failed to search " + ex.Message,
                            Type = "message",
                        },
                    },
                    Status = 200,
                };
            }
        }
    }
}
