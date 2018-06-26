namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams specific conversation state management.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <seealso cref="Microsoft.Bot.Builder.BotState{TState}" />
    public class TeamSpecificConversationState<TState> : BotState<TState>
        where TState : class, new()
    {
        /// <summary>
        /// The key to use to read and write this conversation state object to storage.
        /// </summary>
        public static string PropertyName = $"TeamSpecificConversationState:{typeof(TeamSpecificConversationState<TState>).Namespace}.{typeof(TeamSpecificConversationState<TState>).Name}";

        /// <summary>
        /// Creates a new <see cref="TeamSpecificConversationState{TState}"/> object.
        /// </summary>
        /// <param name="storage">The storage provider to use.</param>
        /// <param name="settings">The state persistance options to use.</param>
        public TeamSpecificConversationState(IStorage storage, StateSettings settings = null) :
            base(storage, PropertyName,
                (context) =>
                {
                    TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();
                    return $"conversation/{context.Activity.ChannelId}/{teamsChannelData.Team.Id}";
                },
                settings)
        {
        }

        /// <summary>
        /// Gets the conversation state object from turn context.
        /// </summary>
        /// <param name="context">The context object for this turn.</param>
        /// <returns>The coversation state object.</returns>
        public static TState Get(ITurnContext context)
        {
            return context.Services.Get<TState>(PropertyName);
        }
    }
}
