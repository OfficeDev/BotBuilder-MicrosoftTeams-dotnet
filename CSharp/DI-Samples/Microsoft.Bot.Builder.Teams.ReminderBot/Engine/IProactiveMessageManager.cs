using System;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Engine
{
    public interface IProactiveMessageManager
    {
        void QueueWorkItem(ITurnContext turnContext, string messageToSend, TimeSpan timeToWait);
    }
}