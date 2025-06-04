using Spectre.Console.Cli;

using System;
using System.ComponentModel;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("docky");
    config.AddCommand<DockyCLI.Commands.ListContainersCommand>("list")
        .WithDescription("Listing Docker Containers");
});

return app.Run(args);

