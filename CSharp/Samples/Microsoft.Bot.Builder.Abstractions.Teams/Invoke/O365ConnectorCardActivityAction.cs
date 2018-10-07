namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using Microsoft.Bot.Schema.Teams;

    public class O365ConnectorCardActivityAction : TeamsInvokeActivityActionBase
    {
        public O365ConnectorCardActionQuery CardActionQuery { get; internal set; }
    }
}
