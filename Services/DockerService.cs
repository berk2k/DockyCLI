using DockyCLI.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DockyCLI.Services
{
    public class DockerService : IDockerService
    {
        // helper method
        private List<T> RunDockerCommandAndParse<T>(string arguments, Func<string, List<T>> parseFunc)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
                throw new Exception($"Docker error: {error}");

            return parseFunc(output);
        }


        public List<ImageInfo> GetImages()
        {
            return RunDockerCommandAndParse("images", ParseDockerImagesOutput);
        }

        public List<ContainerInfo> GetRunningContainers()
        {
            return RunDockerCommandAndParse("ps", ParseDockerPsOutput);
        }

        private List<ContainerInfo> ParseDockerPsOutput(string output)
        {
            var lines = output.Split('\n',StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                return new List<ContainerInfo>();

            var containers = new List<ContainerInfo>();

            foreach (var line in lines.Skip(1))
            {
                var parts = Regex.Split(line.Trim(), @"\s{2,}");
                if (parts.Length < 7)
                    continue;

                containers.Add(new ContainerInfo
                {
                    ContainerId = parts[0],
                    Image = parts[1],
                    Command = parts[2],
                    Created = parts[3],
                    Status = parts[4],
                    Ports = parts[5],
                    Names = parts[6]
                });
            }

            return containers;
        }

        private List<ImageInfo> ParseDockerImagesOutput(string output)
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                return new List<ImageInfo>();

            var images = new List<ImageInfo>();

            foreach (var line in lines.Skip(1))
            {
                var parts = Regex.Split(line.Trim(), @"\s{2,}");
                if (parts.Length < 5)
                    continue;

                images.Add(new ImageInfo
                {
                    Repository = parts[0],
                    Tag = parts[1],
                    ImageId = parts[2],
                    Created = parts[3],
                    Size = parts[4]
                });
            }

            return images;
        }

        public (bool success, string output, string error) RunDockerCommand(string subCommand, string targetId)
        {
            var arguments = $"{subCommand} {targetId}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (string.IsNullOrWhiteSpace(error), output.Trim(), error.Trim());
        }

        public bool StartContainer(string containerId)
        {
            var (success, output, error) = RunDockerCommand("start", containerId);

            if (!success)
                AnsiConsole.MarkupLine($"[red]Docker error:[/] {error}");
            else
                AnsiConsole.MarkupLine($"[green]Container started:[/] {output}");

            return success;
        }

        public bool StopContainer(string containerId)
        {
            var (success, output, error) = RunDockerCommand("stop", containerId);

            if (!success)
                AnsiConsole.MarkupLine($"[red]Docker error:[/] {error}");
            else
                AnsiConsole.MarkupLine($"[green]Container stopped:[/] {output}");

            return success;
        }

        public string GetContainerLogs(string containerId)
        {
            var (success, output, error) = RunDockerCommand("logs", containerId);

            if (!success)
            {
                throw new Exception($"Docker error while fetching logs: {error}");
            }

            return output;
        }

        public bool RestartContainer(string containerId)
        {
            var (success, output, error) = RunDockerCommand("restart", containerId);

            if (!success)
            {
                throw new Exception($"Docker error while restarting container: {error}");
            }

            return true;
        }


    }
}
