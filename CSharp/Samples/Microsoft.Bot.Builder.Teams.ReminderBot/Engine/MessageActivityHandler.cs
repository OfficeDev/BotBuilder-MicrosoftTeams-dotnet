using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Core.Extensions;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Engine
{
    public class MessageActivityHandler : IMessageActivityHandler
    {
        private readonly IRecognizer recognizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActivityHandler"/> class.
        /// </summary>
        /// <param name="recognizer">The recognizer.</param>
        public MessageActivityHandler(IRecognizer recognizer)
        {
            this.recognizer = recognizer;
        }

        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            RecognizerResult recognizerResult = await this.recognizer.Recognize(turnContext.Activity.Text, CancellationToken.None);

            if (recognizerResult.Intents == null || recognizerResult.Intents["RemindMe"] == null)
            {
                await turnContext.SendActivity("Sorry, I did not get that");
            }
            else
            {
                TimeEntity timeEntity = recognizerResult.Entities["Time"].ToObject<TimeEntity>();
                string remindAbout = recognizerResult.Entities["Reminder"].ToString();

                await Task.Delay(TimeSpan.FromSeconds(timeEntity.TimeInSeconds));

                await turnContext.SendActivity("Reminding you about " + remindAbout);
            }
        }
    }
}
