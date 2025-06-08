using Docky.Core.Models;


namespace Docky.Core.Services
{
    public interface IDockerService
    {
        List<ImageInfo> GetImages();
        List<ContainerInfo> GetRunningContainers();
        (bool Success, string Output, string Error) StartContainer(string containerId);
        (bool Success, string Output, string Error) StopContainer(string containerId);
        (bool Success, string Output, string Error) RestartContainer(string containerId);
        (bool Success, string Output, string Error) GetContainerLogs(string containerId);
    }

}
