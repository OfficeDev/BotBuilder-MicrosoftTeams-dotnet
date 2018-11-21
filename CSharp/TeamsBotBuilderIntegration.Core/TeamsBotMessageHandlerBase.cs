namespace TeamsBotBuilderIntegration.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;

    /// <summary>
    /// TeamsBotMessage handler base
    /// </summary>
    public abstract class TeamsBotMessageHandlerBase
    {
        public static readonly JsonSerializer BotMessageSerializer = JsonSerializer.Create(MessageSerializerSettings.Create());

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsBotMessageHandlerBase"/> class.
        /// </summary>
        public TeamsBotMessageHandlerBase()
        {
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="httpContext">HttpContext.</param>
        /// <returns>Task</returns>
        public async Task HandleAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (request.Method != HttpMethods.Post)
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;

                return;
            }

            if (request.ContentLength == 0)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;

                return;
            }

            if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeaderValue)
                    ||
                mediaTypeHeaderValue.MediaType != "application/json")
            {
                response.StatusCode = (int)HttpStatusCode.NotAcceptable;

                return;
            }

            var requestServices = httpContext.RequestServices;
            var adapter = requestServices.GetRequiredService<IAdapterIntegration>();
            var bot = requestServices.GetRequiredService<IBot>();

            try
            {
                // TODO wire up cancellation
#pragma warning disable UseConfigureAwait // Use ConfigureAwait
                var invokeResponse = await ProcessMessageRequestAsync(
                    request,
                    adapter,
                    bot.OnTurnAsync,
                    default(CancellationToken));
#pragma warning restore UseConfigureAwait // Use ConfigureAwait

                if (invokeResponse == null)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    response.StatusCode = invokeResponse.Status;

                    if (response.Body != null)
                    {
                        response.ContentType = "application/json";
                        using (var writer = new StreamWriter(response.Body))
                        {
                            using (var jsonWriter = new JsonTextWriter(writer))
                            {
                                BotMessageSerializer.Serialize(jsonWriter, invokeResponse.Body);
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }

        /// <summary>
        /// Processes incoming request.
        /// </summary>
        /// <param name="request">HttpRequest.</param>
        /// <param name="adapter">Adapter.</param>
        /// <param name="botCallbackHandler">Callback handler.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>InvokeResponse.</returns>
        protected abstract Task<InvokeResponse> ProcessMessageRequestAsync(HttpRequest request, IAdapterIntegration adapter, BotCallbackHandler botCallbackHandler, CancellationToken cancellationToken);
    }
}
