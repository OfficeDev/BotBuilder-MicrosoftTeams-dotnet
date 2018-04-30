// <copyright file="TeamsExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Connector.Teams;

    /// <summary>
    /// Teams extension class.
    /// </summary>
    internal partial class TeamsExtensions
    {
        /// <summary>
        /// Turn context created by adapter and sent over through middlewares.
        /// </summary>
        private readonly ITurnContext turnContext;

        /// <summary>
        /// Teams connector client instance. This is used to make calls to BotFramework APIs which are only supported by MsTeams.
        /// </summary>
        private readonly ITeamsConnectorClient teamsConnectorClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsExtensions"/> class.
        /// </summary>
        /// <param name="turnContext">Turn context created by adapter and sent over through middlewares.</param>
        /// <param name="teamsConnectorClient">Teams connector client instance.</param>
        internal TeamsExtensions(ITurnContext turnContext, ITeamsConnectorClient teamsConnectorClient)
        {
            this.turnContext = turnContext;
            this.teamsConnectorClient = teamsConnectorClient;
        }
    }
}
