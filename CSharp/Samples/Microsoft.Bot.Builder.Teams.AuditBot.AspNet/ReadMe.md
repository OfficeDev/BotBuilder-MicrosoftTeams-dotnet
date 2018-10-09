### Microsoft.Bot.Builder.Teams.AuditBot.AspNet

This sample show cases the handling of different conversation updates and calling into TeamsExtension to get the required information.

##### Setup
###### WebApiConfig.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following 3 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **TeamsMiddleware** - To access Microsoft Teams's constructs
- **DropChatActivitiesMiddleware** - To drop all messages coming from 1on1 and Group Chats

To create a distinction of how messages are handled differently from other events we are going to register different classes for handling different types of activities

```cs
// One central router to call into appropriate handler based on activity type.
builder.RegisterType<TeamsActivityProcessor>().As<IActivityProcessor>();

// Handler for conversation updates.
builder.RegisterType<TeamsConversationUpdateActivityHandler>().As<ITeamsConversationUpdateActivityHandler>();

// Handler for messages sent by user.
builder.RegisterType<MessageActivityHandler>().As<IMessageActivityHandler>();
```

Additionall we setup storage and storage accessor to store events in. Sample uses InMemory storage but for production purposes, use persistent storage like Azure CosmosDB.

```cs
// Create Conversation State object.
// The Conversation State object is where we persist anything at the conversation-scope.
TeamSpecificConversationState conversationState = new TeamSpecificConversationState(dataStore);
botConfig.BotFrameworkOptions.State.Add(conversationState);

// Create the custom state accessor.
// State accessors enable other components to read and write individual properties of state.
var accessors = new AuditLogAccessor(conversationState)
{
    AuditLog = conversationState.CreateProperty<TeamOperationHistory>(AuditLogAccessor.AuditLogName),
};

builder.Register<AuditLogAccessor>((component) => accessors);
```

##### Code execution
###### TeamsConversationUpdateActivityHandler
This class handles individual events (conversation updates) and stores them onto storage. For handling of each individual event type refer to ```TeamsActivityProcessor``` as well.

###### MessageActivityHandler
This class handles the messages sent by the user and access information from the storage to tell user which operations were taken on the team and current status of the team.

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and use your bot Id.