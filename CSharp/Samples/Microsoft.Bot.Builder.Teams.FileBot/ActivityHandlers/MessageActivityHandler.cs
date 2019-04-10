// <copyright file="MessageActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.FileBot.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles Teams invoke activity.
    /// </summary>
    /// <seealso cref="ITeamsInvokeActivityHandler" />
    public class MessageActivityHandler : IMessageActivityHandler
    {
        /// <summary>
        /// Handles the message activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Task tracking operation.</returns>
        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            bool messageWithFileDownloadInfo = turnContext.Activity.Attachments?[0].ContentType == FileDownloadInfo.ContentType;
            if (messageWithFileDownloadInfo)
            {
                Attachment file = turnContext.Activity.Attachments[0];
                FileDownloadInfo fileDownload = JObject.FromObject(file.Content).ToObject<FileDownloadInfo>();

                string filePath = "Files\\" + file.Name;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fileDownload.DownloadUrl, filePath);
                }

                var reply = turnContext.Activity.CreateReply();
                reply.TextFormat = "xml";
                reply.Text = $"Complete downloading <b>{file.Name}</b>";
                await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
            }
            else
            {
                string filename = "teams-logo.png";
                string filePath = "Files\\" + filename;
                long fileSize = new FileInfo(filePath).Length;
                await this.SendFileCardAsync(turnContext, filename, fileSize).ConfigureAwait(false);
            }
        }

        private async Task SendFileCardAsync(ITurnContext turnContext, string filename, long filesize)
        {
            var consentContext = new Dictionary<string, string>
            {
                { "filename", filename },
            };

            var fileCard = new FileConsentCard
            {
                Description = "This is the file I want to send you",
                SizeInBytes = filesize,
                AcceptContext = consentContext,
                DeclineContext = consentContext,
            };

            Activity replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments = new List<Attachment>()
            {
                fileCard.ToAttachment(filename),
            };

            await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);
        }
    }
}
