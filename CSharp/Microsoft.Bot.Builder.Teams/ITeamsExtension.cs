// <copyright file="ITeamsExtension.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams
{
    using Microsoft.Bot.Connector.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Teams extension for Bot Builder SDK.
    /// </summary>
    public interface ITeamsExtension
    {
        /// <summary>
        /// Gets the teams operations. These are extended set of operations available only for 'MsTeams' channel.
        /// </summary>
        ITeamsOperations Teams { get; }

        /// <summary>
        /// Adds the mention to text. Mention is added at the end of existing text.
        /// </summary>
        /// <typeparam name="T">Type of activity.</typeparam>
        /// <param name="activity">The activity.</param>
        /// <param name="mentionedEntity">The mentioned entity.</param>
        /// <param name="mentionText">The mention text.</param>
        /// <returns>Activity with mentions added to it.</returns>
        T AddMentionToText<T>(T activity, ChannelAccount mentionedEntity, string mentionText = null)
            where T : IMessageActivity;

        /// <summary>
        /// Returns <see cref="TeamsChannelAccount"/> instance with extended properties.
        /// </summary>
        /// <param name="channelAccount">The channel account.</param>
        /// <returns>Instance of <see cref="TeamsChannelAccount"/>.</returns>
        TeamsChannelAccount AsTeamsChannelAccount(ChannelAccount channelAccount);

        /// <summary>
        /// Creates the reply to general channel.
        /// </summary>
        /// <param name="text">The text to be included in reply.</param>
        /// <param name="locale">The locale.</param>
        /// <returns><see cref="Activity"/> with text.</returns>
        Activity CreateReplyToGeneralChannel(string text = null, string locale = null);

        /// <summary>
        /// Gets the tenant identifier on the message.
        /// </summary>
        /// <returns>Tenant Identifier on the message.</returns>
        string GetActivityTenantId();

        /// <summary>
        /// Gets the activity text without mentions. This method replaces at mentions with empty string.
        /// </summary>
        /// <returns>Activity without mentions.</returns>
        string GetActivityTextWithoutMentions();

        /// <summary>
        /// Gets the compose extension query data. This should only be called on ComposeExtentionQuery messages.
        /// Check this by calling <see cref="IsRequestMessagingExtensionQuery"/>.
        /// </summary>
        /// <returns>Compose extension query data.</returns>
        MessagingExtensionQuery GetMessagingExtensionQueryData();

        /// <summary>
        /// Gets the general channel for a given Team conversation.
        /// </summary>
        /// <returns>Channel info for General channel of a team.</returns>
        ChannelInfo GetGeneralChannel();

        /// <summary>
        /// Gets the o365 connector card action query data. This should only be called on O365 Connector card action query data.
        /// Check this by calling <see cref="IsRequestO365ConnectorCardActionQuery"/>.
        /// </summary>
        /// <returns><see cref="O365ConnectorCardActionQuery"/> instance.</returns>
        O365ConnectorCardActionQuery GetO365ConnectorCardActionQueryData();

        /// <summary>
        /// Gets the signin state verification query data. This should only be called for SignIn verification queries.
        /// Check this by calling <see cref="IsRequestSigninStateVerificationQuery"/>.
        /// </summary>
        /// <returns><see cref="SigninStateVerificationQuery"/> instance.</returns>
        SigninStateVerificationQuery GetSigninStateVerificationQueryData();

        /// <summary>
        /// Determines whether request is a compose extension query.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is a compose extension query; otherwise, <c>false</c>.
        /// </returns>
        bool IsRequestMessagingExtensionQuery();

        /// <summary>
        /// Determines whether request is an O365 connector card action query.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is an O365 connector card action query; otherwise, <c>false</c>.
        /// </returns>
        bool IsRequestO365ConnectorCardActionQuery();

        /// <summary>
        /// Determines whether request is a signin state verification query.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if request is a signin state verification query; otherwise, <c>false</c>.
        /// </returns>
        bool IsRequestSigninStateVerificationQuery();

        /// <summary>
        /// Adds notification support to the message. Once this message is delivered user will be notified about it.
        /// </summary>
        /// <typeparam name="T">Type of reply activity.</typeparam>
        /// <param name="replyActivity">The reply activity.</param>
        /// <returns>Activity with notification set to true.</returns>
        T NotifyUser<T>(T replyActivity)
            where T : IMessageActivity;

        /// <summary>
        /// Gets the conversation parameters for create or get direct conversation.
        /// </summary>
        /// <param name="user">The user to create conversation with.</param>
        /// <returns>Conversation parameters to get or create direct conversation (1on1) between bot and user.</returns>
        ConversationParameters GetConversationParametersForCreateOrGetDirectConversation(ChannelAccount user);
    }
}