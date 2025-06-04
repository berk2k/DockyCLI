using DockyCLI.Models;
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
        public List<ContainerInfo> GetRunningContainers()
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

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
                throw new Exception($"Docker error: {error}");

            return ParseDockerPsOutput(output);
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
    }
}
