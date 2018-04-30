call npm install replace@0.3.0
call npm install autorest@2.0.4262

call autorest README.md --csharp

cd generated
call ..\node_modules\.bin\replace "Microsoft.Bot.Connector.Teams.Models" "Microsoft.Bot.Schema.Teams" . -r --include="*.cs"
call ..\node_modules\.bin\replace "using Models;" "using Microsoft.Bot.Schema.Teams;"  . -r --include="*.cs"
call ..\node_modules\.bin\replace "FromProperty" "From" . -r --include="*.cs"
call ..\node_modules\.bin\replace "fromProperty" "from" . -r --include="*.cs"
cd ..

copy generated\Models\*.* ..\CSharp\Microsoft.Bot.Schema.Teams
move ..\Microsoft.Bot.Schema\ErrorResponseException.cs ..\Microsoft.Bot.Connector
copy generated\*.* ..\CSharp\Microsoft.Bot.Connector.Teams
rd /q /s generated

