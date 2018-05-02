// <copyright file="FetchChannelListTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Connector.Teams;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Teams Fetch Channel list tests.
    /// </summary>
    [TestClass]
    public class FetchChannelListTests
    {
        /// <summary>
        /// Teams API test for fetching channel list.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListTestAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            ConversationList conversationList = new ConversationList
            {
                Conversations = new List<ChannelInfo>
                {
                    new ChannelInfo
                    {
                        Id = "ChannelId",
                        Name = "ChannelName",
                    },
                },
            };

            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(conversationList));
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = stringContent;
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);

            ConversationList conversationListResponse = await teamsConnectorClient.Teams.FetchChannelListAsync("TestTeamId").ConfigureAwait(false);

            Assert.IsNotNull(conversationListResponse);
            Assert.IsNotNull(conversationListResponse.Conversations);
            Assert.AreEqual(conversationListResponse.Conversations.Count, 1);
            Assert.AreEqual(conversationListResponse.Conversations[0].Id, conversationList.Conversations[0].Id);
            Assert.AreEqual(conversationListResponse.Conversations[0].Name, conversationList.Conversations[0].Name);
        }

        /// <summary>
        /// Teams API test for fetching channel list async with advanced options.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListAsyncWithHttpMessagesTestAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            ConversationList conversationList = new ConversationList
            {
                Conversations = new List<ChannelInfo>
                {
                    new ChannelInfo
                    {
                        Id = "ChannelId",
                        Name = "ChannelName",
                    },
                },
            };

            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(conversationList));
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = stringContent;
                Assert.IsNotNull(request.Headers.GetValues("Authorization"));
                Assert.AreEqual(request.Headers.GetValues("Authorization").Count(), 1);
                Assert.AreEqual(request.Headers.GetValues("Authorization").ToList()[0], "CustomValue");
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);

            ConversationList conversationListResponse = (await teamsConnectorClient.Teams.FetchChannelListWithHttpMessagesAsync(
                    "TestTeamId",
                    new Dictionary<string, List<string>>() { { "Authorization", new List<string>() { "CustomValue" } } }).ConfigureAwait(false)).Body;

            Assert.IsNotNull(conversationListResponse);
            Assert.IsNotNull(conversationListResponse.Conversations);
            Assert.AreEqual(conversationListResponse.Conversations.Count, 1);
            Assert.AreEqual(conversationListResponse.Conversations[0].Id, conversationList.Conversations[0].Id);
            Assert.AreEqual(conversationListResponse.Conversations[0].Name, conversationList.Conversations[0].Name);
        }

        /// <summary>
        /// Teams API test for fetch channel list with invalid http code in response.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(HttpOperationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListTestInvalidHttpCodeAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent("RandomRandomRandom");
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = stringContent;
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);
            ConversationList conversationListResponse = await teamsConnectorClient.Teams.FetchChannelListAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch channel list with invalid http code and no response body.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(HttpOperationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListTestInvalidHttpCodeWithoutResponseContentAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);
            ConversationList conversationListResponse = await teamsConnectorClient.Teams.FetchChannelListAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch channel list with invalid http code in response and response body.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(SerializationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListTestInvalidResponseAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent("RandomRandomRandom");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = stringContent;
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);
            ConversationList conversationListResponse = await teamsConnectorClient.Teams.FetchChannelListAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch channel list with invalid team Id.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ValidationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchChannelListTestInvalidTeamIdAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent("RandomRandomRandom");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = stringContent;
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);
            ConversationList conversationListResponse = await teamsConnectorClient.Teams.FetchChannelListAsync(null).ConfigureAwait(false);
        }
    }
}
