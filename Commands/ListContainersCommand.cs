using DockyCLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Commands
{
    public class ListContainersCommand : Command<ListContainersCommand.Settings>
    {
        private readonly IDockerService _dockerService;
        public ListContainersCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }
        public class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings)
        {
            var (output, error) = _dockerService.RunDockerCommand("ps");

            if (!string.IsNullOrWhiteSpace(error))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {error}");
                return -1;
            }

            AnsiConsole.MarkupLine("[green]Active Docker containers:[/]");
            AnsiConsole.WriteLine(output);
            return 0;
        }
    }
}
