using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    public class DenyTeamMessages : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();

            if (teamsChannelData.Team == null)
            {
                await next(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await context.SendActivityAsync("This bot does not work in teams");
            }
        }
    }
}
