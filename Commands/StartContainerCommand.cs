using Spectre.Console.Cli;
using DockyCLI.Services;

namespace DockyCLI.Commands
{
    public class StartContainerCommand : Command<StartContainerCommand.Settings>
    {
        private readonly IDockerService _dockerService;

        public StartContainerCommand(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public string ContainerId { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var success = _dockerService.StartContainer(settings.ContainerId);
            return success ? 0 : -1;
        }
    }
}
