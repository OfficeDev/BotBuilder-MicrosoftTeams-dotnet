namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;

    public interface ITeamsInvokeActivityHandler : IInvokeActivityHandler
    {
        Task<InvokeResponse> HandleO365ConnectorCardActionAsync(O365ConnectorCardActivityAction o365ConnectorCardActionAction);

        Task<InvokeResponse> HandleSigninStateVerificationActionAsync(SigninStateVerificationActivityAction signinStateVerificationAction);

        Task<InvokeResponse> HandleMessagingExtensionActionAsync(MessagingExtensionActivityAction messagingExtensionAction);
    }
}
