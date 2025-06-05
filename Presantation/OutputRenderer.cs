using DockyCLI.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Presantation
{
    public class OutputRenderer
    {
        public void RenderContainerTable(List<ContainerInfo> containers)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold underline green]Running Containers[/]")
                .AddColumn("ID")
                .AddColumn("Image")
                .AddColumn("Command")
                .AddColumn("Created")
                .AddColumn("Status")
                .AddColumn("Ports")
                .AddColumn("Names");

            foreach (var container in containers)
            {
                table.AddRow(
                    Shorten(container.ContainerId),
                    container.Image,
                    TrimQuotes(container.Command),
                    container.Created,
                    container.Status,
                    string.IsNullOrWhiteSpace(container.Ports) ? "-" : container.Ports,
                    container.Names
                );
            }

            AnsiConsole.Write(table);
        }

        private string Shorten(string text, int length = 12)
        {
            return text.Length > length ? text.Substring(0, length) : text;
        }

        private string TrimQuotes(string text)
        {
            return text.Trim('"');
        }
        public void RenderImageTable(List<ImageInfo> images)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold underline green]Docker Images[/]")
                .AddColumn("Repository")
                .AddColumn("Tag")
                .AddColumn("Image ID")
                .AddColumn("Created")
                .AddColumn("Size");

            foreach (var image in images)
            {
                table.AddRow(
                    image.Repository,
                    image.Tag,
                    Shorten(image.ImageId),
                    image.Created,
                    image.Size
                );
            }

            AnsiConsole.Write(table);
        }
    }


}
