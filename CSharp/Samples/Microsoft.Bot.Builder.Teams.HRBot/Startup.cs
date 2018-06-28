using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Teams.HRBot.Engine;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Bot.Builder.Teams.HRBot
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
            services.Configure<TeamsMiddlewareOptions>((options) =>
            {
                options.EnableTenantFiltering = true;
                options.WhitelistedTenants = this.Configuration["AllowedTenants"].Split(',', StringSplitOptions.RemoveEmptyEntries);
            });

            services.Configure<OAuthSettings>((options) =>
            {
                options.Resource = this.Configuration["OAuthSettings:Resource"];
                options.ClientId = this.Configuration["OAuthSettings:ClientId"];
                options.RedirectUri = new Uri(this.Configuration["OAuthSettings:RedirectUri"]);
            });

            services.AddSingleton<ICredentialProvider>(
                new SimpleCredentialProvider(
                    this.Configuration["BotAppSettings:AppId"],
                    this.Configuration["BotAppSettings:AppPassword"]));

            services.AddSingleton<IMiddleware, TeamsMiddleware>();

            // Using InMemoryStorage for storing tokens.
            services.AddSingleton<IStorage, MemoryStorage>();

            // This is ok for Sample but in real life you might want to encrypt tokens before serializing them.
            services.AddSingleton<IMiddleware, ConversationState<UserDetails>>();

            // Not working in Team.
            services.AddSingleton<IMiddleware, DenyTeamMessages>();
            services.AddSingleton<IMessageActivityHandler, MessageActivityHandler>();
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

            services.AddMvc();
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
