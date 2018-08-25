namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    public interface IInvokeActivityHandler
    {
        Task<InvokeResponse> HandleInvokeTask(ITurnContext turnContext);
    }
}
