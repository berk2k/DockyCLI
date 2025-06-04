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
        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "docker",
                        Arguments = "ps",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {error}");
                    return -1;
                }
                AnsiConsole.MarkupLine("[green]Active Docker containers:[/]");
                AnsiConsole.WriteLine(output);

                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Exception:[/] {ex.Message}");
                return -1;
            }
        }

        public class Settings : CommandSettings { }
    }
}
