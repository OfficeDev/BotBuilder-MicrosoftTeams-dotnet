using System;

namespace Microsoft.Bot.Builder.Teams.RemindMeBot.Engine
{
    public interface IProactiveMessageManager
    {
        void QueueWorkItem(ITurnContext turnContext, string messageToSend, TimeSpan timeToWait);
    }
}