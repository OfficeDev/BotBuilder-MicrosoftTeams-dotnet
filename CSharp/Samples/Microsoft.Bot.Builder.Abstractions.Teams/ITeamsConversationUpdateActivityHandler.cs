namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;

    public interface ITeamsConversationUpdateActivityHandler : IConversationUpdateActivityHandler
    {
        Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent);

        Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent);

        Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent);

        Task HandleChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent);

        Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent);

        Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent);
    }
}
