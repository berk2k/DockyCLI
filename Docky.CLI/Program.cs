﻿using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using DockyCLI.Infrastructure;
using DockyCLI.Commands;
using Docky.Core.Services;
using DockyCLI.Presantation;



var services = new ServiceCollection();
services.AddSingleton<IDockerService, DockerService>();
services.AddSingleton<OutputRenderer>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("docky");
    config.AddCommand<ListRunningContainersCommand>("list")
        .WithDescription("Listing Running Docker Containers");
    config.AddCommand<ListAllContainersCommand>("list all")
        .WithDescription("Listing Docker Containers");
    config.AddCommand<ListImagesCommand>("images")
        .WithDescription("Listing Docker Images");
    config.AddCommand<StartContainersCommand>("start")
        .WithDescription("Starting Docker Container");
    config.AddCommand<StopContainersCommand>("stop")
        .WithDescription("Stopping Docker Container");
    config.AddCommand<GetLogsCommand>("logs")
        .WithDescription("Getting Docker Container logs");
    config.AddCommand<RestartContainerCommand>("restart")
        .WithDescription("Restarting Docker Container");
});

return app.Run(args);

