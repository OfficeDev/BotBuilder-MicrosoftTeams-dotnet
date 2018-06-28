using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Teams.HRBot.Engine
{
    public class OAuthSettings
    {
        public string Resource { get; set; }

        public string ClientId { get; set; }

        public Uri RedirectUri { get; set; } 
    }
}
