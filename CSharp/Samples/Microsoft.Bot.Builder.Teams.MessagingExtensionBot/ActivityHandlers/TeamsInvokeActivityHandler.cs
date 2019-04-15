// <copyright file="TeamsInvokeActivityHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.MessagingExtensionBot.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Abstractions.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles Teams invoke activity.
    /// </summary>
    /// <seealso cref="ITeamsInvokeActivityHandler" />
    public class TeamsInvokeActivityHandler : TeamsInvokeActivityHandlerBase
    {
        /// <summary>
        /// Handles the messaging extension action asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context</param>
        /// <param name="query">The invoke query object</param>
        /// <returns>
        /// Task tracking operation.
        /// </returns>
        public override async Task<InvokeResponse> HandleMessagingExtensionQueryAsync(ITurnContext turnContext, MessagingExtensionQuery query)
        {
            var heroCard = new HeroCard("Result Card", null, "<pre>This card mocks the CE results</pre>");
            var previewCard = new ThumbnailCard("Search Item Card", null, "This is to show the search result");
            return new InvokeResponse
            {
                Body = new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult()
                    {
                        Type = "result",
                        AttachmentLayout = "list",
                        Attachments = new List<MessagingExtensionAttachment>()
                            {
                                heroCard.ToAttachment().ToMessagingExtensionAttachment(previewCard.ToAttachment()),
                            },
                    },
                },
                Status = 200,
            };
        }

        /// <summary>
        /// Handles messaging extension action of "fetch task" asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of messaging extension action.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleMessagingExtensionFetchTaskAsync(ITurnContext turnContext, MessagingExtensionAction query)
        {
            return new InvokeResponse
            {
                Status = 200,
                Body = new MessagingExtensionActionResponse
                {
                    Task = this.TaskModuleResponseTask(query, false),
                },
            };
        }

        /// <summary>
        /// Handles messaging extension action of "submit action" asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of messaging extension action.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleMessagingExtensionSubmitActionAsync(ITurnContext turnContext, MessagingExtensionAction query)
        {
            bool done = false;
            JObject data = null;
            if (query.Data != null)
            {
                data = JObject.FromObject(query.Data);
                done = (bool)data["done"];
            }

            var body = new MessagingExtensionActionResponse();

            if (data != null && done)
            {
                string sharedMessage = string.Empty;
                if (query.CommandId.Equals("shareMessage") && query.CommandContext.Equals("message"))
                {
                    sharedMessage = $"Shared message: <div style=\"background:#F0F0F0\">{JObject.FromObject(query.MessagePayload).ToString()}</div><br/>";
                }

                var preview = new ThumbnailCard("Created Card", null, $"Your input: {data["userText"]?.ToString()}").ToAttachment();
                var heroCard = new HeroCard("Created Card", null, $"{sharedMessage}Your input: {data["userText"]?.ToString()}").ToAttachment();
                var resultCards = new List<MessagingExtensionAttachment> { heroCard.ToMessagingExtensionAttachment(preview) };

                body.ComposeExtension = new MessagingExtensionResult("list", "result", resultCards);
            }
            else if ((query.CommandId != null && query.CommandId.Equals("createWithPreview")) || query.BotMessagePreviewAction != null)
            {
                if (query.BotMessagePreviewAction == null)
                {
                    body.ComposeExtension = new MessagingExtensionResult
                    {
                        Type = "botMessagePreview",
                        ActivityPreview = new Activity
                        {
                            Attachments = new List<Attachment> { this.TaskModuleResponseCard(query, null) },
                        },
                    };
                }
                else
                {
                    var userEditActivities = query.BotActivityPreview;
                    var card = userEditActivities?[0]?.Attachments?[0];
                    if (card == null)
                    {
                        body.Task = new TaskModuleMessageResponse
                        {
                            Type = "message",
                            Value = "Missing user edit card. Something wrong on Teams client.",
                        };
                    }
                    else if (query.BotMessagePreviewAction.Equals("send"))
                    {
                        Activity activity = turnContext.Activity.CreateReply();
                        activity.Attachments = new List<Attachment> { card };
                        await turnContext.SendActivityAsync(activity).ConfigureAwait(false);
                    }
                    else if (query.BotMessagePreviewAction.Equals("edit"))
                    {
                        body.Task = new TaskModuleContinueResponse
                        {
                            Type = "continue",
                            Value = new TaskModuleTaskInfo
                            {
                                Card = card,
                            },
                        };
                    }
                }
            }
            else
            {
                body.Task = this.TaskModuleResponseTask(query, false);
            }

            return new InvokeResponse
            {
                Status = 200,
                Body = body,
            };
        }

        /// <summary>
        /// Handles task module fetch asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of task module request.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleTaskModuleFetchAsync(ITurnContext turnContext, TaskModuleRequest query)
        {
            return new InvokeResponse
            {
                Status = 200,
                Body = new TaskModuleResponse
                {
                    Task = this.TaskModuleResponseTask(query, false),
                },
            };
        }

        /// <summary>
        /// Handles task module submit asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The query object of task module request.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleTaskModuleSubmitAsync(ITurnContext turnContext, TaskModuleRequest query)
        {
            bool done = false;
            if (query.Data != null)
            {
                var data = JObject.FromObject(query.Data);
                done = (bool)data["done"];
            }

            return new InvokeResponse
            {
                Status = 200,
                Body = new TaskModuleResponse
                {
                    Task = this.TaskModuleResponseTask(query, done),
                },
            };
        }

        /// <summary>
        /// Handles app-based link query asynchronously.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="query">The app-based link query.</param>
        /// <returns>Task tracking operation.</returns>
        public override async Task<InvokeResponse> HandleAppBasedLinkQueryAsync(ITurnContext turnContext, AppBasedLinkQuery query)
        {
            var previewImg = new List<CardImage>
            {
                new CardImage("https://assets.pokemon.com/assets/cms2/img/pokedex/full/025.png", "Pokemon"),
            };
            var preview = new ThumbnailCard("Preview Card", null, $"Your query URL: {query.Url}", previewImg).ToAttachment();
            var heroCard = new HeroCard("Preview Card", null, $"Your query URL: <pre>{query.Url}</pre>", previewImg).ToAttachment();
            var resultCards = new List<MessagingExtensionAttachment> { heroCard.ToMessagingExtensionAttachment(preview) };

            return new InvokeResponse
            {
                Status = 200,
                Body = new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult("list", "result", resultCards),
                },
            };
        }

        private TaskModuleResponseBase TaskModuleResponseTask(TaskModuleRequest query, bool done)
        {
            if (done)
            {
                return new TaskModuleMessageResponse()
                {
                    Type = "message",
                    Value = "Thanks for your inputs!",
                };
            }
            else
            {
                string textValue = null;
                if (query.Data != null)
                {
                    var data = JObject.FromObject(query.Data);
                    textValue = data["userText"]?.ToString();
                }

                return new TaskModuleContinueResponse()
                {
                    Type = "continue",
                    Value = new TaskModuleTaskInfo()
                    {
                        Title = "More Page",
                        Card = this.TaskModuleResponseCard(query, textValue),
                    },
                };
            }
        }

        private Attachment TaskModuleResponseCard(TaskModuleRequest query, string textValue)
        {
            AdaptiveCards.AdaptiveCard adaptiveCard = new AdaptiveCards.AdaptiveCard();

            adaptiveCard.Body.Add(new AdaptiveCards.AdaptiveTextBlock("Your Request:")
            {
                Size = AdaptiveCards.AdaptiveTextSize.Large,
                Weight = AdaptiveCards.AdaptiveTextWeight.Bolder,
            });

            adaptiveCard.Body.Add(new AdaptiveCards.AdaptiveContainer()
            {
                Style = AdaptiveCards.AdaptiveContainerStyle.Emphasis,
                Items = new List<AdaptiveCards.AdaptiveElement>
                {
                    new AdaptiveCards.AdaptiveTextBlock(JObject.FromObject(query).ToString())
                    {
                        Wrap = true,
                    },
                },
            });

            adaptiveCard.Body.Add(new AdaptiveCards.AdaptiveTextInput()
            {
                Id = "userText",
                Placeholder = "Type text here...",
                Value = textValue,
            });

            adaptiveCard.Actions.Add(new AdaptiveCards.AdaptiveSubmitAction()
            {
                Title = "Next",
                Data = JObject.Parse(@"{ ""done"": false }"),
            });

            adaptiveCard.Actions.Add(new AdaptiveCards.AdaptiveSubmitAction()
            {
                Title = "Submit",
                Data = JObject.Parse(@"{ ""done"": true }"),
            });

            return adaptiveCard.ToAttachment();
        }
    }
}
