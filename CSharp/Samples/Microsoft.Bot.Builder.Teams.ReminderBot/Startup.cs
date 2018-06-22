using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Teams.ReminderBot.Engine;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Bot.Builder.Teams.ReminderBot
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
                options.EnableTenantFiltering = false;
            });

            services.AddSingleton<ICredentialProvider>(new SimpleCredentialProvider("36c331a0-5800-403c-bfff-c9a3bec2a3c5", "vqTVA2)_mxjcpWAFT8810~%"));
            services.AddSingleton<IRecognizer, ReminderTextRecognizer>();
            services.AddSingleton<IMiddleware, TeamsMiddleware>();
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
            services.AddMvc(options =>
            {
                var mappings = options.FormatterMappings;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
