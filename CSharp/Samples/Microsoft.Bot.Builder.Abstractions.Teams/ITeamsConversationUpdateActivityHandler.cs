// <copyright file="ITeamsConversationUpdateActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;

    /// <summary>
    /// Teams conversation update activity handler.
    /// </summary>
    /// <seealso cref="IConversationUpdateActivityHandler" />
    public interface ITeamsConversationUpdateActivityHandler : IConversationUpdateActivityHandler
    {
        /// <summary>
        /// Handles the team members added event asynchronously.
        /// </summary>
        /// <param name="teamMembersAddedEvent">The team members added event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent);

        /// <summary>
        /// Handles the team members removed event asynchronously.
        /// </summary>
        /// <param name="teamMembersRemovedEvent">The team members removed event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent);

        /// <summary>
        /// Handles the team renamed event asynchronously.
        /// </summary>
        /// <param name="teamRenamedEvent">The team renamed event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent);

        /// <summary>
        /// Handles the channel created event asynchronously.
        /// </summary>
        /// <param name="channelCreatedEvent">The channel created event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent);

        /// <summary>
        /// Handles the channel renamed event asynchronously.
        /// </summary>
        /// <param name="channelRenamedEvent">The channel renamed event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent);

        /// <summary>
        /// Handles the channel deleted event asynchronously.
        /// </summary>
        /// <param name="channelDeletedEvent">The channel deleted event.</param>
        /// <returns>Task tracking operation.</returns>
        Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent);
    }
}
