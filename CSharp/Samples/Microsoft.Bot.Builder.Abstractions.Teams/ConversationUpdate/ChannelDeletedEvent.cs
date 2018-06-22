namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Channel deleted event arguments.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class ChannelDeletedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.ChannelDeleted;
            }
        }

        /// <summary>
        /// Gets the channel deleted.
        /// </summary>
        public ChannelInfo Channel { get; internal set; }
    }
}
