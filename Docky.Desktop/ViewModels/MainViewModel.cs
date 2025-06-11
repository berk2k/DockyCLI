using Docky.Core.Models;
using Docky.Core.Services;
using Docky.Desktop.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Docky.Desktop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDockerService _dockerService;
        private string _imageNameToPull = string.Empty;
        private CreateContainerViewModel _createContainer = new();

        public ObservableCollection<ContainerInfo> Containers { get; set; }
        public ObservableCollection<ImageInfo> Images { get; set; }

        public ICommand StopContainerCommand { get; }
        public ICommand StartContainerCommand { get; }
        public ICommand PullNewImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand CreateContainerCommand { get; }
        public ICommand CreateContainerFromFormCommand { get; }

        public string ImageNameToPull
        {
            get => _imageNameToPull;
            set
            {
                _imageNameToPull = value;
                OnPropertyChanged();
            }
        }

        public CreateContainerViewModel CreateContainer
        {
            get => _createContainer;
            set
            {
                _createContainer = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IDockerService dockerService)
        {
            _dockerService = dockerService;
            Containers = new ObservableCollection<ContainerInfo>();
            Images = new ObservableCollection<ImageInfo>();

            StopContainerCommand = new RelayCommand(StopContainer);
            StartContainerCommand = new RelayCommand(StartContainer);
            PullNewImageCommand = new RelayCommand(PullNewImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
            CreateContainerCommand = new RelayCommand(CreateContainerFromImage);
            CreateContainerFromFormCommand = new RelayCommand(CreateContainerFromForm);

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
            {
                System.Windows.MessageBox.Show($"Image pulled: {ImageNameToPull}", "Success");
                CreateContainer.ImageName = ImageNameToPull;
            }
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

        private void CreateContainerFromImage(object? parameter)
        {
            if (parameter is not ImageInfo image)
                return;

            CreateContainer.ImageName = $"{image.Repository}:{image.Tag}";
            CreateContainer.ContainerName = GenerateContainerName(image.Repository);

            System.Windows.MessageBox.Show(
                $"Image '{CreateContainer.ImageName}' loaded into the create container form. Please configure other settings and click 'Create Container'.",
                "Ready to Create Container",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        private void CreateContainerFromForm(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(CreateContainer.ImageName))
            {
                System.Windows.MessageBox.Show("Please specify an image name.", "Validation Error");
                return;
            }

            var containerParams = new ContainerCreateParams
            {
                ImageName = CreateContainer.ImageName,
                ContainerName = CreateContainer.ContainerName,
                Ports = CreateContainer.Ports,
                Environment = CreateContainer.Environment,
                Volumes = CreateContainer.Volumes,
                DetachedMode = CreateContainer.DetachedMode,
                InteractiveMode = CreateContainer.InteractiveMode,
                RemoveOnExit = CreateContainer.RemoveOnExit,
                AdditionalParams = CreateContainer.AdditionalParams
            };

            var (Success, Output, Error) = _dockerService.CreateContainer(containerParams);

            if (Success)
            {
                System.Windows.MessageBox.Show($"Container created successfully: {CreateContainer.ContainerName}", "Success");
                ReloadContainers();
                
                CreateContainer = new CreateContainerViewModel();
            }
            else
            {
                System.Windows.MessageBox.Show($"Failed to create container: {Error}", "Error");
            }
        }

        private string GenerateContainerName(string imageName)
        {
            var baseName = imageName.Split('/').Last().Split(':').First();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{baseName}_{timestamp}";
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
