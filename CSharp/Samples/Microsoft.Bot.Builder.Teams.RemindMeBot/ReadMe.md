### Microsoft.Bot.Builder.Teams.RemindMeBot

This sample show cases how to proactively create conversation and send messages to them.

##### Setup
###### Startup.cs
Startup.cs sets up the bot. Since this bot only works in Channels in Microsoft Teams, we will be adding following 3 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **DropChannelActivitiesMiddleware** - To drop all messages coming from 1on1 and Group Chats

To process incoming text we have added a custom text recognizer ```ReminderTextRecognizer```
```cs
services.AddTransient<IRecognizer, ReminderTextRecognizer>();
```

We also have registered a class to handle proactive scenarios for us.

```cs
services.AddTransient<IProactiveMessageManager, ProactiveMessageManager>();
```

##### Code execution
###### RemindMeBot
Calls into ```IRecognizer``` to get back intents sent by the user and then calls into I```ProactiveMessageManager``` instance to queue a work item which will be executed at a later point.

###### ProactiveMessageManager
Queues a work item, once the timer expires creates a conversation and then send message to it. Since we are already in the context of the same message, we don't need to store the ```botId``` which received the message. For scenarios where messages are sent after a long time ```botId``` might need to be persisted.

#### How to test
Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and use your bot Id.