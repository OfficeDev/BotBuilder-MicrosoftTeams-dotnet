namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    public interface IConversationUpdateActivityHandler
    {
        Task HandleConversationUpdateActivityTask(ITurnContext turnContext);
    }
}
