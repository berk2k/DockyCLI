using Docky.Core.Models;
using Docky.Core.Services;
using Docky.Desktop.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Docky.Desktop.ViewModels
{
    public class MainViewModel
    {
        private readonly IDockerService _dockerService;

        public ObservableCollection<ContainerInfo> Containers { get; set; }
        public ObservableCollection<ImageInfo> Images { get; set; }

        public ICommand StopContainerCommand { get; }
        public ICommand StartContainerCommand { get; }



        public MainViewModel(IDockerService dockerService)
        {
            _dockerService = dockerService;
            Containers = new ObservableCollection<ContainerInfo>();
            Images = new ObservableCollection<ImageInfo>();
            StopContainerCommand = new RelayCommand(StopContainer);
            StartContainerCommand = new RelayCommand(StartContainer);

            LoadContainers();
            LoadImages();
        }

        private void LoadContainers()
        {
            Containers.Clear();
            var containers = _dockerService.GetAllContainers();
            foreach (var container in containers)
                Containers.Add(container);
        }

        private void LoadImages()
        {
            Images.Clear();
            var images = _dockerService.GetImages();
            foreach (var image in images)
                Images.Add(image);
        }

        public void ReloadContainers() => LoadContainers();
        public void ReloadImages() => LoadImages();

        private void StopContainer(object? parameter)
        {
            if (parameter is not ContainerInfo container)
                return;

            (bool Success, string Output, string Error) = _dockerService.StopContainer(container.ContainerId);

            if (Success)
                System.Windows.MessageBox.Show($"Container stopped: {container.ContainerId}", "Success");
            else
                System.Windows.MessageBox.Show($"Failed to stop container: {container.ContainerId}", "Error");

            ReloadContainers();
        }

        private void StartContainer(object? parameter)
        {
            if (parameter is not ContainerInfo container)
                return;
            (bool Success, string Output, string Error) = _dockerService.StartContainer(container.ContainerId);

            if (Success)
                System.Windows.MessageBox.Show($"Container started: {container.ContainerId}", "Success");
            else
                System.Windows.MessageBox.Show($"Failed to start container: {container.ContainerId}", "Error");

            ReloadContainers();
        }
    }
}
