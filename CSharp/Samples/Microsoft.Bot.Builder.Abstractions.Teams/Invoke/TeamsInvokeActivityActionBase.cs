// <copyright file="TeamsInvokeActivityActionBase.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    /// <summary>
    /// Teams invoke activity base action.
    /// </summary>
    public abstract class TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets or sets the original activity.
        /// </summary>
        public ITurnContext TurnContext { get; set; }
    }
}
