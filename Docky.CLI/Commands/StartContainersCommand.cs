using Spectre.Console.Cli;
using Docky.Core.Services;
using Spectre.Console;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DockyCLI.Commands
{
    public class StartContainersCommand : Command<StartContainersCommand.Settings>
    {
        private readonly IDockerService _dockerService;

        public StartContainersCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public string ContainerId { get; set; } = string.Empty;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            (bool Success, string Output, string Error) = _dockerService.StartContainer(settings.ContainerId);
            if (Success)
            {
                AnsiConsole.MarkupLine($"[green]Container started successfully:[/] {Output}");
                return 0;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to start container:[/] {Error}");
                return -1;
            }
            
        }
    }
}
