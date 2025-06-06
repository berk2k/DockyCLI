using DockyCLI.Services;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var success = _dockerService.StopContainer(settings.ContainerId);
            return success ? 0 : -1;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public string ContainerId { get; set; }
        }


    }
}
