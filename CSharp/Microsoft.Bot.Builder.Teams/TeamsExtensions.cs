using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector.Teams;

namespace Microsoft.Bot.Builder.Teams
{
    internal class TeamsExtensions
    {
        private readonly ITurnContext turnContext;

        private readonly ITeamsConnectorClient teamsConnectorClient;

        internal TeamsExtensions(ITurnContext turnContext, ITeamsConnectorClient teamsConnectorClient)
        {

        }
    }
}
