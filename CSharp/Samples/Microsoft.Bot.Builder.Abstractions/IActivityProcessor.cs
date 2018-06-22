namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    public interface IActivityProcessor
    {
        Task ProcessIncomingActivityAsync(ITurnContext turnContext);
    }
}