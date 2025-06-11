using Docky.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Docky.Core.Services
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

        public List<ContainerInfo> GetAllContainers()
        {
            return RunDockerCommandAndParse("ps -a --format \"{{.ID}}|{{.Image}}|{{.Command}}|{{.CreatedAt}}|{{.Status}}|{{.Ports}}|{{.Names}}\"", ParseDockerPsFormatOutput);
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

        private List<ContainerInfo> ParseDockerPsFormatOutput(string output)
        {
            var containers = new List<ContainerInfo>();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 7)
                {
                    containers.Add(new ContainerInfo
                    {
                        ContainerId = parts[0],
                        Image = parts[1],
                        Command = parts[2].Trim('"'),
                        Created = parts[3],
                        Status = parts[4],
                        Ports = parts[5],
                        Names = parts[6]
                    });
                }
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

        private (bool Success, string Output, string Error) ExecuteDockerRunCommand(List<string> arguments)
        {
            var argumentsString = string.Join(" ", arguments.Select(arg =>
                arg.Contains(' ') && !arg.StartsWith("\"") ? $"\"{arg}\"" : arg));

            var fullArguments = $"run {argumentsString}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = fullArguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                var success = process.ExitCode == 0 && string.IsNullOrWhiteSpace(error);
                return (success, output.Trim(), error.Trim());
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Process execution error: {ex.Message}");
            }
            finally
            {
                process?.Dispose();
            }
        }

        public (bool Success, string Output, string Error) StartContainer(string containerId)
        {
            return RunDockerCommand("start", containerId);
        }


        public (bool Success, string Output, string Error) StopContainer(string containerId)
        {
            return RunDockerCommand("stop", containerId);
        }


        public (bool Success, string Output, string Error) GetContainerLogs(string containerId)
        {
            return RunDockerCommand("logs", containerId);
        }


        public (bool Success, string Output, string Error) RestartContainer(string containerId)
        {
            return RunDockerCommand("restart", containerId);
        }

        public (bool Success, string Output, string Error) PullImage(string imageName)
        {
            return RunDockerCommand("pull", imageName);
        }

        public (bool Success, string Output, string Error) RemoveImage(string imageId)
        {
            return RunDockerCommand("rmi", imageId);
        }

        public (bool Success, string Output, string Error) CreateContainer(ContainerCreateParams parameters)
        {
            try
            {
                var arguments = new List<string>();

                
                if (parameters.DetachedMode)
                    arguments.Add("-d");

                
                if (parameters.InteractiveMode)
                    arguments.Add("-it");

                
                if (parameters.RemoveOnExit)
                    arguments.Add("--rm");

                
                if (!string.IsNullOrWhiteSpace(parameters.ContainerName))
                {
                    arguments.Add("--name");
                    arguments.Add(parameters.ContainerName);
                }

                if (!string.IsNullOrWhiteSpace(parameters.Ports))
                {
                    var ports = parameters.Ports.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var port in ports)
                    {
                        arguments.Add("-p");
                        arguments.Add(port.Trim());
                    }
                }

                
                if (!string.IsNullOrWhiteSpace(parameters.Environment))
                {
                    var envVars = parameters.Environment.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var envVar in envVars)
                    {
                        arguments.Add("-e");
                        arguments.Add(envVar.Trim());
                    }
                }

                
                if (!string.IsNullOrWhiteSpace(parameters.Volumes))
                {
                    var volumes = parameters.Volumes.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var volume in volumes)
                    {
                        arguments.Add("-v");
                        arguments.Add(volume.Trim());
                    }
                }

                
                if (!string.IsNullOrWhiteSpace(parameters.AdditionalParams))
                {
                    var additionalParams = parameters.AdditionalParams.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    arguments.AddRange(additionalParams);
                }

               
                arguments.Add(parameters.ImageName);

                
                return ExecuteDockerRunCommand(arguments);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Error creating container: {ex.Message}");
            }
        }

    }
}
