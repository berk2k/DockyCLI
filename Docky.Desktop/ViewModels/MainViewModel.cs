using Docky.Core.Models;
using Docky.Core.Services;
using Docky.Desktop.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Docky.Desktop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDockerService _dockerService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ContainerInfo> Containers { get; set; }
        public ObservableCollection<ImageInfo> Images { get; set; }

        public ICommand StopContainerCommand { get; }
        public ICommand StartContainerCommand { get; }
        public ICommand PullNewImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public string ImageNameToPull { get; set; }



        public MainViewModel(IDockerService dockerService)
        {
            _dockerService = dockerService;
            Containers = new ObservableCollection<ContainerInfo>();
            Images = new ObservableCollection<ImageInfo>();
            StopContainerCommand = new RelayCommand(StopContainer);
            StartContainerCommand = new RelayCommand(StartContainer);
            PullNewImageCommand = new RelayCommand(PullNewImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
            ImageNameToPull = string.Empty;

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


        private void PullNewImage(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(ImageNameToPull))
                return;

            var (Success, Output, Error) = _dockerService.PullImage(ImageNameToPull);

            if (Success)
                System.Windows.MessageBox.Show($"Image pulled: {ImageNameToPull}", "Success");
            else
                System.Windows.MessageBox.Show($"Failed to pull image: {Error}", "Error");

            ReloadImages();
            ImageNameToPull = string.Empty;
        }

        private void RemoveImage(object? parameter)
        {
            if (parameter is not ImageInfo image)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to remove image: {image.Repository}:{image.Tag}?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;

            var (Success, Output, Error) = _dockerService.RemoveImage(image.ImageId);

            if (Success)
                System.Windows.MessageBox.Show($"Image removed: {image.Repository}:{image.Tag}", "Success");
            else
                System.Windows.MessageBox.Show($"Failed to remove image: {Error}", "Error");

            ReloadImages();
        }

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
