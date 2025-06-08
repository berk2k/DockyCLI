using DockyCLI.Presantation;
using Docky.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;


namespace DockyCLI.Commands
{
    public class ListImagesCommand : Command<ListImagesCommand.Settings>
    {
        private readonly IDockerService _dockerService;
        private readonly OutputRenderer _renderer;

        public ListImagesCommand(IDockerService dockerService, OutputRenderer renderer)
        {
            _dockerService = dockerService;
            _renderer = renderer;
        }

        public class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                var images = _dockerService.GetImages();

                if (images.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No Docker images found.[/]");
                    return 0;
                }

                _renderer.RenderImageTable(images);
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
