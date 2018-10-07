using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams.RemindMeBot.Engine;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Bot.Builder.Teams.RemindMeBot
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
            services.AddTransient<IProactiveMessageManager, ProactiveMessageManager>();
            services.AddTransient<IRecognizer, ReminderTextRecognizer>();

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
            var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }

            ICredentialProvider credentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);
            services.AddSingleton<ICredentialProvider>(credentialProvider);

            services.AddBot<RemindMeBot>(options =>
            {
                options.CredentialProvider = credentialProvider;

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
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
