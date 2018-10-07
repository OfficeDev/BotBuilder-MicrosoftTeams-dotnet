// <copyright file="IMessageActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Processes activites with <see cref="Schema.Activity.Type"/> set to <see cref="Schema.ActivityTypes.Message"/>.
    /// </summary>
    public interface IMessageActivityHandler
    {
        /// <summary>
        /// Handles the message activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleMessageAsync(ITurnContext turnContext);
    }
}
