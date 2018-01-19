// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Bot Framework: http://botframework.com
// Microsoft Teams: https://dev.office.com/microsoft-teams
//
// Bot Builder SDK GitHub:
// https://github.com/Microsoft/BotBuilder
//
// Bot Builder SDK Extensions for Teams
// https://github.com/OfficeDev/BotBuilder-MicrosoftTeams
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Bot.Connector.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Builder;
    using Models;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Location at which AtMention should be added in text.
    /// </summary>
    public enum MentionTextLocation
    {
        /// <summary>
        /// Adds mention to start of text. Note this edits Text property.
        /// </summary>
        PrependText,

        /// <summary>
        /// Adds mention to end of text. Note this edits Text property.
        /// </summary>
        AppendText
    }

    /// <summary>
    /// Activity extensions.
    /// </summary>
    public static class ActivityExtensions
    {
        /// <summary>
        /// The members added event name.
        /// </summary>
        private const string MembersAddedEventName = "teamMemberAdded";

        /// <summary>
        /// The members removed event name.
        /// </summary>
        private const string MembersRemovedEventName = "teamMemberRemoved";

        /// <summary>
        /// The channel created event name.
        /// </summary>
        private const string ChannelCreatedEventName = "channelCreated";

        /// <summary>
        /// The channel deleted event name.
        /// </summary>
        private const string ChannelDeletedEventName = "channelDeleted";

        /// <summary>
        /// The channel renamed event name.
        /// </summary>
        private const string ChannelRenamedEventName = "channelRenamed";

        /// <summary>
        /// The team renamed event name.
        /// </summary>
        private const string TeamRenamedEventName = "teamRenamed";

        /// <summary>
        /// Adds the mention text to the response.
        /// </summary>
        /// <param name="botContext">Bot context.</param>
        /// <param name="mentionedUser">The mentioned user.</param>
        /// <param name="textLocation">Location at which AtMention text should be added to text.</param>
        /// <param name="mentionText">The mention text.</param>
        /// <exception cref="ArgumentException">Either mentioned user name or mentionText must have a value</exception>
        public static void AddAtMention(
            this IBotContext botContext,
            ChannelAccount mentionedUser,
            MentionTextLocation textLocation = MentionTextLocation.PrependText,
            string mentionText = null)
        {
            botContext.EnsureResponseExists();

            IMessageActivity messageActivity = botContext.Responses[0] as IMessageActivity;

            if (messageActivity == null)
            {
                throw new ArgumentException("Method can only be called on message activities", nameof(botContext.Responses));
            }

            if (mentionedUser == null || string.IsNullOrEmpty(mentionedUser.Id))
            {
                throw new ArgumentNullException("mentionedUser", "Mentioned user and user ID cannot be null");
            }

            if (string.IsNullOrEmpty(mentionedUser.Name) && string.IsNullOrEmpty(mentionText))
            {
                throw new ArgumentException("Either mentioned user name or mentionText must have a value");
            }

            if (!string.IsNullOrWhiteSpace(mentionText))
            {
                mentionedUser.Name = mentionText;
            }

            string mentionEntityText = string.Format("<at>{0}</at>", mentionedUser.Name);

            if (textLocation == MentionTextLocation.AppendText)
            {
                messageActivity.Text = messageActivity.Text + " " + mentionEntityText;
            }
            else
            {
                messageActivity.Text = mentionEntityText + " " + messageActivity.Text;
            }

            if (messageActivity.Entities == null)
            {
                messageActivity.Entities = new List<Connector.Entity>();
            }

            messageActivity.Entities.Add(new Mention()
            {
                Text = mentionEntityText,
                Mentioned = mentionedUser
            });
        }

        /// <summary>
        /// Notifies the user in direct conversation.
        /// </summary>
        /// <param name="botContext">Active bot context.</param>
        public static void NotifyUser<T>(this IBotContext botContext)
        {
            botContext.EnsureResponseExists();

            foreach (IActivity replyActivity in botContext.Responses)
            {
                TeamsChannelData channelData = replyActivity.ChannelData as TeamsChannelData;
                channelData.Notification = new NotificationInfo
                {
                    Alert = true
                };

                replyActivity.ChannelData = JObject.FromObject(channelData);
            }
        }

        /// <summary>Gets the conversation update data.</summary>
        /// <param name="activity">The activity.</param>
        /// <returns>Channel event data.</returns>
        /// <exception cref="Exception">
        /// Failed to process channel data in Activity
        /// or
        /// ChannelData missing in Activity
        /// </exception>
        public static TeamEventBase GetConversationUpdateData(this IConversationUpdateActivity activity)
        {
            if (activity.GetActivityType() != ActivityTypes.ConversationUpdate)
            {
                throw new ArgumentException("activity must be a ConversationUpdate");
            }

            if (activity.ChannelData != null)
            {
                TeamsChannelData channelData = activity.ChannelData as TeamsChannelData;

                if (!string.IsNullOrEmpty(channelData?.EventType))
                {
                    switch (channelData.EventType)
                    {
                        case MembersAddedEventName:
                            return new MembersAddedEvent
                            {
                                MembersAdded = activity.MembersAdded,
                                Team = channelData.Team,
                                Tenant = channelData.Tenant
                            };
                        case MembersRemovedEventName:
                            return new MembersRemovedEvent
                            {
                                MembersRemoved = activity.MembersRemoved,
                                Team = channelData.Team,
                                Tenant = channelData.Tenant
                            };
                        case ChannelCreatedEventName:
                            return new ChannelCreatedEvent
                            {
                                Channel = channelData.Channel,
                                Team = channelData.Team,
                                Tenant = channelData.Tenant
                            };
                        case ChannelDeletedEventName:
                            return new ChannelDeletedEvent
                            {
                                Channel = channelData.Channel,
                                Team = channelData.Team,
                                Tenant = channelData.Tenant
                            };
                        case ChannelRenamedEventName:
                            return new ChannelRenamedEvent
                            {
                                Channel = channelData.Channel,
                                Team = channelData.Team,
                                Tenant = channelData.Tenant
                            };
                        case TeamRenamedEventName:
                            return new TeamRenamedEvent
                            {
                                Tenant = channelData.Tenant,
                                Team = channelData.Team
                            };
                    }
                }

                throw new ArgumentException("Failed to process channel data in Activity");
            }
            else
            {
                throw new ArgumentNullException("Activity.ChannelData", "ChannelData missing in Activity");
            }
        }

        /// <summary>
        /// Gets the general channel for a team.
        /// </summary>
        /// <param name="botContext">Bot request context.</param>
        /// <returns>Channel data for general channel.</returns>
        /// <exception cref="ArgumentException">Failed to process channel data in Activity</exception>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity</exception>
        public static ChannelInfo GetGeneralChannel(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            IActivity activity = botContext.Request;

            if (activity.ChannelData != null)
            {
                TeamsChannelData channelData = activity.ChannelData as TeamsChannelData;

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
        /// <param name="botContext">Incoming request.</param>
        /// <param name="text">Reply text.</param>
        /// <param name="locale">Locale information.</param>
        public static void CreateReplyToGeneralChannel(this IBotContext botContext, string text = null, string locale = null)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            TeamsChannelData channelData = botContext.Request.ChannelData as TeamsChannelData;
            if (botContext.Responses == null)
            {
                botContext.Responses = new List<IActivity>();
            }

            var replyActivity = (botContext.Request as Activity).CreateReply(text, locale);

            replyActivity.ChannelData = JObject.FromObject(new TeamsChannelData
            {
                Channel = botContext.GetGeneralChannel(),
                Team = channelData.Team,
                Tenant = channelData.Tenant
            });

            botContext.Responses.Add(replyActivity);
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <param name="botContext">The activity.</param>
        /// <returns>Tenant Id of the user who send the message.</returns>
        /// <exception cref="ArgumentException">Failed to process channel data in Activity</exception>
        /// <exception cref="ArgumentNullException">ChannelData missing in Activity</exception>
        public static string GetTenantId(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            return (botContext.Request.ChannelData as TeamsChannelData)?.Tenant?.Id;
        }

        /// <summary>
        /// Gets the activity text without mentions.
        /// </summary>
        /// <param name="botContext">Bot request context.</param>
        /// <returns>Text without mentions.</returns>
        public static string GetTextWithoutMentions(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            IMessageActivity messageActivity = botContext.Request.AsMessageActivity();

            if (messageActivity == null)
            {
                throw new ArgumentException("Method can only be called on Message activities", nameof(botContext.Request));
            }

            // Case 1. No entities.
            if (messageActivity.Entities?.Count == 0)
            {
                return messageActivity.Text;
            }

            var mentionEntities = messageActivity.Entities.Where(entity => entity.Type.Equals("mention", StringComparison.OrdinalIgnoreCase));

            // Case 2. No Mention entities.
            if (!mentionEntities.Any())
            {
                return messageActivity.Text;
            }

            // Case 3. Mention entities.
            string strippedText = messageActivity.Text;

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
        /// <param name="botContext">Incoming request.</param>
        /// <returns>True is activity is a actionable card query, false otherwise.</returns>
        public static bool IsO365ConnectorCardActionQuery(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            var activity = botContext.Request.AsInvokeActivity();

            if (activity == null)
            {
                throw new ArgumentException("Method is only allowed in Invoke activities", nameof(botContext.Request));
            }

            return !string.IsNullOrEmpty(activity.Name) &&
                activity.Name.StartsWith("actionableMessage/executeAction", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets O365 connector card action query data.
        /// </summary>
        /// <param name="botContext">The incoming request.</param>
        /// <returns>O365 connector card action query data.</returns>
        public static O365ConnectorCardActionQuery GetO365ConnectorCardActionQueryData(this IBotContext botContext)
        {
            var activity = botContext.Request.AsInvokeActivity();

            return JObject.FromObject(activity.Value).ToObject<O365ConnectorCardActionQuery>();
        }

        /// <summary>
        /// Checks if the activity is a signin state verification query.
        /// </summary>
        /// <param name="botContext">Incoming request.</param>
        /// <returns>True is activity is a signin state verification query, false otherwise.</returns>
        public static bool IsSigninStateVerificationQuery(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            var activity = botContext.Request.AsInvokeActivity();

            if (activity == null)
            {
                throw new ArgumentException("Method is only allowed in Invoke activities", nameof(botContext.Request));
            }

            return !string.IsNullOrEmpty(activity.Name) &&
                activity.Name.StartsWith("signin/verifyState", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets signin state verification query data.
        /// </summary>
        /// <param name="botContext">The incoming request.</param>
        /// <returns>Signin state verification query data.</returns>
        public static SigninStateVerificationQuery GetSigninStateVerificationQueryData(this IBotContext botContext)
        {
            var activity = botContext.Request.AsInvokeActivity();

            return JObject.FromObject(activity.Value).ToObject<SigninStateVerificationQuery>();
        }

        /// <summary>
        /// Checks if the activity is a compose extension query.
        /// </summary>
        /// <param name="botContext">Incoming request.</param>
        /// <returns>True is activity is a compose extension query, false otherwise.</returns>
        public static bool IsComposeExtensionQuery(this IBotContext botContext)
        {
            botContext.EnsureTeamsMiddlewareEnabled();

            var activity = botContext.Request.AsInvokeActivity();

            if (activity == null)
            {
                throw new ArgumentException("Method is only allowed in Invoke activities", nameof(botContext.Request));
            }

            return !string.IsNullOrEmpty(activity.Name) &&
                activity.Name.StartsWith("composeExtension", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the compose extension query data.
        /// </summary>
        /// <param name="botContext">The incoming request.</param>
        /// <returns>Compose extension query data.</returns>
        public static ComposeExtensionQuery GetComposeExtensionQueryData(this IBotContext botContext)
        {
            var activity = botContext.Request.AsInvokeActivity();

            return JObject.FromObject(activity.Value).ToObject<ComposeExtensionQuery>();
        }
    }
}
