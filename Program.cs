using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using DockyCLI.Infrastructure;
using DockyCLI.Commands;
using DockyCLI.Services;



var services = new ServiceCollection();
services.AddSingleton<IDockerService, DockerService>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("docky");
    config.AddCommand<ListContainersCommand>("list")
        .WithDescription("Listing Docker Containers");
});

return app.Run(args);

