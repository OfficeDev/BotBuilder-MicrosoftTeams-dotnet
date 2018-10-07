// <copyright file="MessagingExtensionActivityAction.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Processes the messaging extension activity.
    /// </summary>
    /// <seealso cref="TeamsInvokeActivityActionBase" />
    public class MessagingExtensionActivityAction : TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets or sets the messaging extension query.
        /// </summary>
        public MessagingExtensionQuery MessagingExtensionQuery { get; set; }
    }
}
