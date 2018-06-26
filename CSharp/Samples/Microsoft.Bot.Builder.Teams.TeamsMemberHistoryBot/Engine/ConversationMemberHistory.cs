using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Teams.TeamsMemberHistoryBot.Engine
{
    /// <summary>
    /// Conversation member history. Every change in membership is recorded here.
    /// </summary>
    public class ConversationMemberHistory
    {
        /// <summary>
        /// Gets or sets the member operations. Operation is a tuple of ObjectId, Operation and Time.
        /// </summary>
        public List<MemberOperationDetails> MemberOperations { get; set; } = new List<MemberOperationDetails>();
    }

    public class MemberOperationDetails
    {
        public string MemberObjectId { get; set; }

        public string Operation { get; set; }

        public DateTimeOffset OperationTime { get; set; }
    }
}
