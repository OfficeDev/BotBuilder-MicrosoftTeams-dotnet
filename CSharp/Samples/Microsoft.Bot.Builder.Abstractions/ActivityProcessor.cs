// <copyright file="ActivityProcessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Activity processor to process incoming Bot Framework activities.
    /// </summary>
    /// <seealso cref="Microsoft.Bot.Builder.Abstractions.IActivityProcessor" />
    public partial class ActivityProcessor : IActivityProcessor
    {
        /// <summary>
        /// Message (text) activity handler.
        /// </summary>
        private readonly IMessageActivityHandler messageActivityHandler;

        /// <summary>
        /// Conversation update activity handler.
        /// </summary>
        private readonly IConversationUpdateActivityHandler conversationUpdateActivityHandler;

        /// <summary>
        /// Invoke handler.
        /// </summary>
        private readonly IInvokeActivityHandler invokeActivityHandler;

        /// <summary>
        /// Message reaction activity handler.
        /// </summary>
        private readonly IMessageReactionActivityHandler messageReactionActivityHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProcessor"/> class.
        /// </summary>
        /// <param name="messageActivityHandler">The message activity handler.</param>
        /// <param name="conversationUpdateActivityHandler">The conversation update activity handler.</param>
        /// <param name="invokeActivityHandler">The invoke activity handler.</param>
        /// <param name="messageReactionActivityHandler">The message reaction activity handler.</param>
        public ActivityProcessor(
            IMessageActivityHandler messageActivityHandler = null,
            IConversationUpdateActivityHandler conversationUpdateActivityHandler = null,
            IInvokeActivityHandler invokeActivityHandler = null,
            IMessageReactionActivityHandler messageReactionActivityHandler = null)
        {
            this.messageActivityHandler = messageActivityHandler;
            this.conversationUpdateActivityHandler = conversationUpdateActivityHandler;
            this.invokeActivityHandler = invokeActivityHandler;
            this.messageReactionActivityHandler = messageReactionActivityHandler;
        }

        /// <summary>
        /// Processes the incoming activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task tracking the operation.</returns>
        public virtual async Task ProcessIncomingActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        if (this.messageActivityHandler != null)
                        {
                            await this.messageActivityHandler.HandleMessageAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.ConversationUpdate:
                    {
                        if (this.conversationUpdateActivityHandler != null)
                        {
                            await this.conversationUpdateActivityHandler.HandleConversationUpdateActivityTaskAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.Invoke:
                    {
                        if (this.invokeActivityHandler != null)
                        {
                            InvokeResponse invokeResponse = await this.invokeActivityHandler.HandleInvokeTaskAsync(turnContext).ConfigureAwait(false);
                            await turnContext.SendActivityAsync(new Activity { Value = invokeResponse }).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.MessageReaction:
                    {
                        if (this.messageReactionActivityHandler != null)
                        {
                            await this.messageReactionActivityHandler.HandleMessageReactionAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }
            }
        }
    }
}
