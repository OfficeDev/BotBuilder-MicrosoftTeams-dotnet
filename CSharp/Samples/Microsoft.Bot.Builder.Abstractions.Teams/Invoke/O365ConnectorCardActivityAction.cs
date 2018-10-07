// <copyright file="O365ConnectorCardActivityAction.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Handles the O365 Connection card activity action.
    /// </summary>
    /// <seealso cref="TeamsInvokeActivityActionBase" />
    public class O365ConnectorCardActivityAction : TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets the card action query.
        /// </summary>
        public O365ConnectorCardActionQuery CardActionQuery { get; internal set; }
    }
}
