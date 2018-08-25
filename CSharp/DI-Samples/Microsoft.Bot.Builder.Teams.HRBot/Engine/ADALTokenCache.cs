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

        private IStatePropertyAccessor<UserDetails> userDetailsAccessor;

        public ADALTokenCache(ITurnContext turnContext, IStatePropertyAccessor<UserDetails> userDetailsAccessor)
        {
            this.userDetailsAccessor = userDetailsAccessor;
        }

        /// <summary>
        /// Notification raised before ADAL accesses the cache.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <remarks>This is your chance to update the in-memory copy from the cache, if the in-memory version is stale.</remarks>
        protected virtual void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            UserDetails userDetails = this.userDetailsAccessor.GetAsync(this.turnContext).ConfigureAwait(false).GetAwaiter().GetResult();
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

            UserDetails userDetails = this.userDetailsAccessor.GetAsync(this.turnContext).ConfigureAwait(false).GetAwaiter().GetResult();

            if (userDetails == null)
            {
                userDetails = new UserDetails
                {
                    OAuthProviderUserId = args.UniqueId,
                    UserId = this.turnContext.Activity.From.Id,
                };
            }

            userDetails.UserToken = serializedData;
            this.userDetailsAccessor.SetAsync(this.turnContext, userDetails).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
