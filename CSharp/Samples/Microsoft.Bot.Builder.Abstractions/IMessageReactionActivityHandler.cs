// <copyright file="IMessageReactionActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Processes activites with <see cref="Schema.Activity.Type"/> set to <see cref="Schema.ActivityTypes.MessageReaction"/>.
    /// </summary>
    public interface IMessageReactionActivityHandler
    {
        /// <summary>
        /// Handles the message reaction activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleMessageReactionAsync(ITurnContext turnContext);
    }
}
