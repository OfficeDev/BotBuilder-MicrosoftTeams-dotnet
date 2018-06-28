using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    public class DenyTeamMessages : IMiddleware
    {
        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();

            if (teamsChannelData.Team == null)
            {
                await next.Invoke().ConfigureAwait(false);
            }
            else
            {
                await context.SendActivity("This bot does not work in teams");
            }
        }
    }
}
