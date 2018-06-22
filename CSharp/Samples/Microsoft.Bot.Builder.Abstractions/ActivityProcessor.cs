using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Abstractions
{
    public partial class ActivityProcessor : IActivityProcessor
    {
        private readonly IMessageActivityHandler messageActivityHandler;

        private readonly IConversationUpdateActivityHandler conversationUpdateActivityHandler;

        private readonly IInvokeActivityHandler invokeActivityHandler;

        private readonly IMessageReactionActivityHandler messageReactionActivityHandler;

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

        public virtual async Task ProcessIncomingActivityAsync(ITurnContext turnContext)
        {
            switch (turnContext.Activity.Type)
            {
                case (ActivityTypes.Message):
                    {
                        if (this.messageActivityHandler != null)
                        {
                            await this.messageActivityHandler.HandleMessageAsync(turnContext);
                        }

                        return;
                    }
                case (ActivityTypes.ConversationUpdate):
                    {
                        if (this.conversationUpdateActivityHandler != null)
                        {
                            await this.conversationUpdateActivityHandler.HandleConversationUpdateActivityTask(turnContext);
                        }

                        return;
                    }
                case (ActivityTypes.Invoke):
                    {
                        if (this.invokeActivityHandler != null)
                        {
                            InvokeResponse invokeResponse = await this.invokeActivityHandler.HandleInvokeTask(turnContext);
                            await turnContext.SendActivity(new Activity { Value = invokeResponse });
                        }

                        return;
                    }
                case (ActivityTypes.MessageReaction):
                    {
                        if (this.messageReactionActivityHandler != null)
                        {
                            await this.messageReactionActivityHandler.HandleMessageReactionAsync(turnContext);
                        }

                        return;
                    }
            }
        }
    }
}
