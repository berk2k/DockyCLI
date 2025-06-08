using Docky.Core.Services;
using DockyCLI.Presantation;
using Spectre.Console.Cli;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Commands
{
    public class ListAllContainersCommand : Command<ListAllContainersCommand.Settings>
    {
        private readonly IDockerService _dockerService;
        private readonly OutputRenderer _renderer;

        public ListAllContainersCommand(IDockerService dockerService, OutputRenderer renderer)
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
