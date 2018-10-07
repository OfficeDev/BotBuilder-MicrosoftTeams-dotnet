using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;

namespace Microsoft.Bot.Builder.Teams.AuditBot.AspNet
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ////// Web API configuration and services

            ////// Web API routes
            ////config.MapHttpAttributeRoutes();

            ////config.Routes.MapHttpRoute(
            ////    name: "DefaultApi",
            ////    routeTemplate: "api/{controller}/{id}",
            ////    defaults: new { id = RouteParameter.Optional }
            ////);

            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            config.MapBotFramework(botConfig =>
            {
                // Load Connected Services from .bot file
                var path = HostingEnvironment.MapPath(@"~/BotConfiguration.bot");
                var botConfigurationFile = BotConfiguration.Load(path);
                var endpointService = (EndpointService)botConfigurationFile.Services.First(s => s.Type == "endpoint");

                botConfig
                    .UseMicrosoftApplicationIdentity(endpointService?.AppId, endpointService?.AppPassword);

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new TeamSpecificConversationState(dataStore);
                botConfig.BotFrameworkOptions.State.Add(conversationState);

                // --> Add Teams Middleware.
                botConfig.BotFrameworkOptions.Middleware.Add(
                    new TeamsMiddleware(
                        new SimpleCredentialProvider(endpointService?.AppId, endpointService?.AppPassword),
                        new TeamsMiddlewareOptions
                        {
                            EnableTenantFiltering = false,
                        },
                        null,
                        null));

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new AuditLogAccessor(conversationState)
                {
                    AuditLog = conversationState.CreateProperty<TeamOperationHistory>(AuditLogAccessor.AuditLogName),
                };

                builder.Register<AuditLogAccessor>((component) => accessors);
            });

            builder.RegisterType<TeamsActivityProcessor>().As<IActivityProcessor>();
            builder.RegisterType<TeamsConversationUpdateActivityHandler>().As<ITeamsConversationUpdateActivityHandler>();
            builder.RegisterType<MessageActivityHandler>().As<IMessageActivityHandler>();

            builder.RegisterType<AuditBot>().As<IBot>().InstancePerRequest();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
