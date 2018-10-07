// <copyright file="RemindMeBot.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.RemindMeBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams.RemindMeBot.Engine;

    /// <summary>
    /// Remind me bot.
    /// </summary>
    /// <seealso cref="IBot" />
    public class RemindMeBot : IBot
    {
        private readonly IRecognizer recognizer;

        private readonly IProactiveMessageManager proactiveMessageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemindMeBot"/> class.
        /// </summary>
        /// <param name="recognizer">The recognizer.</param>
        /// <param name="proactiveMessageManager">Proactive message manager.</param>
        public RemindMeBot(IRecognizer recognizer, IProactiveMessageManager proactiveMessageManager)
        {
            this.recognizer = recognizer;
            this.proactiveMessageManager = proactiveMessageManager;
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
        /// <seealso cref="T:Microsoft.Bot.Builder.ITurnContext" />
        /// <seealso cref="T:Microsoft.Bot.Schema.IActivity" />
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            RecognizerResult recognizerResult = await this.recognizer.RecognizeAsync(turnContext, CancellationToken.None).ConfigureAwait(false);

            if (recognizerResult.Intents == null || recognizerResult.Intents["RemindMe"] == null)
            {
                await turnContext.SendActivityAsync("Sorry, I did not get that").ConfigureAwait(false);
            }
            else
            {
                TimeEntity timeEntity = recognizerResult.Entities["Time"].ToObject<TimeEntity>();
                string remindAbout = recognizerResult.Entities["Reminder"].ToString();

                this.proactiveMessageManager.QueueWorkItem(turnContext, "Reminding you about " + remindAbout, TimeSpan.FromSeconds(timeEntity.TimeInSeconds));
            }
        }
    }
}
