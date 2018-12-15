// <copyright file="TeamsExtension.IncomingActivity.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Teams extension methods which operate on incoming activity.
    /// </summary>
    public partial class TeamsContext
    {
        /// <summary>
        /// Gets the general channel for a team.
        /// </summary>
        /// <returns>Channel data for general channel.</returns>
        /// <exception cref="ArgumentException">Failed to process channel data in Activity.</exception>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity.</exception>
        public ChannelInfo GetGeneralChannel()
        {
            if (this.turnContext.Activity.ChannelData != null)
            {
                TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();

                if (channelData != null && channelData.Team != null)
                {
                    return new ChannelInfo
                    {
                        Id = channelData.Team.Id,
                    };
                }

                throw new ArgumentException("Failed to process channel data in Activity. ChannelData is missing Team property.");
            }
            else
            {
                throw new ArgumentException("ChannelData missing in Activity");
            }
        }

        /// <summary>
        /// Creates a reply for the General channel of the team.
        /// </summary>
        /// <param name="text">Reply text.</param>
        /// <param name="locale">Locale information.</param>
        /// <returns>New reply activity with General channel channel data.</returns>
        public Activity CreateReplyToGeneralChannel(string text = null, string locale = null)
        {
            TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();
            Activity replyActivity = this.turnContext.Activity.CreateReply(text, locale);

            replyActivity.ChannelData = new TeamsChannelData
            {
                Channel = this.GetGeneralChannel(),
                Team = channelData.Team,
                Tenant = channelData.Tenant,
            }.AsJObject();

            return replyActivity;
        }

        /// <summary>
        /// Gets the tenant id of the user who sent the message.
        /// </summary>
        /// <returns>Tenant Id of the user who sent the message.</returns>
        /// <exception cref="ArgumentException">Failed to process channel data in Activity.</exception>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity.</exception>
        public string GetActivityTenantId()
        {
            if (this.turnContext.Activity.ChannelData != null)
            {
                TeamsChannelData channelData = this.turnContext.Activity.GetChannelData<TeamsChannelData>();

                if (!string.IsNullOrEmpty(channelData?.Tenant?.Id))
                {
                    return channelData.Tenant.Id;
                }

                throw new ArgumentException("Failed to process channel data in Activity");
            }
            else
            {
                throw new ArgumentNullException("ChannelData missing in Activity");
            }
        }

        /// <summary>
        /// Gets the activity text without mentions.
        /// </summary>
        /// <returns>Text without mentions.</returns>
        public string GetActivityTextWithoutMentions()
        {
            Activity activity = this.turnContext.Activity;

            // Case 1. No entities.
            if (activity.Entities?.Count == 0)
            {
                return activity.Text;
            }

            IEnumerable<Entity> mentionEntities = activity.Entities.Where(entity => entity.Type.Equals("mention", StringComparison.OrdinalIgnoreCase));

            // Case 2. No Mention entities.
            if (!mentionEntities.Any())
            {
                return activity.Text;
            }

            // Case 3. Mention entities.
            string strippedText = activity.Text;

            mentionEntities.ToList()
                .ForEach(entity =>
                {
                    strippedText = strippedText.Replace(entity.GetAs<Mention>().Text, string.Empty);
                });

            return strippedText.Trim();
        }

        /// <summary>
        /// Checks if the request is a O365 connector card action query.
        /// </summary>
        /// <returns>True is activity is a actionable card query, false otherwise.</returns>
        public bool IsRequestO365ConnectorCardActionQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.StartsWith("actionableMessage/executeAction", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets O365 connector card action query data.
        /// </summary>
        /// <returns>O365 connector card action query data.</returns>
        public O365ConnectorCardActionQuery GetO365ConnectorCardActionQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<O365ConnectorCardActionQuery>();
        }

        /// <summary>
        /// Checks if the request is a signin state verification query.
        /// </summary>
        /// <returns>True is activity is a signin state verification query, false otherwise.</returns>
        public bool IsRequestSigninStateVerificationQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.StartsWith("signin/verifyState", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets signin state verification query data.
        /// </summary>
        /// <returns>Signin state verification query data.</returns>
        public SigninStateVerificationQuery GetSigninStateVerificationQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<SigninStateVerificationQuery>();
        }

        /// <summary>
        /// Checks if the activity is a messaging extension query.
        /// </summary>
        /// <returns>True is activity is a messaging extension query, false otherwise.</returns>
        public bool IsRequestMessagingExtensionQuery()
        {
            return this.turnContext.Activity.Type == ActivityTypes.Invoke &&
                !string.IsNullOrEmpty(this.turnContext.Activity.Name) &&
                this.turnContext.Activity.Name.StartsWith("composeExtension", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the messaging extension query data.
        /// </summary>
        /// <returns>Messaging extension query data.</returns>
        public MessagingExtensionQuery GetMessagingExtensionQueryData()
        {
            return this.turnContext.Activity.Value.AsJObject().ToObject<MessagingExtensionQuery>();
        }

        /// <summary>
        /// Gets the conversation parameters for create or get direct conversation.
        /// </summary>
        /// <param name="user">The user to create conversation with.</param>
        /// <returns>Conversation parameters to get or create direct conversation (1on1) between bot and user.</returns>
        public ConversationParameters GetConversationParametersForCreateOrGetDirectConversation(ChannelAccount user)
        {
            return new ConversationParameters()
            {
                Bot = this.turnContext.Activity.Recipient,
                ChannelData = JObject.FromObject(
                    new TeamsChannelData
                    {
                        Tenant = new TenantInfo
                        {
                            Id = this.GetActivityTenantId(),
                        },
                    },
                    JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    })),
                Members = new List<ChannelAccount>() { user },
            };
        }
    }
}
