### Microsoft.Bot.Builder.Teams.TeamsEchoBot

This sample showcases a basic bot which access TeamsExtensions.

##### Setup
###### WebApiConfig.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following 3 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **TeamsMiddleware** - To access Microsoft Teams's constructs

To store turn count we are adding storage. Additionally since we want one counter to be maintained for the whole team, we are adding a conversation state handler which understands the whole Team as once single conversation.

```cs
IStorage dataStore = new MemoryStorage();

// --> Adding conversation state handler which understands a team as single conversation.
options.State.Add(new TeamSpecificConversationState(dataStore));
```

##### Code execution
###### EchoBot
For every message sent, EchoBot stores a turn counter. Every message will show the number of messages received from a particular chat or team.

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and use your bot Id.