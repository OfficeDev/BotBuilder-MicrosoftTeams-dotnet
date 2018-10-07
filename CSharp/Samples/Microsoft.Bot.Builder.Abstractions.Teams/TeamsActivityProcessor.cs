namespace Microsoft.Bot.Builder.Abstractions.Teams
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
    using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    public class TeamsActivityProcessor : IActivityProcessor
    {
        private readonly IMessageActivityHandler messageActivityHandler;

        private readonly ITeamsConversationUpdateActivityHandler conversationUpdateActivityHandler;

        private readonly ITeamsInvokeActivityHandler invokeActivityHandler;

        private readonly IMessageReactionActivityHandler messageReactionActivityHandler;

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

        public virtual async Task ProcessIncomingActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        if (this.messageActivityHandler != null)
                        {
                            await this.messageActivityHandler.HandleMessageAsync(turnContext);
                        }

                        return;
                    }

                case ActivityTypes.ConversationUpdate:
                    {
                        if (this.conversationUpdateActivityHandler != null)
                        {
                            await this.ProcessTeamsConversationUpdateAsync(turnContext);
                        }

                        return;
                    }

                case ActivityTypes.Invoke:
                    {
                        if (this.invokeActivityHandler != null)
                        {
                            InvokeResponse invokeResponse = await this.ProcessTeamsInvokeActivityAsync(turnContext);
                            await turnContext.SendActivityAsync(
                                new Activity
                                {
                                    Value = invokeResponse,
                                    Type = ActivityTypesEx.InvokeResponse,
                                });
                        }

                        return;
                    }

                case ActivityTypes.MessageReaction:
                    {
                        if (this.messageReactionActivityHandler != null)
                        {
                            await this.messageReactionActivityHandler.HandleMessageReactionAsync(turnContext);
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
                                });

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
                                });

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
                                });

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
                                });

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
                                });

                                return;
                            }

                        case "teamRenamed":
                            {
                                await this.conversationUpdateActivityHandler.HandleTeamRenamedEventAsync(new TeamRenamedEvent
                                {
                                    TurnContext = turnContext,
                                    Team = channelData.Team,
                                    Tenant = channelData.Tenant,
                                });

                                return;
                            }
                    }
                }
            }

            await this.conversationUpdateActivityHandler.HandleConversationUpdateActivityTask(turnContext);
        }

        private async Task<InvokeResponse> ProcessTeamsInvokeActivityAsync(ITurnContext turnContext)
        {
            ITeamsExtension teamsExtension = turnContext.TurnState.Get<ITeamsExtension>();

            if (teamsExtension.IsRequestMessagingExtensionQuery())
            {
                return await this.invokeActivityHandler.HandleMessagingExtensionActionAsync(new MessagingExtensionActivityAction
                {
                    ComposeExtensionQuery = teamsExtension.GetMessagingExtensionQueryData(),
                    TurnContext = turnContext,
                });
            }

            if (teamsExtension.IsRequestO365ConnectorCardActionQuery())
            {
                return await this.invokeActivityHandler.HandleO365ConnectorCardActionAsync(new O365ConnectorCardActivityAction
                {
                    CardActionQuery = teamsExtension.GetO365ConnectorCardActionQueryData(),
                    TurnContext = turnContext,
                });
            }

            if (teamsExtension.IsRequestSigninStateVerificationQuery())
            {
                return await this.invokeActivityHandler.HandleSigninStateVerificationActionAsync(new SigninStateVerificationActivityAction
                {
                    TurnContext = turnContext,
                    VerificationQuery = teamsExtension.GetSigninStateVerificationQueryData(),
                });
            }

            return await this.invokeActivityHandler.HandleInvokeTask(turnContext);
        }
    }
}
