// <copyright file="ConnectorExtensionTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// Connector extension tests.
    /// </summary>
    [TestClass]
    public class ConnectorExtensionTests
    {
        /// <summary>
        /// Connector extensions test for creating 1 on 1 conversation between bot and user.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ConnectorExtensions_Create1on1Async()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    ConversationParameters conversationParameters = teamsExtension.GetConversationParametersForCreateOrGetDirectConversation(new ChannelAccount
                    {
                        Id = "UserId",
                        Name = "UserName",
                    });

                    Assert.IsNotNull(conversationParameters.Bot);
                    Assert.IsNotNull(conversationParameters.Members);
                    Assert.AreEqual(1, conversationParameters.Members.Count);
                    Assert.AreEqual(sampleActivity.Recipient.Id, conversationParameters.Bot.Id);
                    Assert.AreEqual("UserId", conversationParameters.Members[0].Id);
                    Assert.AreEqual(
                        sampleActivity.GetChannelData<TeamsChannelData>().Tenant.Id,
                        conversationParameters.ChannelData.AsJObject().ToObject<TeamsChannelData>().Tenant.Id);

                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests resolution of ChannelAccount to TeamsChannelAccount.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ConnectorExtensions_ResolveAsTeamsChannelAccountAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityAtMention.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                async (teamsExtension) =>
                {
                    TestDelegatingHandler testDelegatingHandler = new TestDelegatingHandler((request) =>
                    {
                        Assert.IsFalse(request.Headers.Contains("X-MsTeamsTenantId"));

                        StringContent stringContent = new StringContent(File.ReadAllText(@"Jsons\SampleResponseGetTeamsConversationMembers.json"));
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = stringContent;
                        return Task.FromResult(response);
                    });

                    ConnectorClient conClient = new ConnectorClient(new Uri("https://testservice.com"), "Test", "Test", testDelegatingHandler);

                    var memberList = (await conClient.Conversations.GetConversationMembersAsync("TestConversationId").ConfigureAwait(false))
                        .ToList()
                        .ConvertAll(member =>
                        {
                            return teamsExtension.AsTeamsChannelAccount(member);
                        });

                    Assert.IsTrue(memberList.Count() == 2);
                    Assert.IsFalse(memberList.Any(member => string.IsNullOrEmpty(member.AadObjectId)));
                    Assert.IsFalse(memberList.Any(member => string.IsNullOrEmpty(member.Name)));
                    Assert.IsFalse(memberList.Any(member => string.IsNullOrEmpty(member.UserPrincipalName)));
                    Assert.IsFalse(memberList.Any(member => string.IsNullOrEmpty(member.Id)));
                    Assert.IsFalse(memberList.Any(member => string.IsNullOrEmpty(member.Email)));
                }).ConfigureAwait(false);
        }
    }
}
