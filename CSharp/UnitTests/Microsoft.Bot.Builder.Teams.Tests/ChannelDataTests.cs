// <copyright file="ChannelDataTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Teams channel data tests.
    /// </summary>
    [TestClass]
    public class ChannelDataTests
    {
        /// <summary>
        /// Channel data test to get general channel.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ChannelData_GetGeneralChannelAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    ChannelInfo generalChannel = teamsExtension.GetGeneralChannel();

                    TeamsChannelData channelData = sampleActivity.GetChannelData<TeamsChannelData>();

                    Assert.IsNotNull(generalChannel);
                    Assert.IsNotNull(generalChannel.Id);
                    Assert.IsTrue(generalChannel.Id == channelData.Team.Id);

                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Channel data test to get general channel while channel data is missing.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public async Task ChannelData_GetGeneralChannelNoChannelDataAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            sampleActivity.ChannelData = null;
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    ChannelInfo generalChannel = teamsExtension.GetGeneralChannel();
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Channel data test to get general channel with invalid channel data.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public async Task ChannelData_GetGeneralChannelInvalidChannelDataAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            TeamsChannelData channelData = JsonConvert.DeserializeObject<TeamsChannelData>(sampleActivity.ChannelData.ToString());
            channelData.Team = null;
            sampleActivity.ChannelData = JObject.FromObject(channelData);
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    ChannelInfo generalChannel = teamsExtension.GetGeneralChannel();
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Channel data test to check properties.
        /// </summary>
        [TestMethod]
        public void ChannelData_PropertyCheck()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            TeamsChannelData channelData = JsonConvert.DeserializeObject<TeamsChannelData>(sampleActivity.ChannelData.ToString());
            Assert.IsNotNull(channelData);
            Assert.IsNotNull(channelData.Channel);
            Assert.IsNotNull(channelData.Channel.Id);
            Assert.IsNotNull(channelData.Team);
            Assert.IsNotNull(channelData.Team.Id);
            Assert.IsNotNull(channelData.Tenant);
            Assert.IsNotNull(channelData.Tenant.Id);
        }

        /// <summary>
        /// Channel data test to get tenant Id.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ChannelData_GetTenantIdAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Assert.IsNotNull(teamsExtension.GetActivityTenantId());
                    Assert.AreEqual(teamsExtension.GetActivityTenantId(), "3b9e9fbb-ed2f-415b-b776-cf788e573366");
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Channel data test to get tenant Id with missing channel data.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public async Task ChannelData_GetTenantIdMissingChannelDataAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            sampleActivity.ChannelData = null;
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    teamsExtension.GetActivityTenantId();
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Channel data test to get tenant Id with missing tenant Id.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public async Task ChannelData_GetTenantIdMissingTenantDataAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));
            TeamsChannelData channelData = JsonConvert.DeserializeObject<TeamsChannelData>(sampleActivity.ChannelData.ToString());
            channelData.Tenant = null;
            sampleActivity.ChannelData = JObject.FromObject(channelData);
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    teamsExtension.GetActivityTenantId();
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Add notification tests.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ChannelData_AddNotificationAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity replyActivity = JObject.FromObject(sampleActivity).ToObject<Activity>();
                    Activity modifiedActivity = teamsExtension.NotifyUser(replyActivity);

                    Assert.IsNotNull(modifiedActivity.ChannelData);
                    Assert.IsNotNull(modifiedActivity.GetChannelData<TeamsChannelData>().Notification);
                    Assert.IsTrue(modifiedActivity.GetChannelData<TeamsChannelData>().Notification.Alert.Value);
                    Assert.IsNotNull(modifiedActivity.GetChannelData<TeamsChannelData>().Team.Id);

                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Add notification when no at mentions are present.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ChannelData_AddNotification_NoMentionsAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityNoMentions.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Activity replyActivity = JObject.FromObject(sampleActivity).ToObject<Activity>();
                    Activity modifiedActivity = teamsExtension.NotifyUser(replyActivity);

                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }
    }
}
