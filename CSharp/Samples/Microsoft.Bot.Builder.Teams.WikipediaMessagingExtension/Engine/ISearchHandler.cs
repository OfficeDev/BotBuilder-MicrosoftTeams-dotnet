using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    public interface ISearchHandler
    {
        Task<MessagingExtensionResult> GetSearchResultAsync(MessagingExtensionActivityAction messagingExtensionActivityAction);
    }
}