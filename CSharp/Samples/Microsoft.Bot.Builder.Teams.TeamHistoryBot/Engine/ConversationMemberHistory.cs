using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Teams.TeamHistoryBot.Engine
{
    /// <summary>
    /// Team operation history.
    /// </summary>
    public class TeamOperationHistory
    {
        /// <summary>
        /// Gets or sets the member operations. Operation is a tuple of ObjectId, Operation and Time.
        /// </summary>
        public List<OperationDetails> MemberOperations { get; set; } = new List<OperationDetails>();
    }

    public class OperationDetails
    {
        public string ObjectId { get; set; }

        public string Operation { get; set; }

        public DateTimeOffset OperationTime { get; set; }
    }
}
