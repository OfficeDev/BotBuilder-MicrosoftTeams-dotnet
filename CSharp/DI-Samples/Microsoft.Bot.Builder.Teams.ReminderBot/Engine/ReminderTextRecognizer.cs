using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Engine
{
    public class ReminderTextRecognizer : IRecognizer
    {
        public Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(this.RecognizeInternal(turnContext.Activity.Text));
        }

        public Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken ct)
            where T : IRecognizerConvert, new()
        {
            ct.ThrowIfCancellationRequested();
            T result = new T();
            RecognizerResult recognizerResult = this.RecognizeInternal(turnContext.Activity.Text);
            result.Convert(recognizerResult);
            return Task.FromResult(result);
        }

        private RecognizerResult RecognizeInternal(string utterance)
        {
            TimeEntity timeEntity = null;

            // Solve Remind me in X hours
            if (utterance.IndexOf("Remind me in ", 0, StringComparison.OrdinalIgnoreCase) != -1)
            {
                string timeString = utterance.Replace("Remind me in ", string.Empty);
                string[] splits = timeString.Split(' ');

                if (splits.Length < 2)
                {
                    return new RecognizerResult();
                }

                if (!int.TryParse(splits[0], out int timeValue))
                {
                    return new RecognizerResult();
                }

                string timeType = splits[1];

                if (timeType.Equals("hours", StringComparison.OrdinalIgnoreCase) ||
                    timeType.Equals("hour", StringComparison.OrdinalIgnoreCase))
                {
                    timeEntity = new TimeEntity
                    {
                        TimeInSeconds = timeValue * 60 * 60
                    };
                }
                else if (timeType.Equals("minutes", StringComparison.OrdinalIgnoreCase) ||
                    timeType.Equals("minute", StringComparison.OrdinalIgnoreCase))
                {
                    timeEntity = new TimeEntity
                    {
                        TimeInSeconds = timeValue * 60
                    };
                }
                else if (timeType.Equals("seconds", StringComparison.OrdinalIgnoreCase) ||
                    timeType.Equals("second", StringComparison.OrdinalIgnoreCase))
                {
                    timeEntity = new TimeEntity
                    {
                        TimeInSeconds = timeValue
                    };
                }
                else if (timeType.Equals("days", StringComparison.OrdinalIgnoreCase) ||
                    timeType.Equals("day", StringComparison.OrdinalIgnoreCase))
                {
                    timeEntity = new TimeEntity
                    {
                        TimeInSeconds = timeValue * 24 * 60 * 60
                    };
                }
                else
                {
                    return new RecognizerResult();
                }

                string remindAbout = string.Join("", splits.Skip(2));

                return new RecognizerResult
                {
                    Intents = new Dictionary<string, IntentScore>
                    {
                        { "RemindMe", new IntentScore { Score = 0.9 } }
                    },
                    Entities = new JObject
                    {
                        { "Time", JObject.FromObject(timeEntity) },
                        { "Reminder", remindAbout }
                    },
                    Text = utterance
                };
            }

            return new RecognizerResult();
        }
    }
}
