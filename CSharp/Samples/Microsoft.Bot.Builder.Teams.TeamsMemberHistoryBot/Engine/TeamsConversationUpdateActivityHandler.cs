using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.TeamsMemberHistoryBot.Engine
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
            return Task.CompletedTask;
        }

        public Task HandleChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            return Task.CompletedTask;
        }

        public Task HandleChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            return Task.CompletedTask;
        }

        public Task HandleConversationUpdateActivityTask(ITurnContext turnContext)
        {
            return Task.CompletedTask;
        }

        public Task HandleTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            ConversationMemberHistory conversationHistory = TeamSpecificConversationState<ConversationMemberHistory>.Get(teamMembersAddedEvent.TurnContext);

            foreach (ChannelAccount memberAdded in teamMembersAddedEvent.MembersAdded)
            {
                ITeamsExtension teamsExtension = teamMembersAddedEvent.TurnContext.Services.Get<ITeamsExtension>();
                TeamsChannelAccount teamsChannelAccount = teamsExtension.AsTeamsChannelAccount(memberAdded);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<MemberOperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new MemberOperationDetails
                {
                    MemberObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "Add",
                    OperationTime = DateTimeOffset.Now
                });
            }

            return Task.CompletedTask;
        }

        public Task HandleTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            ConversationMemberHistory conversationHistory = TeamSpecificConversationState<ConversationMemberHistory>.Get(teamMembersRemovedEvent.TurnContext);

            foreach (ChannelAccount memberAdded in teamMembersRemovedEvent.MembersRemoved)
            {
                ITeamsExtension teamsExtension = teamMembersRemovedEvent.TurnContext.Services.Get<ITeamsExtension>();
                TeamsChannelAccount teamsChannelAccount = teamsExtension.AsTeamsChannelAccount(memberAdded);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<MemberOperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new MemberOperationDetails
                {
                    MemberObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "Remove",
                    OperationTime = DateTimeOffset.Now
                });
            }

            return Task.CompletedTask;
        }

        public Task HandleTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            return Task.CompletedTask;
        }
    }
}
