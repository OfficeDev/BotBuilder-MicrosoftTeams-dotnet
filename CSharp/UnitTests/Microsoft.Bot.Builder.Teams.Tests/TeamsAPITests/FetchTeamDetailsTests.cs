// <copyright file="FetchTeamDetailsTests.cs" company="Microsoft">
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// Fetch team details tests.
    /// </summary>
    public class FetchTeamDetailsTests
    {
        /// <summary>
        /// Teams API test for fetching team details.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TeamDetails teamDetails = new TeamDetails
            {
                Id = "TeamId",
                AadGroupId = "GroupId",
                Name = "TeamName",
            };

            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(teamDetails));
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = stringContent;
                return Task.FromResult(response);
            });

            TeamsConnectorClient teamsConnectorClient = new TeamsConnectorClient(
                new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"),
                new MicrosoftAppCredentials("Test", "Test"),
                testHandler);

            TeamDetails teamDetailsResult = await teamsConnectorClient.Teams.FetchTeamDetailsAsync("TestTeamId").ConfigureAwait(false);

            Assert.IsNotNull(teamDetailsResult);
            Assert.IsNotNull(teamDetailsResult.Id);
            Assert.IsNotNull(teamDetailsResult.Name);
            Assert.IsNotNull(teamDetailsResult.AadGroupId);
            Assert.AreEqual(teamDetailsResult.Id, teamDetails.Id);
            Assert.AreEqual(teamDetailsResult.Name, teamDetails.Name);
            Assert.AreEqual(teamDetailsResult.AadGroupId, teamDetails.AadGroupId);
        }

        /// <summary>
        /// Teams API test for fetching team details with advanced options.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsAsyncWithHttpMessagesTestAsync()
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = true;
            TeamDetails teamDetails = new TeamDetails
            {
                Id = "TeamId",
                AadGroupId = "GroupId",
                Name = "TeamName",
            };

            TestDelegatingHandler testHandler = new TestDelegatingHandler((request) =>
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(teamDetails));
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

            TeamDetails teamDetailsResult = (await teamsConnectorClient.Teams.FetchTeamDetailsWithHttpMessagesAsync(
                    "TestTeamId",
                    new Dictionary<string, List<string>>() { { "Authorization", new List<string>() { "CustomValue" } } }).ConfigureAwait(false)).Body;

            Assert.IsNotNull(teamDetailsResult);
            Assert.IsNotNull(teamDetailsResult.Id);
            Assert.IsNotNull(teamDetailsResult.Name);
            Assert.IsNotNull(teamDetailsResult.AadGroupId);
            Assert.AreEqual(teamDetailsResult.Id, teamDetails.Id);
            Assert.AreEqual(teamDetailsResult.Name, teamDetails.Name);
            Assert.AreEqual(teamDetailsResult.AadGroupId, teamDetails.AadGroupId);
        }

        /// <summary>
        /// Teams API test for fetch team details with invalid http code in response.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(HttpOperationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsTestInvalidHttpCodeAsync()
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
            await teamsConnectorClient.Teams.FetchTeamDetailsAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch team details with invalid http code and no response body.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(HttpOperationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsTestInvalidHttpCodeWithoutResponseContentAsync()
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
            await teamsConnectorClient.Teams.FetchTeamDetailsAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch team details with invalid http code in response and response body.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(SerializationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsTestInvalidResponseAsync()
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
            await teamsConnectorClient.Teams.FetchTeamDetailsAsync("TestTeamId").ConfigureAwait(false);
        }

        /// <summary>
        /// Teams API test for fetch team details with invalid team Id.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [ExpectedException(typeof(ValidationException))]
        [TestMethod]
        public async Task TeamsAPI_FetchTeamDetailsTestInvalidTeamIdAsync()
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
            await teamsConnectorClient.Teams.FetchTeamDetailsAsync(null).ConfigureAwait(false);
        }
    }
}
