// <copyright file="IInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Processes activites with <see cref="Schema.Activity.Type"/> set to <see cref="Schema.ActivityTypes.Invoke"/>.
    /// </summary>
    public interface IInvokeActivityHandler
    {
        /// <summary>
        /// Handles the invoke task asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <returns>Invoke response.</returns>
        Task<InvokeResponse> HandleInvokeTaskAsync(ITurnContext turnContext);
    }
}
