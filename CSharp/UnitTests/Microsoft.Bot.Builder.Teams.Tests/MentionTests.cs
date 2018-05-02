// <copyright file="MentionTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Rest;

    /// <summary>
    /// @Mention tests.
    /// </summary>
    [TestClass]
    public class MentionTests
    {
        /// <summary>
        /// @Mention tests with no mention text.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task AddMention_NoMentionTextAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity reply = sampleActivity.CreateReply();
                    reply = teamsExtension.AddMentionToText(reply, sampleActivity.From);

                    Assert.IsTrue(reply.Entities.Count == 1);
                    Assert.IsInstanceOfType(reply.Entities[0], typeof(Mention));
                    Assert.IsTrue(reply.Text.Contains((reply.Entities[0] as Mention).Text));
                    Assert.IsTrue((reply.Entities[0] as Mention).Text.Contains("<at>"));
                    Assert.IsTrue((reply.Entities[0] as Mention).Text == "<at>" + (reply.Entities[0] as Mention).Mentioned.Name + "</at>");
                    Assert.IsTrue((reply.Entities[0] as Mention).Text.EndsWith("</at>"));
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// @Mention tests with mention text.
        /// </summary>
        /// <returns>Task tracking operations.</returns>
        [TestMethod]
        public async Task AddMention_WithMentionTextAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity reply = sampleActivity.CreateReply();
                    reply = teamsExtension.AddMentionToText(reply, sampleActivity.From, mentionText: "SampleName");

                    Assert.IsTrue(reply.Entities.Count == 1);
                    Assert.IsInstanceOfType(reply.Entities[0], typeof(Mention));
                    Assert.IsTrue(reply.Text.Contains((reply.Entities[0] as Mention).Text));
                    Assert.IsTrue((reply.Entities[0] as Mention).Text.Contains("SampleName"));
                    Assert.IsTrue((reply.Entities[0] as Mention).Text == "<at>" + (reply.Entities[0] as Mention).Mentioned.Name + "</at>");
                    Assert.IsTrue((reply.Entities[0] as Mention).Text.StartsWith("<at>"));
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// @Mention tests with no mention text and no username. Expects exception.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddMention_WithNoMentionTextAndNoChannelAccountNameAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity reply = sampleActivity.CreateReply();
                    reply = teamsExtension.AddMentionToText(
                        reply, new ChannelAccount
                        {
                            Id = sampleActivity.From.Id,
                        });
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// @Mention tests with entities instantiated to null (new Activity case).
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task AddMention_WithEntitiesAsNullAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity reply = sampleActivity.CreateReply();
                    reply.Entities = null;
                    reply = teamsExtension.AddMentionToText(reply, sampleActivity.From);
                    Assert.IsTrue(reply.Entities.Count == 1);
                    Assert.IsInstanceOfType(reply.Entities[0], typeof(Mention));
                    Assert.IsTrue(reply.Text.Contains((reply.Entities[0] as Mention).Text));
                    Assert.IsTrue((reply.Entities[0] as Mention).Text == "<at>" + (reply.Entities[0] as Mention).Mentioned.Name + "</at>");
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Activity extensions tests for strip mentions with mentions in it.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task RemoveMentions_StripMentionsWithMentionsInItAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivity2AtMentions.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    string noMentionText = teamsExtension.GetActivityTextWithoutMentions();
                    Assert.IsTrue(sampleActivity.Text.Contains(noMentionText));
                    Assert.AreEqual("TestMessage", noMentionText);
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Test to removes mentions from activity with no mentions.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task RemoveMentions_StripMentionsWithNoMentionsAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityNoMentions.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    string noMentionText = teamsExtension.GetActivityTextWithoutMentions();
                    Assert.IsTrue(sampleActivity.Text.Contains(noMentionText));
                    Assert.AreEqual(sampleActivity.Text, noMentionText);
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }
    }
}
