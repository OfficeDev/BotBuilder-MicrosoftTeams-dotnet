using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Bot.Builder;

namespace Microsoft.Bot.Connector.Teams
{
    public static class TeamMiddlewareExtensions
    {
        public static ITeamsMiddleware GetTeamsMiddleware(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            return botContext.ToBotContext()["TeamsMiddleware"] as ITeamsMiddleware;
        }

        internal static void EnsureTeamsMiddlewareEnabled(this IBotContext botContext)
        {
            if (!botContext.ToBotContext().ContainsKey("TeamsMiddleware"))
            {
                throw new InvalidOperationException("Teams Middleware is not enabled, Teams middleware can only be used on calls Microsoft Teams calls.");
            }
        }

        internal static void EnsureResponseExists(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            if (botContext.Responses?.Count == 0)
            {
                throw new ArgumentException("Response activity needs to be assigned before this method can be called.", nameof(botContext));
            }
        }
    }
}
