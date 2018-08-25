using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Teams.TeamsMemberHistoryBot.Controllers
{
    public class MessagingController : ApiController
    {
        /// <summary>
        /// The bot framework adapter.
        /// </summary>
        private readonly BotFrameworkAdapter botFrameworkAdapter;

        /// <summary>
        /// The incoming activity processor.
        /// </summary>
        private readonly IActivityProcessor activityProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingController"/> class.
        /// </summary>
        /// <param name="botFrameworkAdapter">The bot framework adapter.</param>
        public MessagingController(BotFrameworkAdapter botFrameworkAdapter, IActivityProcessor activityProcessor)
        {
            this.botFrameworkAdapter = botFrameworkAdapter;
            this.activityProcessor = activityProcessor;
        }

        [Route("api/messages")]
        [HttpPost]
        public async Task<IHttpActionResult> PostActivityAsync([FromBody] Activity activity)
        {
            var activityResponse = await this.botFrameworkAdapter.ProcessActivityAsync(
                this.Request.Headers.Authorization.ToString(),
                activity,
                this.activityProcessor.ProcessIncomingActivityAsync,
                CancellationToken.None).ConfigureAwait(false);

            if (activityResponse == null)
            {
                return this.Ok();
            }
            else
            {
                return this.ResponseMessage(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(activityResponse.Body)),
                    StatusCode = (HttpStatusCode)activityResponse.Status,
                });
            }
        }
    }
}