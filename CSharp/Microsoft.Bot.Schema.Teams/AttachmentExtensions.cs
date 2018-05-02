// <copyright file="AttachmentExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Attachment extensions.
    /// </summary>
    public static class AttachmentExtensions
    {
        /// <summary>
        /// Converts normal attachment into the compose extension attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <param name="previewAttachment">The preview attachment.</param>
        /// <returns>Compose extension attachment</returns>
        public static ComposeExtensionAttachment ToComposeExtensionAttachment(this Attachment attachment, Attachment previewAttachment = null)
        {
            // We are recreating the attachment so that JsonSerializerSettings with ReferenceLoopHandling set to Error does not generate error
            // while serializing. Refer to issue - https://github.com/OfficeDev/BotBuilder-MicrosoftTeams/issues/52.
            return new ComposeExtensionAttachment
            {
                Content = attachment.Content,
                ContentType = attachment.ContentType,
                ContentUrl = attachment.ContentUrl,
                Name = attachment.Name,
                ThumbnailUrl = attachment.ThumbnailUrl,
                Preview = previewAttachment == null ?
                    JObject.FromObject(attachment).ToObject<Attachment>() :
                    previewAttachment,
            };
        }
    }
}
