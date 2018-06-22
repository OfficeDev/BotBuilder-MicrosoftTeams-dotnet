namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    using Microsoft.Bot.Schema.Teams;

    public class SigninStateVerificationActivityAction : TeamsInvokeActivityActionBase
    {
        public SigninStateVerificationQuery VerificationQuery { get; internal set; }
    }
}
