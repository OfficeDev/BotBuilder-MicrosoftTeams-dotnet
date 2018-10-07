// <copyright file="ITeamsInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;

    /// <summary>
    /// Handles the teams invoke activities.
    /// </summary>
    /// <seealso cref="IInvokeActivityHandler" />
    public interface ITeamsInvokeActivityHandler : IInvokeActivityHandler
    {
        /// <summary>
        /// Handles the o365 connector card action asynchronously.
        /// </summary>
        /// <param name="o365ConnectorCardActionAction">The o365 connector card action action.</param>
        /// <returns>Task tracking operation.</returns>
        Task<InvokeResponse> HandleO365ConnectorCardActionAsync(O365ConnectorCardActivityAction o365ConnectorCardActionAction);

        /// <summary>
        /// Handles the signin state verification action asynchronously.
        /// </summary>
        /// <param name="signinStateVerificationAction">The signin state verification action.</param>
        /// <returns>Task tracking operation.</returns>
        Task<InvokeResponse> HandleSigninStateVerificationActionAsync(SigninStateVerificationActivityAction signinStateVerificationAction);

        /// <summary>
        /// Handles the messaging extension action asynchronously.
        /// </summary>
        /// <param name="messagingExtensionAction">The messaging extension action.</param>
        /// <returns>Task tracking operation.</returns>
        Task<InvokeResponse> HandleMessagingExtensionActionAsync(MessagingExtensionActivityAction messagingExtensionAction);
    }
}
