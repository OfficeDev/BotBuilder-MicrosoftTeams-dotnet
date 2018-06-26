using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Teams.TeamsMemberHistoryBot.Engine
{
    public class MessageActivityHandler : IMessageActivityHandler
    {
        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            ITeamsExtension teamsExtension = turnContext.Services.Get<ITeamsExtension>();

            string actualText = teamsExtension.GetActivityTextWithoutMentions();
            if (actualText.Equals("ShowHistory", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show History", StringComparison.OrdinalIgnoreCase))
            {
                ConversationMemberHistory memberHistory = TeamSpecificConversationState<ConversationMemberHistory>.Get(turnContext);

                Activity replyActivity = turnContext.Activity.CreateReply();

                teamsExtension.AddMentionToText(replyActivity, turnContext.Activity.From);
                replyActivity.Text = replyActivity.Text + $" Total of {memberHistory.MemberOperations.Count} operations were performed";

                // Temporary Fix for Mentions not working
                (replyActivity.Entities[0] as Mention).Type = "mention";
                await turnContext.SendActivity(replyActivity);

                // Going in reverse chronological order.
                for (int i = memberHistory.MemberOperations.Count % 10; i >= 0; i--)
                {
                    List<MemberOperationDetails> elementsToSend = memberHistory.MemberOperations.Skip(10 * i).Take(10).ToList();

                    StringBuilder stringBuilder = new StringBuilder();

                    if (elementsToSend.Count > 0)
                    {
                        for (int j = elementsToSend.Count - 1; j >= 0; j--)
                        {
                            stringBuilder.Append($"{elementsToSend[j].MemberObjectId} --> {elementsToSend[j].Operation} -->  {elementsToSend[j].OperationTime} </br>");
                        }

                        Activity memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivity(memberListActivity);
                    }
                }
            }
            else
            {
                await turnContext.SendActivity("Type ShowHistory to show history");
            }
        }
    }
}
