// <copyright file="TeamsConversationUpdateActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams conversation update activity handler.
    /// </summary>
    /// <seealso cref="ITeamsConversationUpdateActivityHandler" />
    public class TeamsConversationUpdateActivityHandler : ITeamsConversationUpdateActivityHandler
    {
        /// <summary>
        /// The team history accessor.
        /// </summary>
        private readonly AuditLogAccessor teamHistoryAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsConversationUpdateActivityHandler"/> class.
        /// </summary>
        /// <param name="teamHistoryAccessor">The team history accessor.</param>
        public TeamsConversationUpdateActivityHandler(AuditLogAccessor teamHistoryAccessor)
        {
            this.teamHistoryAccessor = teamHistoryAccessor;
        }

        /// <summary>
        /// Handles the channel created event asynchronous.
        /// </summary>
        /// <param name="channelCreatedEvent">The channel created event.</param>
        /// <returns>Task tracking operation.</returns>
        public async Task HandleChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(channelCreatedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelCreatedEvent.Channel.Id,
                Operation = "ChannelCreated",
                OperationTime = DateTimeOffset.Now,
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelCreatedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelCreatedEvent.TurnContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the channel deleted event asynchronously.
        /// </summary>
        /// <param name="channelDeletedEvent">The channel deleted event.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(channelDeletedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelDeletedEvent.Channel.Id,
                Operation = "ChannelDeleted",
                OperationTime = DateTimeOffset.Now,
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelDeletedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelDeletedEvent.TurnContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the channel renamed event asynchronously.
        /// </summary>
        /// <param name="channelRenamedEvent">The channel renamed event.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(channelRenamedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelRenamedEvent.Channel.Id,
                Operation = "ChannelRenamed",
                OperationTime = DateTimeOffset.Now,
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelRenamedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelRenamedEvent.TurnContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the conversation update activity task asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public Task HandleConversationUpdateActivityTaskAsync(ITurnContext turnContext)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the team members added event asynchronously.
        /// </summary>
        /// <param name="teamMembersAddedEvent">The team members added event.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(teamMembersAddedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            foreach (ChannelAccount memberAdded in teamMembersAddedEvent.MembersAdded)
            {
                ITeamsExtension teamsExtension = teamMembersAddedEvent.TurnContext.TurnState.Get<ITeamsExtension>();
                TeamsChannelAccount teamsChannelAccount = teamsExtension.AsTeamsChannelAccount(memberAdded);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<OperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberAdded",
                    OperationTime = DateTimeOffset.Now,
                });
            }

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamMembersAddedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamMembersAddedEvent.TurnContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the team members removed event asynchronously.
        /// </summary>
        /// <param name="teamMembersRemovedEvent">The team members removed event.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(teamMembersRemovedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            foreach (ChannelAccount memberRemoved in teamMembersRemovedEvent.MembersRemoved)
            {
                ITeamsExtension teamsExtension = teamMembersRemovedEvent.TurnContext.TurnState.Get<ITeamsExtension>();
                TeamsChannelAccount teamsChannelAccount = teamsExtension.AsTeamsChannelAccount(memberRemoved);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<OperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberRemoved",
                    OperationTime = DateTimeOffset.Now,
                });
            }

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamMembersRemovedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamMembersRemovedEvent.TurnContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the team renamed event asynchronously.
        /// </summary>
        /// <param name="teamRenamedEvent">The team renamed event.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(teamRenamedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = teamRenamedEvent.Team.Id,
                Operation = "TeamRenamed",
                OperationTime = DateTimeOffset.Now,
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamRenamedEvent.TurnContext, conversationHistory).ConfigureAwait(false);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamRenamedEvent.TurnContext).ConfigureAwait(false);
        }
    }
}
