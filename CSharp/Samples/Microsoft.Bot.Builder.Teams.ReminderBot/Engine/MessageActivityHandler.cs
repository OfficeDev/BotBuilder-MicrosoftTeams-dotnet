using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Engine
{
    public class MessageActivityHandler : IMessageActivityHandler
    {
        private readonly IRecognizer recognizer;

        private readonly IProactiveMessageManager proactiveMessageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActivityHandler"/> class.
        /// </summary>
        /// <param name="recognizer">The recognizer.</param>
        public MessageActivityHandler(IRecognizer recognizer, IProactiveMessageManager proactiveMessageManager)
        {
            this.recognizer = recognizer;
            this.proactiveMessageManager = proactiveMessageManager;
        }

        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            RecognizerResult recognizerResult = await this.recognizer.RecognizeAsync(turnContext, CancellationToken.None);

            if (recognizerResult.Intents == null || recognizerResult.Intents["RemindMe"] == null)
            {
                await turnContext.SendActivityAsync("Sorry, I did not get that");
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
