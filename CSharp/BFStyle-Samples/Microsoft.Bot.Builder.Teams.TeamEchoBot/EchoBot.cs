using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Teams.TeamEchoBot
{
    public class EchoBot : IBot
    {
        private readonly IStatePropertyAccessor<EchoState> stateAccessor;

        public EchoBot(IStatePropertyAccessor<EchoState> stateAccessor)
        {
            this.stateAccessor = stateAccessor;
        }

        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                try
                {
                    // --> Get Teams Extensions.
                    ITeamsExtension teamsExtension = context.TurnState.Get<ITeamsExtension>();

                    var state = await stateAccessor.GetAsync(context, () => new EchoState());

                    state.TurnCount++;

                    await stateAccessor.SetAsync(context, state);

                    string suffixMessage = $"from tenant Id {teamsExtension.GetActivityTenantId()}";

                    // Echo back to the user whatever they typed.
                    await context.SendActivityAsync($"Turn {state.TurnCount}: You sent '{context.Activity.Text}' {suffixMessage}");
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
