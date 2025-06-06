using DockyCLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Commands
{
    public class GetLogsCommand : Command<GetLogsCommand.Settings>
    {
        private readonly IDockerService _dockerService;

        public GetLogsCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                var logs = _dockerService.GetContainerLogs(settings.ContainerId);
                AnsiConsole.MarkupLine($"[green]Logs for container {settings.ContainerId}:[/]");
                AnsiConsole.WriteLine(logs);
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
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
