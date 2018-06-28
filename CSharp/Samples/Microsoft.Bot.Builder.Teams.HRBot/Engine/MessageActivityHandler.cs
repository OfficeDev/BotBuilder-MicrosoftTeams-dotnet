using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions;

namespace Microsoft.Bot.Builder.Teams.HRBot.Engine
{
    public class MessageActivityHandler : IMessageActivityHandler
    {
        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            await turnContext.SendActivity("If you seeing this message you are allowed!!");
        }
    }
}
