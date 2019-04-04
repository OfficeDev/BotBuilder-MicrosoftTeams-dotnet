### Microsoft.Bot.Builder.Teams.MessagingExtensionBot

This sample showcases how to build a Messaging extension demo bot with new v4 SDK. The dome includes

1. Adaptive card
2. Wrapping BotBuilder actions into an adaptive card
3. Task module:
    1. prepare card action to launch task module on cards.
    2. handle task module fetch / submit.
4. Messaging extension - search query
5. Messaging extension - create flow
6. Messaging extension - message action (sharing)
7. Messaging extension - bot message preview
8. Messaging extension - app-based link query

##### Setup
###### WebApiConfig.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following
 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **TeamsMiddleware** - To access Microsoft Teams's constructs

Optionally to simplify handling Messaging extensions only we are utilizing following constructs

```cs
// Process all incoming activities and figure out which ones are for invoke activities
services.AddTransient<IActivityProcessor, TeamsActivityProcessor>();

// Process all message activities to send out cards.
services.AddTransient<IMessageActivityHandler, MessageActivityHandler>();

// Process all invokes and then figure out the ones which have messaging extension payload.
services.AddTransient<ITeamsInvokeActivityHandler, TeamsInvokeActivityHandler>();

// Call into Wikipedia and get the required details.
services.AddSingleton<ISearchHandler, WikipediaSearchHandler>();
```

#### Code execution
##### MessageActivityHandler.cs
This file handles all messages through
```cs
public async Task HandleMessageAsync(ITurnContext turnContext)
{
    // ...
}
```
If user types "cards" then it'll send out 3 demo cards, which are
1. Demo card 1 - an adaptive card that wraps a BotBuilder `CardAction` into adaptive card submit action through calling `.ToAdaptiveCardAction()`
2. Demo card 2 - an adaptive card that includes a card button of `TaskModuleAction` to launch task module (through calling `.ToAdaptiveCardAction()` since `TaskModuleAction` inerits from BotBuilder `CardAction`)
3. Demo card 3 - an hero card that includes a card button of `TaskModuleAction` to launch task module

##### TeamsInvokeActivityHandler.cs
This file handles all invoke activities that include:

1. Regular messaging extension search query through 
```cs
public override async Task<InvokeResponse> HandleMessagingExtensionQueryAsync(ITurnContext turnContext, MessagingExtensionQuery query) { ... }
```

2. Handle invoke request for messaging extension actions such as fetch task and submit action through the following handlers respectively:
```cs
public override async Task<InvokeResponse> HandleMessagingExtensionFetchTaskAsync(ITurnContext turnContext, MessagingExtensionAction query) { ... }
```
```cs
public override async Task<InvokeResponse> HandleMessagingExtensionSubmitActionAsync(ITurnContext turnContext, MessagingExtensionAction query) { ... }
``` 

3. Handle task module request for fetch and submit, through the following handlers respectively:
```cs
public override async Task<InvokeResponse> HandleTaskModuleFetchAsync(ITurnContext turnContext, TaskModuleRequest query) { ... }
```
```cs
public override async Task<InvokeResponse> HandleTaskModuleSubmitAsync(ITurnContext turnContext, TaskModuleRequest query) { ... }
```

4. App-based link query to generate app-specific previews for the URLs requested by users. It's done by this handler:
```cs
public override async Task<InvokeResponse> HandleAppBasedLinkQueryAsync(ITurnContext turnContext, AppBasedLinkQuery query) { ... }
```

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and appId at --YOURAPPIDHERE-- to use your own bot and app id respectively.