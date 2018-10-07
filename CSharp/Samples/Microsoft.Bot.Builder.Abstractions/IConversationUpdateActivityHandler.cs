// <copyright file="IConversationUpdateActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Processes activites with <see cref="Schema.Activity.Type"/> set to <see cref="Schema.ActivityTypes.ConversationUpdate"/>.
    /// </summary>
    public interface IConversationUpdateActivityHandler
    {
        /// <summary>
        /// Handles the conversation update activity task asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleConversationUpdateActivityTaskAsync(ITurnContext turnContext);
    }
}
