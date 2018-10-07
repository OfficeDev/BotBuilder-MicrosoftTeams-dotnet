using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    public class TeamsConversationUpdateActivityHandler : ITeamsConversationUpdateActivityHandler
    {
        private readonly AuditLogAccessor teamHistoryAccessor;

        public TeamsConversationUpdateActivityHandler(AuditLogAccessor teamHistoryAccessor)
        {
            this.teamHistoryAccessor = teamHistoryAccessor;
        }

        /// <summary>
        /// Handles the channel created event asynchronous.
        /// </summary>
        /// <param name="channelCreatedEvent">The channel created event.</param>
        /// <returns></returns>
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
                OperationTime = DateTimeOffset.Now
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelCreatedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelCreatedEvent.TurnContext);
        }

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
                OperationTime = DateTimeOffset.Now
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelDeletedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelDeletedEvent.TurnContext);
        }

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
                OperationTime = DateTimeOffset.Now
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(channelRenamedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(channelRenamedEvent.TurnContext);
        }

        public Task HandleConversationUpdateActivityTask(ITurnContext turnContext)
        {
            return Task.CompletedTask;
        }

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
                    OperationTime = DateTimeOffset.Now
                });
            }

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamMembersAddedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamMembersAddedEvent.TurnContext);
        }

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
                    OperationTime = DateTimeOffset.Now
                });
            }

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamMembersRemovedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamMembersRemovedEvent.TurnContext);
        }

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
                OperationTime = DateTimeOffset.Now
            });

            await this.teamHistoryAccessor.AuditLog.SetAsync(teamRenamedEvent.TurnContext, conversationHistory);
            await this.teamHistoryAccessor.ConversationState.SaveChangesAsync(teamRenamedEvent.TurnContext);
        }
    }
}
