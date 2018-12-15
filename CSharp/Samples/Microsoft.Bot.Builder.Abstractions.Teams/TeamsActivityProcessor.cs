// <copyright file="TeamsActivityProcessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

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

            if (teamsContext.IsRequestMessagingExtensionQuery())
            {
                return await this.invokeActivityHandler.HandleMessagingExtensionActionAsync(new MessagingExtensionActivityAction
                {
                    MessagingExtensionQuery = teamsContext.GetMessagingExtensionQueryData(),
                    TurnContext = turnContext,
                }).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestO365ConnectorCardActionQuery())
            {
                return await this.invokeActivityHandler.HandleO365ConnectorCardActionAsync(new O365ConnectorCardActivityAction
                {
                    CardActionQuery = teamsContext.GetO365ConnectorCardActionQueryData(),
                    TurnContext = turnContext,
                }).ConfigureAwait(false);
            }

            if (teamsContext.IsRequestSigninStateVerificationQuery())
            {
                return await this.invokeActivityHandler.HandleSigninStateVerificationActionAsync(new SigninStateVerificationActivityAction
                {
                    TurnContext = turnContext,
                    VerificationQuery = teamsContext.GetSigninStateVerificationQueryData(),
                }).ConfigureAwait(false);
            }

            return await this.invokeActivityHandler.HandleInvokeTaskAsync(turnContext).ConfigureAwait(false);
        }
    }
}
