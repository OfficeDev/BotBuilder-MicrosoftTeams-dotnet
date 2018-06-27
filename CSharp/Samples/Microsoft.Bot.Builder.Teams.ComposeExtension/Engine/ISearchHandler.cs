using System.Threading.Tasks;
using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.Bot.Builder.Teams.ComposeExtension.Engine
{
    public interface ISearchHandler
    {
        Task<ComposeExtensionResult> GetSearchResultAsync(ComposeExtensionActivityAction composeExtensionActivityAction);
    }
}