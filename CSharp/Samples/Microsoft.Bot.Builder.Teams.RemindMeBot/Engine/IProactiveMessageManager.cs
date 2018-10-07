// <copyright file="IProactiveMessageManager.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.RemindMeBot.Engine
{
    using System;

    /// <summary>
    /// Proactive message manager.
    /// </summary>
    public interface IProactiveMessageManager
    {
        /// <summary>
        /// Queues the work item to be executed at a later point.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="messageToSend">The message to send.</param>
        /// <param name="timeToWait">The time to wait.</param>
        void QueueWorkItem(ITurnContext turnContext, string messageToSend, TimeSpan timeToWait);
    }
}