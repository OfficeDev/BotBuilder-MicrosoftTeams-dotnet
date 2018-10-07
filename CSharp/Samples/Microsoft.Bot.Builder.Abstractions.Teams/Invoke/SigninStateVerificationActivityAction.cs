// <copyright file="SigninStateVerificationActivityAction.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Handles the Signin State verification activity.
    /// </summary>
    /// <seealso cref="TeamsInvokeActivityActionBase" />
    public class SigninStateVerificationActivityAction : TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets the verification query.
        /// </summary>
        public SigninStateVerificationQuery VerificationQuery { get; internal set; }
    }
}
