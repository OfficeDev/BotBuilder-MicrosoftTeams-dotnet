### Microsoft.Bot.Builder.Teams.FileBot

This sample showcases how to build a demo bot with new v4 SDK to show the example for bot files. The dome includes

1. User to bot file sending
2. Bot to user file sending

##### Setup
###### WebApiConfig.cs
Startup.cs sets up the bot. Since this bot only works in Chats in Microsoft Teams, we will be adding following
 middlewares
- **DropNonTeamsActivitiesMiddleware** - To drop all activities not coming from Microsoft Teams
- **DropChannelActivitiesMiddleware** - To drop all Channel activities as bot file is only working for chats
- **TeamsMiddleware** - To access Microsoft Teams's constructs

Optionally to simplify handling Messaging extensions only we are utilizing following constructs

```cs
// Process all incoming activities and figure out which ones are for invoke activities
services.AddTransient<IActivityProcessor, TeamsActivityProcessor>();

// Process all message activities, including messages with user-to-bot files.
services.AddTransient<IMessageActivityHandler, MessageActivityHandler>();

// Process all invokes.
services.AddTransient<ITeamsInvokeActivityHandler, TeamsInvokeActivityHandler>();
```

#### Code execution
##### User-to-Bot files
When user sends a file to bot, bot will receive a message with file download information that includes the URL to download that file:
```cs
public class MessageActivityHandler : IMessageActivityHandler
{
    public async Task HandleMessageAsync(ITurnContext turnContext)
    {
        bool messageWithFileDownloadInfo = turnContext.Activity.Attachments?[0].ContentType == FileDownloadInfo.ContentType;
        if (messageWithFileDownloadInfo)
        {
            Attachment file = turnContext.Activity.Attachments[0];
            FileDownloadInfo fileDownload = JObject.FromObject(file.Content).ToObject<FileDownloadInfo>();
            // download file from fileDownload.DownloadUrl
        }
    }
}
```

##### Bot-to-User files

1. In message activity handler, you may send a file consent card to ask user to accept the file or not, before bot sends out files:
```cs
public class MessageActivityHandler : IMessageActivityHandler
{
    public async Task HandleMessageAsync(ITurnContext turnContext)
    {
        // prepare file name and file size 
        // and then call SendFileCardAsync() to send out file consent card
        await this.SendFileCardAsync(turnContext, filename, fileSize).ConfigureAwait(false);
    }

    private async Task SendFileCardAsync(ITurnContext turnContext, string filename, long filesize)
    {
        var fileCard = new FileConsentCard
        {
            // prepare a file consent card
        };

        Activity replyActivity = turnContext.Activity.CreateReply();
        replyActivity.Attachments = new List<Attachment>()
        {
            fileCard.ToAttachment(filename),
        };

        await turnContext.SendActivityAsync(replyActivity).ConfigureAwait(false);
    }
}
```

2. When user received the card, he / she can accept or decline it. Then your bot will receive an invoke request:

```cs
public class TeamsInvokeActivityHandler : TeamsInvokeActivityHandlerBase
{
    public override async Task<InvokeResponse> HandleFileConsentResponseAsync(ITurnContext turnContext, FileConsentCardResponse query)
    {
        JToken context = JObject.FromObject(query.Context);
        if (query.Action.Equals("accept"))
        {
            string fileUploadUrl = query.UploadInfo.UploadUrl;
            /*
             * ... upload the file to the endpoint fileUploadUrl through http PUT
             */

            // after file uploading completed, 
            // send a file download card (type of FileInfoCard)
            // to user to guide him/her to download it
            await this.FileUploadCompletedAsync(turnContext, query).ConfigureAwait(false);
        }

        if (query.Action.Equals("decline"))
        {
            // user declined. Bot can not send out file.
        }

        return null;
    }
}
```

#### How to test
- Use the [sample manifest](TeamsAppManifest/manifest.json) and change the botId at --YOURBOTIDHERE-- and appId at --YOURAPPIDHERE-- to use your own bot and app id respectively.
- Note that `bots.supportsFiles` needs to set `true`