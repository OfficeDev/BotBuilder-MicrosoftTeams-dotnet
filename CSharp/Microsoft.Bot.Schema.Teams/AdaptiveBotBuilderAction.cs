// <copyright file="AdaptiveBotBuilderAction.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace AdaptiveCards
{
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Adapter class to represent BotBuilder card action as adaptive card action (in type of Action.Submit).
    /// </summary>
    public class AdaptiveBotBuilderAction : AdaptiveSubmitAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveBotBuilderAction"/> class.
        /// Constructor must wrap an existing CardAction from BotBuilder framework.
        /// </summary>
        /// <param name="action">The bot builder action to be wrapped.</param>
        public AdaptiveBotBuilderAction(CardAction action)
        {
            this.RepresentAsBotBuilderAction(action);
        }
    }
}
