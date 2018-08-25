namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema.Teams;

    public class DenyNonTeamMessage : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();

            if (string.IsNullOrEmpty(teamsChannelData.Team?.Id))
            {
                await context.SendActivityAsync("This bot only works in Teams");
            }
            else
            {
                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
