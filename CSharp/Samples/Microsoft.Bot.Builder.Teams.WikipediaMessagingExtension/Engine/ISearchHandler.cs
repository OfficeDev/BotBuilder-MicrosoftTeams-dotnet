// <copyright file="ISearchHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Search handler.
    /// </summary>
    public interface ISearchHandler
    {
        /// <summary>
        /// Gets the search result asynchronously.
        /// </summary>
        /// <param name="messagingExtensionActivityAction">The messaging extension activity action.</param>
        /// <returns>Messaging extension result.</returns>
        Task<MessagingExtensionResult> GetSearchResultAsync(MessagingExtensionActivityAction messagingExtensionActivityAction);
    }
}