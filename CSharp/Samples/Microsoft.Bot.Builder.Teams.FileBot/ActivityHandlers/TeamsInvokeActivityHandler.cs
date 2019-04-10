// <copyright file="TeamsInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.FileBot.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles Teams invoke activity.
    /// </summary>
    /// <seealso cref="ITeamsInvokeActivityHandler" />
    public class TeamsInvokeActivityHandler : TeamsInvokeActivityHandlerBase
    {
        /// <summary>
        /// Handles file consent response asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of file consent user's response.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleFileConsentResponseAsync(ITurnContext turnContext, FileConsentCardResponse query)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>Received user's consent</b> <pre>{JObject.FromObject(query).ToString()}</pre>";
            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);

            JToken context = JObject.FromObject(query.Context);

            if (query.Action.Equals("accept"))
            {
                try
                {
                    string filePath = "Files\\" + context["filename"];
                    string fileUploadUrl = query.UploadInfo.UploadUrl;
                    long fileSize = new FileInfo(filePath).Length;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Content-Length", fileSize.ToString());
                        client.Headers.Add("Content-Range", $"bytes 0-{fileSize - 1}/{fileSize}");
                        using (Stream fileStream = File.OpenRead(filePath))
                        using (Stream requestStream = client.OpenWrite(new Uri(fileUploadUrl), "PUT"))
                        {
                            fileStream.CopyTo(requestStream);
                        }
                    }

                    await this.FileUploadCompletedAsync(turnContext, query).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await this.FileUploadFailedAsync(turnContext, e.ToString()).ConfigureAwait(false);
                }
            }

            if (query.Action.Equals("decline"))
            {
                reply.Text = $"Declined. We won't upload file <b>{context["filename"]}</b>.";
                await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
            }

            return null;
        }

        private async Task FileUploadCompletedAsync(ITurnContext turnContext, FileConsentCardResponse query)
        {
            var downloadCard = new FileInfoCard()
            {
                UniqueId = query.UploadInfo.UniqueId,
                FileType = query.UploadInfo.FileType,
            };

            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File uploaded.</b> Your file <b>{query.UploadInfo.Name}</b> is ready to download";
            reply.Attachments = new List<Attachment>
            {
                downloadCard.ToAttachment(query.UploadInfo.Name, query.UploadInfo.ContentUrl),
            };

            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
        }

        private async Task FileUploadFailedAsync(ITurnContext turnContext, string error)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.TextFormat = "xml";
            reply.Text = $"<b>File upload failed.</b> Error: <pre>{error}</pre>";
            await turnContext.SendActivityAsync(reply).ConfigureAwait(false);
        }
    }
}
