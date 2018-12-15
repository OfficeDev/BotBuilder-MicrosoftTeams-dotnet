// <copyright file="EchoBot.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.TeamEchoBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Echo bot.
    /// </summary>
    /// <seealso cref="Microsoft.Bot.Builder.IBot" />
    public class EchoBot : IBot
    {
        /// <summary>
        /// The echo state accessor.
        /// </summary>
        private readonly EchoStateAccessor echoStateAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoBot"/> class.
        /// </summary>
        /// <param name="echoStateAccessor">The echo state accessor.</param>
        public EchoBot(EchoStateAccessor echoStateAccessor)
        {
            this.echoStateAccessor = echoStateAccessor;
        }

        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them.
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task tracking operation.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                try
                {
                    // --> Get Teams Extensions.
                    ITeamsContext teamsContext = context.TurnState.Get<ITeamsContext>();

                    EchoState state = await this.echoStateAccessor.CounterState.GetAsync(context, () => new EchoState()).ConfigureAwait(false);

                    state.TurnCount++;

                    await this.echoStateAccessor.CounterState.SetAsync(context, state).ConfigureAwait(false);

                    await this.echoStateAccessor.ConversationState.SaveChangesAsync(context).ConfigureAwait(false);

                    string suffixMessage = $"from tenant Id {teamsContext.Tenant.Id}";

                    // Echo back to the user whatever they typed.
                    await context.SendActivityAsync($"Turn {state.TurnCount}: You sent '{context.Activity.Text}' {suffixMessage}").ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
