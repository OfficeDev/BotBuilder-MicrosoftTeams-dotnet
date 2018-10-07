namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema.Teams;

    public class MessagingExtensionActivityAction : TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets or sets the compose extension query.
        /// </summary>
        public ComposeExtensionQuery ComposeExtensionQuery { get; set; }
    }
}
