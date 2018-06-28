using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Bot.Builder.Teams.HRBot.Engine
{
    public class MessageActivityHandler : IMessageActivityHandler
    {
        private readonly OAuthSettings oAuthSettings;

        public MessageActivityHandler(IOptions<OAuthSettings> oAuthSettings)
        {
            this.oAuthSettings = oAuthSettings.Value;
        }

        public async Task HandleMessageAsync(ITurnContext turnContext)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/common", false, new ADALTokenCache(turnContext));
            UserDetails userDetails = ConversationState<UserDetails>.Get(turnContext);
            if (userDetails.UserId == null || userDetails.UserToken == null)
            {
                Uri authUrl = await authenticationContext.GetAuthorizationRequestUrlAsync(
                    this.oAuthSettings.Resource,
                    this.oAuthSettings.ClientId,
                    this.oAuthSettings.RedirectUri, 
                    UserIdentifier.AnyUser, 
                    null);

                SigninCard card = SigninCard.Create("Login to Graph", "Login", authUrl.AbsoluteUri);

                Activity replyActivity = turnContext.Activity.CreateReply();
                replyActivity.Attachments = new List<Attachment>();
                Attachment plAttachment = card.ToAttachment();
                replyActivity.Attachments.Add(plAttachment);
                replyActivity.Type = ActivityTypes.Message;
                await turnContext.SendActivity(replyActivity);
            }
        }
    }
}
