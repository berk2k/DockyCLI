﻿using Docky.Core.Models;


namespace Docky.Core.Services
{
    public interface IDockerService
    {
        List<ImageInfo> GetImages();
        List<ContainerInfo> GetRunningContainers();

        List<ContainerInfo> GetAllContainers();
        (bool Success, string Output, string Error) StartContainer(string containerId);
        (bool Success, string Output, string Error) StopContainer(string containerId);
        (bool Success, string Output, string Error) RemoveContainer(string containerId);
        (bool Success, string Output, string Error) RestartContainer(string containerId);
        (bool Success, string Output, string Error) GetContainerLogs(string containerId);
        (bool Success, string Output, string Error) PullImage(string imageName);
        (bool Success, string Output, string Error) RemoveImage(string imageId);
        (bool Success, string Output, string Error) CreateContainer(ContainerCreateParams parameters);
    }

}
