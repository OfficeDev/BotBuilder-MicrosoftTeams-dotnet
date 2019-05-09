// <copyright file="TeamsActivityProcessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using HtmlAgilityPack;

    /// <summary>
    /// Teams activity processor.
    /// </summary>
    /// <seealso cref="IActivityProcessor" />
    public class TeamsActivityProcessor : IActivityProcessor
    {
        /// <summary>
        /// The message activity handler.
        /// </summary>
        private readonly IMessageActivityHandler messageActivityHandler;

        /// <summary>
        /// The conversation update activity handler.
        /// </summary>
        private readonly ITeamsConversationUpdateActivityHandler conversationUpdateActivityHandler;

        /// <summary>
        /// The invoke activity handler.
        /// </summary>
        private readonly ITeamsInvokeActivityHandler invokeActivityHandler;

        /// <summary>
        /// The message reaction activity handler.
        /// </summary>
        private readonly IMessageReactionActivityHandler messageReactionActivityHandler;

        /// <summary>
        /// HTML tags that we need to keep in MessagePayload.
        /// </summary>
        private readonly ISet<string> TextRestrictedHtmlTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsActivityProcessor"/> class.
        /// </summary>
        /// <param name="messageActivityHandler">The message activity handler.</param>
        /// <param name="conversationUpdateActivityHandler">The conversation update activity handler.</param>
        /// <param name="invokeActivityHandler">The invoke activity handler.</param>
        /// <param name="messageReactionActivityHandler">The message reaction activity handler.</param>
        public TeamsActivityProcessor(
            IMessageActivityHandler messageActivityHandler = null,
            ITeamsConversationUpdateActivityHandler conversationUpdateActivityHandler = null,
            ITeamsInvokeActivityHandler invokeActivityHandler = null,
            IMessageReactionActivityHandler messageReactionActivityHandler = null)
        {
            this.messageActivityHandler = messageActivityHandler;
            this.conversationUpdateActivityHandler = conversationUpdateActivityHandler;
            this.invokeActivityHandler = invokeActivityHandler;
            this.messageReactionActivityHandler = messageReactionActivityHandler;
        }

        /// <summary>
        /// Processes the incoming activity asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public virtual async Task ProcessIncomingActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        if (this.messageActivityHandler != null)
                        {
                            await this.messageActivityHandler.HandleMessageAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.ConversationUpdate:
                    {
                        if (this.conversationUpdateActivityHandler != null)
                        {
                            await this.ProcessTeamsConversationUpdateAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.Invoke:
                    {
                        if (this.invokeActivityHandler != null)
                        {
                            InvokeResponse invokeResponse = await this.ProcessTeamsInvokeActivityAsync(turnContext).ConfigureAwait(false);
                            await turnContext.SendActivityAsync(
                                new Activity
                                {
                                    Value = invokeResponse,
                                    Type = ActivityTypesEx.InvokeResponse,
                                }).ConfigureAwait(false);
                        }

                        return;
                    }

                case ActivityTypes.MessageReaction:
                    {
                        if (this.messageReactionActivityHandler != null)
                        {
                            await this.messageReactionActivityHandler.HandleMessageReactionAsync(turnContext).ConfigureAwait(false);
                        }

                        return;
                    }
            }
        }

        private async Task ProcessTeamsConversationUpdateAsync(ITurnContext turnContext)
        {
            if (turnContext.Activity.ChannelData != null)
            {
                TeamsChannelData channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

                if (!string.IsNullOrEmpty(channelData?.EventType))
                {
                    switch (channelData.EventType)
                    {
                        case "teamMemberAdded":
                            {
                                await this.conversationUpdateActivityHandler.HandleTeamMembersAddedEventAsync(new TeamMembersAddedEvent
                                {
                                    MembersAdded = turnContext.Activity.MembersAdded,
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                }).ConfigureAwait(false);

                                return;
                            }

                        case "teamMemberRemoved":
                            {
                                await this.conversationUpdateActivityHandler.HandleTeamMembersRemovedEventAsync(new TeamMembersRemovedEvent
                                {
                                    MembersRemoved = turnContext.Activity.MembersRemoved,
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                }).ConfigureAwait(false);

                                return;
                            }

                        case "channelCreated":
                            {
                                await this.conversationUpdateActivityHandler.HandleChannelCreatedEventAsync(new ChannelCreatedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                }).ConfigureAwait(false);

                                return;
                            }

                        case "channelDeleted":
                            {
                                await this.conversationUpdateActivityHandler.HandleChannelDeletedEventAsync(new ChannelDeletedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                }).ConfigureAwait(false);

                                return;
                            }

                        case "channelRenamed":
                            {
                                await this.conversationUpdateActivityHandler.HandleChannelRenamedEventAsync(new ChannelRenamedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                    Channel = channelData.Channel,
                                }).ConfigureAwait(false);

                                return;
                            }

                        case "teamRenamed":
                            {
                                await this.conversationUpdateActivityHandler.HandleTeamRenamedEventAsync(new TeamRenamedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                }).ConfigureAwait(false);

                                return;
                            }
                    }
                }
            }

            await this.conversationUpdateActivityHandler.HandleConversationUpdateActivityTaskAsync(turnContext).ConfigureAwait(false);
        }

        private async Task<InvokeResponse> ProcessTeamsInvokeActivityAsync(ITurnContext turnContext)
        {
            ITeamsContext teamsContext = turnContext.TurnState.Get<ITeamsContext>();

            if (teamsContext.IsRequestO365ConnectorCardActionQuery())
            {
                return await this.invokeActivityHandler.HandleO365ConnectorCardActionAsync(turnContext, teamsContext.GetO365ConnectorCardActionQueryData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestSigninStateVerificationQuery())
            {
                return await this.invokeActivityHandler.HandleSigninStateVerificationActionAsync(turnContext, teamsContext.GetSigninStateVerificationQueryData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestFileConsentResponse())
            {
                return await this.invokeActivityHandler.HandleFileConsentResponseAsync(turnContext, teamsContext.GetFileConsentQueryData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestMessagingExtensionQuery())
            {
                return await this.invokeActivityHandler.HandleMessagingExtensionQueryAsync(turnContext, teamsContext.GetMessagingExtensionQueryData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestAppBasedLinkQuery())
            {
                return await this.invokeActivityHandler.HandleAppBasedLinkQueryAsync(turnContext, teamsContext.GetAppBasedLinkQueryData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestMessagingExtensionFetchTask())
            {   
                MessagingExtensionAction messagingExtensionActionData = teamsContext.GetMessagingExtensionActionData();
                messagingExtensionActionData.MessagePayload.body.textContent = this.StripHtmlTags(messagingExtensionActionData.MessagePayload.body.content);
                return await this.invokeActivityHandler.HandleMessagingExtensionFetchTaskAsync(turnContext, messagingExtensionActionData).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestMessagingExtensionSubmitAction())
            {
                MessagingExtensionAction messagingExtensionActionData = teamsContext.GetMessagingExtensionActionData();
                messagingExtensionActionData.MessagePayload.body.textContent = this.StripHtmlTags(messagingExtensionActionData.MessagePayload.body.content);                
                return await this.invokeActivityHandler.HandleMessagingExtensionSubmitActionAsync(turnContext, messagingExtensionActionData).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestTaskModuleFetch())
            {
                return await this.invokeActivityHandler.HandleTaskModuleFetchAsync(turnContext, teamsContext.GetTaskModuleRequestData()).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestTaskModuleSubmit())
            {
                return await this.invokeActivityHandler.HandleTaskModuleSubmitAsync(turnContext, teamsContext.GetTaskModuleRequestData()).ConfigureAwait(false);
            }

            return await this.invokeActivityHandler.HandleInvokeTaskAsync(turnContext).ConfigureAwait(false);
        }

        private static string StripHtmlTags(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            this.TextRestrictedHtmlTags = new HashSet<string> { "at", "attachment" };
            return StripHtmlTagsHelper(doc.DocumentNode, this.TextRestrictedHtmlTags);
        }

        private static string StripHtmlTagsHelper(HtmlNode node, ISet<string> tags)
        {
            string result = "";
            if (tags.Contains(node.Name))
            {
                result += node.OuterHtml;
            }
            else
            {
                foreach (HtmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == HtmlNodeType.Text)
                    {
                        result += childNode.InnerText;
                    }
                    else
                    {
                        result += StripHtmlTagsHelper(childNode, tags);
                    }
                }
            }
            return result;
        }        
    }
}
