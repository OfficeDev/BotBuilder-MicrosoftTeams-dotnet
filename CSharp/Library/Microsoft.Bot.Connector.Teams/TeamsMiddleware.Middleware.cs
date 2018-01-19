using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Middleware;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Connector.Teams
{
    public class TeamsMiddleware : IContextCreated, IReceiveActivity
    {
        public async Task ContextCreated(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            // We only work in MS Teams scenario.
            if (context.Request.ChannelId.Equals("msteams"))
            {
                context.ToBotContext().Add("TeamsMiddleware", this);
            }

            await next().ConfigureAwait(false);
        }

        public async Task ReceiveActivity(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            context.Request.ChannelData = context.Request.GetChannelData<TeamsChannelData>();
            context.Request.From = JObject.FromObject(context.Request.From).ToObject<TeamsChannelAccount>();
        }
    }
}
