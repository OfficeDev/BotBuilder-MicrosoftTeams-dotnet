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
        /// <summary>
        /// Handles the channel created event asynchronous.
        /// </summary>
        /// <param name="channelCreatedEvent">The channel created event.</param>
        /// <returns></returns>
        public Task HandleChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(channelCreatedEvent.TurnContext);

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

            return Task.CompletedTask;
        }

        public Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(channelDeletedEvent.TurnContext);

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

            return Task.CompletedTask;
        }

        public Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(channelRenamedEvent.TurnContext);

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

            return Task.CompletedTask;
        }

        public Task HandleConversationUpdateActivityTask(ITurnContext turnContext)
        {
            return Task.CompletedTask;
        }

        public Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(teamMembersAddedEvent.TurnContext);

            foreach (ChannelAccount memberAdded in teamMembersAddedEvent.MembersAdded)
            {
                ITeamsExtension teamsExtension = teamMembersAddedEvent.TurnContext.Services.Get<ITeamsExtension>();
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

            return Task.CompletedTask;
        }

        public Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(teamMembersRemovedEvent.TurnContext);

            foreach (ChannelAccount memberAdded in teamMembersRemovedEvent.MembersRemoved)
            {
                ITeamsExtension teamsExtension = teamMembersRemovedEvent.TurnContext.Services.Get<ITeamsExtension>();
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

            return Task.CompletedTask;
        }

        public Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            TeamOperationHistory conversationHistory = TeamSpecificConversationState<TeamOperationHistory>.Get(teamRenamedEvent.TurnContext);

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

            return Task.CompletedTask;
        }
    }
}
