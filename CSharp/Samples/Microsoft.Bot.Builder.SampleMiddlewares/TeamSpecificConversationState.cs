namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams specific conversation state management.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <seealso cref="Microsoft.Bot.Builder.BotState" />
    public class TeamSpecificConversationState : BotState
    {
        /// <summary>
        /// The key to use to read and write this conversation state object to storage.
        /// </summary>
        public static string PropertyName = $"TeamSpecificConversationState:{typeof(TeamSpecificConversationState).Namespace}.{typeof(TeamSpecificConversationState).Name}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpecificConversationState"/> class.
        /// Creates a new <see cref="TeamSpecificConversationState"/> object.
        /// </summary>
        /// <param name="storage">The storage provider to use.</param>
        /// <param name="settings">The state persistance options to use.</param>
        public TeamSpecificConversationState(IStorage storage)
            : base(storage, PropertyName)
        {
        }

        /// <summary>
        /// Gets the key to use when reading and writing state to and from storage.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <returns>The storage key.</returns>
        protected override string GetStorageKey(ITurnContext turnContext)
        {
            TeamsChannelData teamsChannelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            if (string.IsNullOrEmpty(teamsChannelData.Team?.Id))
            {
                return $"chat/{turnContext.Activity.ChannelId}/{turnContext.Activity.Conversation.Id}";
            }
            else
            {
                return $"team/{turnContext.Activity.ChannelId}/{teamsChannelData.Team.Id}";
            }
        }
    }
}
