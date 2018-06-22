namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    public interface IMessageReactionActivityHandler
    {
        Task HandleMessageReactionAsync(ITurnContext turnContext);
    }
}
