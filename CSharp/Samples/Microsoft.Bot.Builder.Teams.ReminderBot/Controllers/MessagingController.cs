using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Bot.Builder.Teams.ReminderBot.Controllers
{
    [Route("api/messages")]
    public class MessagingController : Controller
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

        public async Task<IActionResult> PostActivityAsync([FromBody] Activity activity)
        {
            var activityResponse = await this.botFrameworkAdapter.ProcessActivityAsync(
                this.Request.Headers["Authorization"],
                activity,
                this.activityProcessor.ProcessIncomingActivityAsync,
                this.HttpContext.RequestAborted);

            if (activityResponse == null)
            {
                return new OkResult();
            }
            else
            {
                return new ObjectResult(activityResponse.Body)
                {
                    StatusCode = activityResponse.Status,
                };
            }
        }
    }
}