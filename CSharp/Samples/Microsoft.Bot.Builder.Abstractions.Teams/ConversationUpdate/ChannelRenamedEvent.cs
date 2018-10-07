namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Channel renamed event.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class ChannelRenamedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.ChannelRenamed;
            }
        }

        /// <summary>
        /// Gets the details of the channel renamed.
        /// </summary>
        public ChannelInfo Channel { get; internal set; }
    }
}
