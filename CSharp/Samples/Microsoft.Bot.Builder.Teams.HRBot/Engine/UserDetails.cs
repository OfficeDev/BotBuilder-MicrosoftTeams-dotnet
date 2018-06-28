using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Teams.HRBot.Engine
{
    public class UserDetails
    {
        public string UserId { get; set; }

        public string OAuthProviderUserId { get; set; }

        public byte[] UserToken { get;set; }
    }
}
