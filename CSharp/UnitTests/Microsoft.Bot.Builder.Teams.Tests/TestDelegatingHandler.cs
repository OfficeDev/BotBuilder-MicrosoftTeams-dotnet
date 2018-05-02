// <copyright file="TestDelegatingHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Test delegating handler.
    /// </summary>
    public class TestDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        /// The send function to be executed on request.
        /// </summary>
        private Func<HttpRequestMessage, Task<HttpResponseMessage>> sendFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDelegatingHandler"/> class.
        /// </summary>
        /// <param name="sendAsyncFunc">Function to be executed when request is made.</param>
        public TestDelegatingHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsyncFunc)
        {
            this.sendFunc = sendAsyncFunc;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="Task" />. The task object representing the asynchronous operation.
        /// </returns>
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await this.sendFunc.Invoke(request).ConfigureAwait(false);
        }
    }
}
