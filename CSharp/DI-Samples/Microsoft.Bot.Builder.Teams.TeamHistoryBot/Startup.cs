using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Builder.Teams.TeamHistoryBot.Engine;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bot.Builder.Teams.TeamHistoryBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////services.Configure<CosmosDbStorageOptions>(storageOptions =>
            ////{
            ////    // Using CosmosDB Storage Emulator.
            ////    storageOptions.CosmosDBEndpoint = new Uri("https://localhost:8081");
            ////    storageOptions.AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            ////    storageOptions.CollectionId = "ConversationStates";
            ////    storageOptions.DatabaseId = "TeamsMemberHistoryBot";
            ////});

            services.AddSingleton<CosmosDbStorageOptions>(new CosmosDbStorageOptions
            {
                // Using CosmosDB Storage Emulator.
                CosmosDBEndpoint = new Uri("https://localhost:8081"),
                AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                CollectionId = "ConversationStates",
                DatabaseId = "TeamHistoryBotASPNetCore",
            });

            services.AddSingleton<ICredentialProvider>(
                new SimpleCredentialProvider(
                    this.Configuration["BotAppSettings:AppId"],
                    this.Configuration["BotAppSettings:AppPassword"]));

            services.AddSingleton<IMiddleware, TeamsMiddleware>();

            // Using CosmosDB.
            services.AddSingleton<IStorage, CosmosDbStorage>();

            // We want conversation state to be stored at Team level not conversation (channel).
            services.AddSingleton<BotState, TeamSpecificConversationState>();
            services.AddSingleton<IMiddleware>(context => context.GetRequiredService<BotState>());
            services.AddSingleton<IStatePropertyAccessor<TeamOperationHistory>>((context) =>
                context.GetRequiredService<TeamSpecificConversationState>().CreateProperty<TeamOperationHistory>("TeamHistory"));

            // We only service Team message.
            services.AddSingleton<IMiddleware, DenyNonTeamMessage>();

            services.AddSingleton<IMessageActivityHandler, MessageActivityHandler>();
            services.AddSingleton<ITeamsConversationUpdateActivityHandler, TeamsConversationUpdateActivityHandler>();
            services.AddSingleton<IActivityProcessor, TeamsActivityProcessor>();
            services.AddSingleton<BotFrameworkAdapter>((serviceProvider) =>
            {
                IEnumerable<IMiddleware> middlewares = serviceProvider.GetServices<IMiddleware>();
                ICredentialProvider credentialProvider = serviceProvider.GetRequiredService<ICredentialProvider>();

                BotFrameworkAdapter botFrameworkAdapter = new BotFrameworkAdapter(credentialProvider);

                foreach (IMiddleware middleware in middlewares)
                {
                    botFrameworkAdapter.Use(middleware);
                }

                return botFrameworkAdapter;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
