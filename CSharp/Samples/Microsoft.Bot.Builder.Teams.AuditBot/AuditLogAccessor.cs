// <copyright file="AuditLogAccessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    using System;
    using Microsoft.Bot.Builder.Teams.SampleMiddlewares;

    /// <summary>
    /// Accessor to read and write audit logs to storage.
    /// </summary>
    public class AuditLogAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogAccessor"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the logs.</param>
        public AuditLogAccessor(TeamSpecificConversationState conversationState)
        {
            this.ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="AuditLog"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the counter accessor.</value>
        public static string AuditLogName { get; } = $"{nameof(AuditLogAccessor)}.AuditLog";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for EchoState.
        /// </summary>
        /// <value>
        /// The accessor stores the turn count for the conversation.
        /// </value>
        public IStatePropertyAccessor<TeamOperationHistory> AuditLog { get; set; }

        /// <summary>
        /// Gets the <see cref="TeamSpecificConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="TeamSpecificConversationState"/> object.</value>
        public TeamSpecificConversationState ConversationState { get; }
    }
}
