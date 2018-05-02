// <copyright file="FileCardsTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests.CardTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// File cards tests.
    /// </summary>
    [TestClass]
    public class FileCardsTests
    {
        /// <summary>
        /// File info card test.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task CardTests_FileInfoCardAsync()
        {
            FileInfoCard fileInfoCard = new FileInfoCard
            {
                FileType = "txt",
                UniqueId = Guid.NewGuid().ToString(),
                Etag = Guid.NewGuid().ToString(),
            };

            Attachment attachment = fileInfoCard.ToAttachment();
            Assert.AreEqual(FileInfoCard.ContentType, attachment.ContentType);
            await TestHelpers.TestAttachmentAsync(attachment).ConfigureAwait(false);
        }

        /// <summary>
        /// File consent card test.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task CardTests_FileConsentCardAsync()
        {
            FileConsentCard fileConsentCard = new FileConsentCard
            {
                Description = "File consent",
                SizeInBytes = 1024,
            };

            Attachment attachment = fileConsentCard.ToAttachment();
            Assert.AreEqual(FileConsentCard.ContentType, attachment.ContentType);
            await TestHelpers.TestAttachmentAsync(attachment).ConfigureAwait(false);
        }

        /// <summary>
        /// File download info attachment.
        /// </summary>
        [TestMethod]
        public void CardTests_FileDownloadInfoAttachment()
        {
            FileDownloadInfo fileDownloadInfo = new FileDownloadInfo
            {
                DownloadUrl = "https://bing.com",
                UniqueId = "b83b9f77-7003-4d63-985c-9611c98303f3",
                FileType = "txt",
                Etag = "078251f7-12bb-4132-93e4-2f2bb05fee8c",
            };

            string contents = JsonConvert.SerializeObject(new Attachment
            {
                Content = fileDownloadInfo,
                ContentType = FileDownloadInfo.ContentType,
            });
            Attachment attachment = JsonConvert.DeserializeObject<Attachment>(File.ReadAllText(@"Jsons\SampleFileDownloadInfoAttachment.json"));

            Assert.IsNotNull(attachment);
            Assert.IsNotNull(attachment.Content);
            Assert.IsTrue(JObject.DeepEquals(JObject.FromObject(fileDownloadInfo), JObject.FromObject(attachment.Content)));
            Assert.AreEqual(FileDownloadInfo.ContentType, attachment.ContentType);
        }
    }
}
