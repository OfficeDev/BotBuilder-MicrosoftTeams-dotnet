### Microsoft.Bot.Builder.Teams.AuditBot

This sample show cases the handling of different conversation updates and calling into TeamsExtension to get the required information.

##### Setup
###### Startup.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following 3 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **TeamsMiddleware** - To access Microsoft Teams's constructs
- **DropChatActivitiesMiddleware** - To drop all messages coming from 1on1 and Group Chats

To create a distinction of how messages are handled differently from other events we are going to register different classes for handling different types of activities

```cs
// One central router to call into appropriate handler based on activity type.
services.AddTransient<IActivityProcessor, TeamsActivityProcessor>();

// Handler for conversation updates.
services.AddTransient<ITeamsConversationUpdateActivityHandler, TeamsConversationUpdateActivityHandler>();

// Handler for messages sent by user.
services.AddTransient<IMessageActivityHandler, MessageActivityHandler>();
```

Additionall we setup storage and storage accessor to store events in. Sample uses InMemory storage but for production purposes, use persistent storage like Azure CosmosDB.

```cs
services.AddBot<AuditBot>(options =>
{
    IStorage dataStore = new MemoryStorage();

......
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
    var accessors = new AuditLogAccessor(conversationState)
    {
        AuditLog = conversationState.CreateProperty<TeamOperationHistory>(AuditLogAccessor.AuditLogName),
    };

    return accessors;
});
```

##### Code execution
###### TeamsConversationUpdateActivityHandler
This class handles individual events (conversation updates) and stores them onto storage. For handling of each individual event type refer to ```TeamsActivityProcessor``` as well.

###### MessageActivityHandler
This class handles the messages sent by the user and access information from the storage to tell user which operations were taken on the 

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and use your bot Id.