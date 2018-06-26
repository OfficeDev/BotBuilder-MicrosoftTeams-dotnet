namespace Microsoft.Bot.Builder.Teams.SampleMiddlewares
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema.Teams;

    public class DenyNonTeamMessage : IMiddleware
    {
        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            TeamsChannelData teamsChannelData = context.Activity.GetChannelData<TeamsChannelData>();

            if (string.IsNullOrEmpty(teamsChannelData.Team?.Id))
            {
                await context.SendActivity("This bot only works in Teams");
            }
            else
            {
                await next.Invoke().ConfigureAwait(false);
            }
        }
    }
}
