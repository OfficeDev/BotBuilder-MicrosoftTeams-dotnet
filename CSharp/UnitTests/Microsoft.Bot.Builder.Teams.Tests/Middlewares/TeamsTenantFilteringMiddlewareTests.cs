// <copyright file="TeamsTenantFilteringMiddlewareTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests.Middleware
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Teams.Middlewares;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Tests for the <see cref="TeamsTenantFilteringMiddleware"/> class.
    /// </summary>
    [TestClass]
    public class TeamsTenantFilteringMiddlewareTests
    {
        /// <summary>
        /// Check that the <see cref="TeamsTenantFilteringMiddleware(System.Collections.Generic.IEnumerable{string})"/> constructor
        /// handles null arguments appropriately.
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Constructor_Nulls()
        {
            var middleware = new TeamsTenantFilteringMiddleware(null);
        }

        /// <summary>
        /// Check that OnTurnAsync does not call next if allows tenant list is empty on any tenant.
        /// </summary>
        /// <returns>A task.</returns>
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public async Task OnTurnAsync_EmptyAllowedList_WithTenantIdAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(true, "TenantId");
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[0]);
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsFalse(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        /// <summary>
        /// Check that OnTurnAsync does not call next if allows tenant list is empty on any tenant.
        /// </summary>
        /// <returns>A task.</returns>
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public async Task OnTurnAsync_EmptyAllowedList_WithoutTenantIdAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(true, null);
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[0]);
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsFalse(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        /// <summary>
        /// Check that OnTurnAsync calls next for tenant on allow list.
        /// </summary>
        /// <returns>A task.</returns>
        [TestMethod]
        public async Task OnTurnAsync_PopulatedAllowList_AllowedTenantAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(true, "TenantId");
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[] { "TenantId" });
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsTrue(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        /// <summary>
        /// Check that OnTurnAsync does not call next for tenant not on allow list.
        /// </summary>
        /// <returns>A task.</returns>
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public async Task OnTurnAsync_PopulatedAllowList_NotAllowedTenantAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(true, "ADifferentTenantId");
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[] { "TenantId" });
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsFalse(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        /// <summary>
        /// Check that OnTurnAsync does not call next for tenant not on allow list.
        /// </summary>
        /// <returns>A task.</returns>
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public async Task OnTurnAsync_PopulatedAllowList_NoTenantAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(true, null);
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[] { "TenantId" });
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsFalse(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        /// <summary>
        /// Check that OnTurnAsync does call next for other channels.
        /// </summary>
        /// <returns>A task.</returns>
        [TestMethod]
        public async Task OnTurnAsync_PopulatedAllowList_NotAllowedTenant_NotTeamsChannelAsync()
        {
            // Setup
            var turnContextMock = new Mock<ITurnContext>();
            var activity = this.TeamsActivityWithTenantId(false, "ADifferentTenantId");
            turnContextMock.Setup((turnContext) => turnContext.Activity).Returns(activity);
            var middleware = new TeamsTenantFilteringMiddleware(new string[] { "TenantId" });
            bool isDelegateCalled = false;
            Task Next(CancellationToken cancellationToken) => Task.FromResult(isDelegateCalled = true);

            // Action
            await middleware.OnTurnAsync(turnContextMock.Object, Next).ConfigureAwait(false);

            // Verify
            Assert.IsTrue(isDelegateCalled); // This doesn't actually get called since we throw an exception.
        }

        private Activity TeamsActivityWithTenantId(bool teamsChannel, string tenantId)
        {
            return new Activity()
            {
                ChannelId = teamsChannel ? Channels.Msteams : Channels.Skype,
                ChannelData = new TeamsChannelData
                {
                    Tenant = tenantId == null ? null : new TenantInfo
                    {
                        Id = tenantId,
                    },
                },
            };
        }
    }
}
