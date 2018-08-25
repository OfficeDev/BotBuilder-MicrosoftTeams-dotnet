using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.TeamHistoryBot.Engine
{
    public class TeamsConversationUpdateActivityHandler : ITeamsConversationUpdateActivityHandler
    {
        private readonly IStatePropertyAccessor<TeamOperationHistory> teamHistoryAccessor;

        public TeamsConversationUpdateActivityHandler(IStatePropertyAccessor<TeamOperationHistory> teamHistoryAccessor)
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
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(channelCreatedEvent.TurnContext).ConfigureAwait(false);

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
        }

        public async Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(channelDeletedEvent.TurnContext).ConfigureAwait(false);

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
        }

        public async Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(channelRenamedEvent.TurnContext).ConfigureAwait(false);

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
        }

        public Task HandleConversationUpdateActivityTask(ITurnContext turnContext)
        {
            return Task.CompletedTask;
        }

        public async Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(teamMembersAddedEvent.TurnContext).ConfigureAwait(false);

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
        }

        public async Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(teamMembersRemovedEvent.TurnContext).ConfigureAwait(false);

            foreach (ChannelAccount memberAdded in teamMembersRemovedEvent.MembersRemoved)
            {
                ITeamsExtension teamsExtension = teamMembersRemovedEvent.TurnContext.TurnState.Get<ITeamsExtension>();
                TeamsChannelAccount teamsChannelAccount = teamsExtension.AsTeamsChannelAccount(memberAdded);

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
        }

        public async Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            TeamOperationHistory conversationHistory = await this.teamHistoryAccessor.GetAsync(teamRenamedEvent.TurnContext).ConfigureAwait(false);

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
        }
    }
}
