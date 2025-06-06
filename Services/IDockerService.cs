using DockyCLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Services
{
    public interface IDockerService
    {
        List<ContainerInfo> GetRunningContainers();
        List<ImageInfo> GetImages();
        bool StartContainer(string containerId);

        bool StopContainer(string containerId);

        public string GetContainerLogs(string containerId);

        public bool RestartContainer(string containerId);

    }
}
