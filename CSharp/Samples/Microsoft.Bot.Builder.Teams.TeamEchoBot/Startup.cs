// <copyright file="Startup.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.TeamEchoBot
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Builder.Teams.Middlewares;
    using Microsoft.Bot.Builder.Teams.StateStorage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Starts up the web application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The hosting env.</param>
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services. This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddBot<EchoBot>(options =>
            {
                IStorage dataStore = new MemoryStorage();

                // --> Adding conversation state handler which understands a team as single conversation.
                options.State.Add(new TeamSpecificConversationState(dataStore));

                // Drop all activities not coming from Microsoft Teams.
                options.Middleware.Add(new DropNonTeamsActivitiesMiddleware());

                // --> Add Teams Middleware.
                options.Middleware.Add(
                    new TeamsMiddleware(
                        new ConfigurationCredentialProvider(this.Configuration)));

                options.CredentialProvider = new ConfigurationCredentialProvider(this.Configuration);
            });

            services.AddSingleton(sp =>
            {
                BotFrameworkOptions options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                TeamSpecificConversationState conversationState = options.State.OfType<TeamSpecificConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new EchoStateAccessor(conversationState)
                {
                    CounterState = conversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName),
                };

                return accessors;
            });
        }

        /// <summary>
        /// Configures the specified application. This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The hosting env.</param>
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
