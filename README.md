
# This functionality is moving into the core Bot Framework SDK

We are migrating the functionality of this SDK into the core Bot Framework SDK, and are targeting the 4.6 release (early November 2019). Please see our [early example code](https://github.com/microsoft/botbuilder-dotnet/tree/master/tests/Teams) for an early look at the new, improved, way easier to use, SDK!


# Bot Builder SDK 4 - Microsoft Teams Extensions

The Microsoft Bot Builder SDK 4 Teams Extensions allow you to build bots for Microsoft Teams quickly and easily. **[Review the documentation](https://msdn.microsoft.com/en-us/microsoft-teams/bots)** to get started!

## This SDK allows you to easily...

* Fetch a list of channels in a team
* Fetch profile info about all members of a team
* Fetch tenant-id from an incoming message to bot
* Create 1:1 chat with a specific user
* Mention a specific user
* Consume various events like channel-created, team-renamed, etc.
* Accept messages only from specific tenants
* Write Compose Extensions
* _and more!_

## Installing

Simply grab the [Microsoft.Bot.Builder.Teams](https://www.nuget.org/packages/Microsoft.Bot.Builder.Teams) nuget.

Bot Builder SDK 4 - Microsoft Teams extensions for Node is available at https://github.com/OfficeDev/BotBuilder-MicrosoftTeams-node.

## Getting started

* If you don't already have it, install the Visual Studio [project template for Bot Framework V4 bot](https://marketplace.visualstudio.com/items?itemName=BotBuilder.botbuilderv4).
* Add a reference to `Microsoft.Bot.Builder.Teams` nuget package.
* Go to `Startup.cs` and add the following snippet of code:
```csharp
            services.AddBot<EchoBot>(options =>
            {
                // ... other stuff snipped for brevity

                // Add Teams Middleware.
                options.Middleware.Add(
                    new TeamsMiddleware(
                        new ConfigurationCredentialProvider(this.Configuration)));

                // ... other stuff snipped for brevity
            });
```
* Now in the `OnTurnAsync` method of your bot, to do any Teams specific stuff, first grab the ITeamsContext as shown below:
```csharp
           var teamsContext = turnContext.TurnState.Get<ITeamsContext>();
```
* And once you have `teamsContext`, you can use intellisense built into Visual Studio to discover all the operations you can do. For instance, here's how you can fetch the list of channels in the team and fetch information about the team:
```csharp
// Now fetch the Team ID, Channel ID, and Tenant ID off of the incoming activity
var incomingTeamId = teamsContext.Team.Id;
var incomingChannelid = teamsContext.Channel.Id;
var incomingTenantId = teamsContext.Tenant.Id;

// Make an operation call to fetch the list of channels in the team, and print count of channels.
var channels = await teamsContext.Operations.FetchChannelListAsync(incomingTeamId);
await turnContext.SendActivityAsync($"You have {channels.Conversations.Count} channels in this team");

// Make an operation call to fetch details of the team where the activity was posted, and print it.
var teamInfo = await teamsContext.Operations.FetchTeamDetailsAsync(incomingTeamId);
await turnContext.SendActivityAsync($"Name of this team is {teamInfo.Name} and group-id is {teamInfo.AadGroupId}");
```

## Samples:
Take a look [here](CSharp/Samples).

Stand-alone sample can be found [here](https://github.com/OfficeDev/msteams-samples-dotnet-echobot-bf4).

## Building:
-  Install latest NodeJS from [here](https://nodejs.org/en/download/)
-  Install Visual Studio 2017 or later

### Updating Swagger spec
If you have updated the TeamsAPI.json. You will need to regenerate the client models
- Delete the [Generated Models](CSharp/Microsoft.Bot.Schema.Teams/Generated)
- Run [client model generation script](Swagger/generateclient.cmd)

### Building the solution
- Open the [Solution](CSharp/Microsoft.Bot.Builder.Teams.sln) in Visual Studio
- Build the solution

## Questions, bugs, feature requests, and contributions
Please review the information [here](https://msdn.microsoft.com/en-us/microsoft-teams/feedback).

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
