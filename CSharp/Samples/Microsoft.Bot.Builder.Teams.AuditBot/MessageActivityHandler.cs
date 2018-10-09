// <copyright file="MessageActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Message activity handler.
    /// </summary>
    /// <seealso cref="IMessageActivityHandler" />
    public class MessageActivityHandler : IMessageActivityHandler
    {
        /// <summary>
        /// The team history accessor.
        /// </summary>
        private readonly AuditLogAccessor teamHistoryAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActivityHandler"/> class.
        /// </summary>
        /// <param name="teamHistoryAccessor">The team history accessor.</param>
        public MessageActivityHandler(AuditLogAccessor teamHistoryAccessor)
        {
            this.teamHistoryAccessor = teamHistoryAccessor;
        }

        /// <summary>
        /// Handles the message activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            ITeamsExtension teamsExtension = turnContext.TurnState.Get<ITeamsExtension>();

            string actualText = teamsExtension.GetActivityTextWithoutMentions();
            if (actualText.Equals("ShowHistory", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show History", StringComparison.OrdinalIgnoreCase))
            {
                TeamOperationHistory memberHistory = await this.teamHistoryAccessor.AuditLog.GetAsync(turnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

                Activity replyActivity = turnContext.Activity.CreateReply();

                teamsExtension.AddMentionToText(replyActivity, turnContext.Activity.From);
                replyActivity.Text = replyActivity.Text + $" Total of {memberHistory.MemberOperations.Count} operations were performed";

                await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);

                // Going in reverse chronological order.
                for (int i = memberHistory.MemberOperations.Count % 10; i >= 0; i--)
                {
                    List<OperationDetails> elementsToSend = memberHistory.MemberOperations.Skip(10 * i).Take(10).ToList();

                    StringBuilder stringBuilder = new StringBuilder();

                    if (elementsToSend.Count > 0)
                    {
                        for (int j = elementsToSend.Count - 1; j >= 0; j--)
                        {
                            stringBuilder.Append($"{elementsToSend[j].ObjectId} --> {elementsToSend[j].Operation} -->  {elementsToSend[j].OperationTime} </br>");
                        }

                        Activity memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity).ConfigureAwait(false);
                    }
                }
            }
            else if (actualText.Equals("ShowCurrentMembers", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Current Members", StringComparison.OrdinalIgnoreCase))
            {
                List<ChannelAccount> teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(
                    turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id).ConfigureAwait(false)).ToList();

                Activity replyActivity = turnContext.Activity.CreateReply();
                teamsExtension.AddMentionToText(replyActivity, turnContext.Activity.From);
                replyActivity.Text = replyActivity.Text + $" Total of {teamMembers.Count} members are currently in team";

                await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);

                for (int i = teamMembers.Count % 10; i >= 0; i--)
                {
                    List<TeamsChannelAccount> elementsToSend = teamMembers.Skip(10 * i).Take(10).ToList().ConvertAll<TeamsChannelAccount>((account) => teamsExtension.AsTeamsChannelAccount(account));

                    StringBuilder stringBuilder = new StringBuilder();

                    if (elementsToSend.Count > 0)
                    {
                        for (int j = elementsToSend.Count - 1; j >= 0; j--)
                        {
                            stringBuilder.Append($"{elementsToSend[j].AadObjectId} --> {elementsToSend[j].Name} -->  {elementsToSend[j].UserPrincipalName} </br>");
                        }

                        Activity memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity).ConfigureAwait(false);
                    }
                }
            }
            else if (actualText.Equals("ShowChannelList", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("ShowChannels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channel List", StringComparison.OrdinalIgnoreCase))
            {
                ConversationList channelList = await teamsExtension.Teams.FetchChannelListAsync(turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id).ConfigureAwait(false);

                Activity replyActivity = turnContext.Activity.CreateReply();
                teamsExtension.AddMentionToText(replyActivity, turnContext.Activity.From);
                replyActivity.Text = replyActivity.Text + $" Total of {channelList.Conversations.Count} channels are currently in team";

                await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);

                for (int i = channelList.Conversations.Count % 10; i >= 0; i--)
                {
                    List<ChannelInfo> elementsToSend = channelList.Conversations.Skip(10 * i).Take(10).ToList();

                    StringBuilder stringBuilder = new StringBuilder();

                    if (elementsToSend.Count > 0)
                    {
                        for (int j = elementsToSend.Count - 1; j >= 0; j--)
                        {
                            stringBuilder.Append($"{elementsToSend[j].Id} --> {elementsToSend[j].Name}</br>");
                        }

                        Activity memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                await turnContext.SendActivityAsync("Invalid command").ConfigureAwait(false);
            }
        }
    }
}
