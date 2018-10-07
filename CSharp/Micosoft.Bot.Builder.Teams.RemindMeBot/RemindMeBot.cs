using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams.RemindMeBot.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Micosoft.Bot.Builder.Teams.RemindMeBot
{
    public class RemindMeBot : IBot
    {
        private readonly IRecognizer recognizer;

        private readonly IProactiveMessageManager proactiveMessageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActivityHandler"/> class.
        /// </summary>
        /// <param name="recognizer">The recognizer.</param>
        public RemindMeBot(IRecognizer recognizer, IProactiveMessageManager proactiveMessageManager)
        {
            this.recognizer = recognizer;
            this.proactiveMessageManager = proactiveMessageManager;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
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
