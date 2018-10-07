// <copyright file="IActivityProcessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Incoming Bot Framework activity processor.
    /// </summary>
    public interface IActivityProcessor
    {
        /// <summary>
        /// Processes the incoming activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task tracking operation.</returns>
        Task ProcessIncomingActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}