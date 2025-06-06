using DockyCLI.Services;
using Spectre.Console.Cli;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Commands
{
    public class RestartContainerCommand : Command<RestartContainerCommand.Settings>
    {
        private readonly IDockerService _dockerService;

        public RestartContainerCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            [Description("The ID or name of the container")]
            public string ContainerId { get; set; } = string.Empty;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                _dockerService.RestartContainer(settings.ContainerId);
                AnsiConsole.MarkupLine($"[green]Container {settings.ContainerId} restarted successfully.[/]");
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                return -1;
            }
        }
    }
}
