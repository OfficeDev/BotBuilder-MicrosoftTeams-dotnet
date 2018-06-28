using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Bot.Builder.Teams.HRBot.Engine
{
    public class ADALTokenCache : TokenCache
    {
        private ITurnContext turnContext;

        public ADALTokenCache(ITurnContext turnContext)
        {
            this.turnContext = turnContext;
        }

        /// <summary>
        /// Notification raised before ADAL accesses the cache.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <remarks>This is your chance to update the in-memory copy from the cache, if the in-memory version is stale</remarks>
        protected virtual void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            UserDetails userDetails = ConversationState<UserDetails>.Get(this.turnContext);
            this.Deserialize(userDetails.UserToken);
        }

        /// <summary>
        /// Notification raised after ADAL accessed the cache.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <remarks> If the HasStateChanged flag is set, ADAL changed the content of the cache. </remarks>
        protected virtual void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            byte[] serializedData = this.Serialize();
            UserDetails userDetails = ConversationState<UserDetails>.Get(this.turnContext);
            userDetails.UserToken = serializedData;
        }
    }
}
