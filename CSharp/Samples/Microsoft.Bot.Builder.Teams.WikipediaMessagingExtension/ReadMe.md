### Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension

This sample showcases how to build a Messaging extension with new v4 SDK.

##### Setup
###### WebApiConfig.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following 3 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **TeamsMiddleware** - To access Microsoft Teams's constructs

Optionally to simplify handling Messaging extensions only we are utilizing following 3 constructs

```cs
// Process all incoming activities and figure out which ones are for invoke activities
services.AddTransient<IActivityProcessor, TeamsActivityProcessor>();

// Process all invokes and then figure out the ones which have messaging extension payload.
services.AddTransient<ITeamsInvokeActivityHandler, TeamsInvokeActivityHandler>();

// Call into Wikipedia and get the required details.
services.AddSingleton<ISearchHandler, WikipediaSearchHandler>();
```

##### Code execution
###### WikipediaSearchHandler
Handles contacting Wikipedia and gives back a Messaging extension result.

###### TeamsInvokeActivityHandler
Handles incoming invoke activities and sends back responses. Invoke handler returns an ```InvokeResponse``` object
```cs
return new InvokeResponse
{
    Body = new MessagingExtensionResponse
    {
        ComposeExtension = await this.searchHandler.GetSearchResultAsync(messagingExtensionAction).ConfigureAwait(false),
    },
    Status = 200,
};
```
this is then sent to ```TeamsActivityProcessor``` which then sends it over the wire
```cs
await turnContext.SendActivityAsync(
    new Activity
    {
        Value = invokeResponse,
        Type = ActivityTypesEx.InvokeResponse,
    }).ConfigureAwait(false);
```

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and use your bot Id.