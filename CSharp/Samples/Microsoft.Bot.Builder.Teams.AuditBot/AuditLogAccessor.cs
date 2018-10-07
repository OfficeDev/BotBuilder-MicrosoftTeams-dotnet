using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    public class AuditLogAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoBotAccessors"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the counter.</param>
        public AuditLogAccessor(TeamSpecificConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
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
