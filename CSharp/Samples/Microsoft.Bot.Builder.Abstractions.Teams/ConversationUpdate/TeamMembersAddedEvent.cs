namespace Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate
{
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Event arguments for members added event.
    /// </summary>
    /// <seealso cref="TeamEventBase" />
    public class TeamMembersAddedEvent : TeamEventBase
    {
        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override TeamEventType EventType
        {
            get
            {
                return TeamEventType.MembersAdded;
            }
        }

        /// <summary>
        /// Gets the members added.
        /// </summary>
        public IList<ChannelAccount> MembersAdded { get; internal set; }
    }
}
