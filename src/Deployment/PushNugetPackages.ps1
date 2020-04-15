param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release"
)

$packages = @(
    "Kephas.Application",
    "Kephas.Application.AspNetCore",
    "Kephas.Application.Console",
    "Kephas.CodeAnalysis",
    "Kephas.Collections",
    "Kephas.Composition.Autofac",
    "Kephas.Composition.Mef",
    "Kephas.Core",
    "Kephas.Core.Endpoints",
    "Kephas.Data",
    "Kephas.Data.Client",
    "Kephas.Data.Endpoints",
    "Kephas.Data.IO",
    "Kephas.Data.LLBLGen",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Extensions.Configuration",
    "Kephas.Extensions.DependencyInjection",
    "Kephas.Extensions.Hosting",
    "Kephas.Extensions.Logging",
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Serilog",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Messaging",
    "Kephas.Messaging.Model",
    "Kephas.Messaging.Redis",
    "Kephas.Model",
    "Kephas.Npgsql",
    "Kephas.Orchestration",
    "Kephas.Plugins",
    "Kephas.Plugins.Endpoints",
    "Kephas.Plugins.NuGet",
    "Kephas.Redis",
    "Kephas.Scheduling",
#    "Kephas.Scheduling.Quartz",
#    "Kephas.Scheduling.Quartz.MongoDB",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Scripting.Lua",
    "Kephas.Scripting.Python",
    "Kephas.Serialization.NewtonsoftJson",
    "Kephas.TextProcessing",
    "Kephas.Workflow",
    "Kephas.Workflow.Model",
    "Kephas.Testing",
    "Kephas.Testing.Composition.Autofac",
    "Kephas.Testing.Composition.Mef",
    "Kephas.Testing.Model"
)

foreach ($package in $packages) {
    $packagepath = "..\$package\bin\$build\$package.$version.nupkg"
    .\NuGet.exe push $packagepath -Source https://api.nuget.org/v3/index.json 
}