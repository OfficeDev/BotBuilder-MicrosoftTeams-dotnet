namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IActivityProcessor
    {
        Task ProcessIncomingActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}