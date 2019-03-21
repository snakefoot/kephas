param (
    [string]$version = $( Read-Host "Please provide package version" )
)

$packages = @(
    "Kephas.Core",
    "Kephas.CodeAnalysis",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.Serilog",
    "Kephas.Composition.Mef",
    "Kephas.Messaging",
    "Kephas.Messaging.Model",
    "Kephas.Model",
    "Kephas.Data",
    "Kephas.Data.Client",
    "Kephas.Data.Entity",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Data.IO",
    "Kephas.Data.Endpoints",
    "Kephas.Serialization.Json",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Npgsql",
    "Kephas.ServiceStack",
    "Kephas.Serialization.ServiceStack.Text",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Orchestration",
    "Kephas.AspNetCore"
    "Kephas.Testing.Composition.Mef"
    "Kephas.Testing.Model"
)

foreach ($package in $packages) {
    .\NuGet.exe push "$package.$version.nupkg" -Source https://api.nuget.org/v3/index.json 
    .\NuGet.exe push "$package.$version.symbols.nupkg" -Source https://nuget.smbsrc.net
}