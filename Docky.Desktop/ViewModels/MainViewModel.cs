using Docky.Core.Models;
using Docky.Core.Services;
using Docky.Desktop.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Docky.Desktop.ViewModels
{
    public class MainViewModel
    {
        private readonly IDockerService _dockerService;

        public ObservableCollection<ContainerInfo> Containers { get; set; }

        public ICommand StopContainerCommand { get; }

        public MainViewModel(IDockerService dockerService)
        {
            _dockerService = dockerService;
            Containers = new ObservableCollection<ContainerInfo>();
            StopContainerCommand = new RelayCommand(StopContainer);

            LoadContainers();
        }

        private void LoadContainers()
        {
            Containers.Clear();
            var containers = _dockerService.GetRunningContainers();
            foreach (var container in containers)
                Containers.Add(container);
        }

        public void ReloadContainers() => LoadContainers();

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
    }
}
