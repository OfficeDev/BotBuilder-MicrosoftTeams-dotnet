using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    public class Startup
    {
        private bool isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            BotConfiguration botConfig = null;
            try
            {
                botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
            }
            catch
            {
                var msg = "Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.\n" +
                    " - The botFileSecret is available under appsettings for your Azure Bot Service bot.\n" +
                    " - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.\n" +
                    " - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.\n\n";
                throw new InvalidOperationException(msg);
            }

            services.AddSingleton(sp => botConfig);

            // Retrieve current endpoint.
            var environment = isProduction ? "production" : "development";
            var botService = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
            if (!(botService is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }

            services.AddTransient<IActivityProcessor, TeamsActivityProcessor>();
            services.AddTransient<ITeamsConversationUpdateActivityHandler, TeamsConversationUpdateActivityHandler>();
            services.AddTransient<IMessageActivityHandler, MessageActivityHandler>();

            services.AddBot<AuditBot>(options =>
            {
                IStorage dataStore = new MemoryStorage();

                // --> Adding conversation state handler which understands a team as single conversation.
                options.State.Add(new TeamSpecificConversationState(dataStore));

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // --> Add Teams Middleware.
                options.Middleware.Add(
                    new TeamsMiddleware(
                        options.CredentialProvider,
                        new TeamsMiddlewareOptions
                        {
                            EnableTenantFiltering = false,
                        },
                        null,
                        null));

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
            });

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var conversationState = options.State.OfType<TeamSpecificConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new AuditLogAccessor(conversationState)
                {
                    AuditLog = conversationState.CreateProperty<TeamOperationHistory>(AuditLogAccessor.AuditLogName),
                };

                return accessors;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
