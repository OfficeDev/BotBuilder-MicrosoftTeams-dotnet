using Microsoft.Bot.Builder.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    public class AuditBot : IBot
    {
        IActivityProcessor activityProcessor;
        public AuditBot(IActivityProcessor activityProcessor)
        {
            this.activityProcessor = activityProcessor;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.activityProcessor.ProcessIncomingActivityAsync(turnContext);
        }
    }
}
