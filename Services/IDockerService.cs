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

    }
}
