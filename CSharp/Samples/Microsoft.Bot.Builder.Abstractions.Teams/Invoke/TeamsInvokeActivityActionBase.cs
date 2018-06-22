namespace Microsoft.Bot.Builder.Abstractions.Teams.Invoke
{
    public abstract class TeamsInvokeActivityActionBase
    {
        /// <summary>
        /// Gets or sets the original activity.
        /// </summary>
        public ITurnContext TurnContext { get; set; }
    }
}
