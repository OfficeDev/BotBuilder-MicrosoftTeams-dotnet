// <copyright file="CardExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///  Card extension methods.
    /// </summary>
    public static partial class CardExtensions
    {
        /// <summary>
        /// Creates a new attachment from <see cref="O365ConnectorCard"/>.
        /// </summary>
        /// <param name="card"> The instance of <see cref="O365ConnectorCard"/>.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this O365ConnectorCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = O365ConnectorCard.ContentType,
            };
        }

        /// <summary>
        /// Creates a new attachment from <see cref="FileInfoCard"/>.
        /// </summary>
        /// <param name="card"> The instance of <see cref="FileInfoCard"/>.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this FileInfoCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = FileInfoCard.ContentType,
            };
        }

        /// <summary>
        /// Creates a new attachment from <see cref="FileConsentCard"/>.
        /// </summary>
        /// <param name="card"> The instance of <see cref="FileConsentCard"/>.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this FileConsentCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = FileConsentCard.ContentType,
            };
        }

        /// <summary>
        /// Creates a new attachment from AdaptiveCard.
        /// </summary>
        /// <param name="card"> The instance of AdaptiveCard.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this AdaptiveCards.AdaptiveCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = AdaptiveCards.AdaptiveCard.ContentType,
            };
        }

        /// <summary>
        /// Creates a new attachment from AdaptiveCardParseResult.
        /// </summary>
        /// <param name="cardParsedResult"> The instance of AdaptiveCardParseResult that represents results parsed from JSON string.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this AdaptiveCards.AdaptiveCardParseResult cardParsedResult)
        {
            return cardParsedResult.Card.ToAttachment();
        }

        /// <summary>
        /// Wrap BotBuilder action into AdaptiveCard.
        /// </summary>
        /// <param name="action"> The instance of adaptive card.</param>
        /// <param name="targetAction"> Target action to be adapted.</param>
        public static void RepresentAsBotBuilderAction(this AdaptiveCards.AdaptiveSubmitAction action, CardAction targetAction)
        {
            var wrappedAction = new CardAction
            {
                Type = targetAction.Type,
                Value = targetAction.Value,
                Text = targetAction.Text,
                DisplayText = targetAction.DisplayText,
            };

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string jsonStr = action.DataJson == null ? "{}" : action.DataJson;
            JToken dataJson = JObject.Parse(jsonStr);
            dataJson["msteams"] = JObject.FromObject(wrappedAction, JsonSerializer.Create(serializerSettings));

            action.Title = targetAction.Title;
            action.DataJson = dataJson.ToString();
        }
    }
}
