using DockyCLI.Presantation;
using Docky.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;


namespace DockyCLI.Commands
{
    public class ListContainersCommand : Command<ListContainersCommand.Settings>
    {
        private readonly IDockerService _dockerService;
        private readonly OutputRenderer _renderer;

        public ListContainersCommand(IDockerService dockerService, OutputRenderer renderer)
        {
            _dockerService = dockerService;
            _renderer = renderer;
        }
        public class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                var containers = _dockerService.GetRunningContainers();

                if (containers.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No running containers found.[/]");
                    return 0;
                }
                _renderer.RenderContainerTable(containers);
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
