// <copyright file="WikipediaMessagingExtensionBot.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions;

    /// <summary>
    /// Wikipedia messaging extension.
    /// </summary>
    /// <seealso cref="IBot" />
    public class WikipediaMessagingExtensionBot : IBot
    {
        /// <summary>
        /// The activity processor.
        /// </summary>
        private readonly IActivityProcessor activityProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikipediaMessagingExtensionBot"/> class.
        /// </summary>
        /// <param name="activityProcessor">The activity processor.</param>
        public WikipediaMessagingExtensionBot(IActivityProcessor activityProcessor)
        {
            this.activityProcessor = activityProcessor;
        }

        /// <summary>
        /// When implemented in a bot, handles an incoming activity.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the work queued to execute.
        /// </returns>
        /// <remarks>
        /// The <paramref name="turnContext" /> provides information about the
        /// incoming activity, and other data needed to process the activity.
        /// </remarks>
        /// <seealso cref="ITurnContext" />
        /// <seealso cref="Schema.IActivity" />
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.activityProcessor.ProcessIncomingActivityAsync(turnContext).ConfigureAwait(false);
        }
    }
}
