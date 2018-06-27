using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Abstractions.Teams;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Teams.SampleMiddlewares;
using Microsoft.Bot.Builder.Teams.TeamHistoryBot.Engine;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.Bot.Builder.Teams.TeamHistoryBot
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ContainerBuilder builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.Register<ICredentialProvider>((context) => 
                new SimpleCredentialProvider(
                    ConfigurationManager.AppSettings["BotAppSettings.AppId"],
                    ConfigurationManager.AppSettings["BotAppSettings.AppPassword"])).SingleInstance();

            builder.Register<CosmosDbStorageOptions>((context) =>
                new CosmosDbStorageOptions
                {
                    // Using CosmosDB Storage Emulator.
                    CosmosDBEndpoint = new Uri("https://localhost:8081"),
                    AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    CollectionId = "ConversationStates",
                    DatabaseId = "TeamHistoryBotASPNet",
                }).SingleInstance();

            builder.RegisterType<TeamsMiddleware>().As<IMiddleware>().SingleInstance();
            builder.RegisterType<DenyNonTeamMessage>().As<IMiddleware>().SingleInstance();

            builder.RegisterType<MessageActivityHandler>().As<IMessageActivityHandler>().SingleInstance();
            builder.RegisterType<TeamsConversationUpdateActivityHandler>().As<ITeamsConversationUpdateActivityHandler>().SingleInstance();
            builder.RegisterType<TeamSpecificConversationState<TeamOperationHistory>>().As<IMiddleware>().SingleInstance();

            builder.RegisterType<TeamsActivityProcessor>().As<IActivityProcessor>().SingleInstance();
            builder.RegisterType<CosmosDbStorage>().As<IStorage>().SingleInstance();

            builder.Register<BotFrameworkAdapter>((context) =>
            {
                IEnumerable<IMiddleware> middlewares = context.Resolve<IEnumerable<IMiddleware>>();
                ICredentialProvider credentialProvider = context.Resolve<ICredentialProvider>();

                BotFrameworkAdapter botFrameworkAdapter = new BotFrameworkAdapter(credentialProvider);

                foreach (IMiddleware middleware in middlewares)
                {
                    botFrameworkAdapter.Use(middleware);
                }

                return botFrameworkAdapter;
            }).SingleInstance();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
        }
    }
}
