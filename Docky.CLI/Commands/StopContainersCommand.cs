using Docky.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;


namespace DockyCLI.Commands
{
    public class StopContainersCommand : Command<StopContainersCommand.Settings>
    {
        private readonly IDockerService _dockerService;

        public StopContainersCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            (bool Success, string Output, string Error) = _dockerService.StopContainer(settings.ContainerId);
            if (Success)
            {
                AnsiConsole.MarkupLine($"[green]Container stopped successfully:[/] {Output}");
                return 0;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to stop container:[/] {Error}");
                return -1;
            }
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public string ContainerId { get; set; } = string.Empty;
        }


    }
}
