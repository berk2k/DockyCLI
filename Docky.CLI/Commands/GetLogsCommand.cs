using Docky.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;


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
            var (success, output, error) = _dockerService.GetContainerLogs(settings.ContainerId);

            if (!success)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {error}");
                return -1;
            }

            AnsiConsole.MarkupLine($"[green]Logs for container {settings.ContainerId}:[/]");
            AnsiConsole.WriteLine(output);
            return 0;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public string ContainerId { get; set; } = string.Empty;
        }
    }
}
