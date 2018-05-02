// <copyright file="CardExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
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
    }
}
