using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Docky.Desktop.ViewModels
{
    public class CreateContainerViewModel : INotifyPropertyChanged
    {
        private string _imageName = string.Empty;
        private string _containerName = string.Empty;
        private string _ports = string.Empty;
        private string _environment = string.Empty;
        private string _volumes = string.Empty;
        private string _additionalParams = string.Empty;
        private bool _detachedMode = true;
        private bool _interactiveMode = false;
        private bool _removeOnExit = false;

        public string ImageName
        {
            get => _imageName;
            set
            {
                _imageName = value;
                OnPropertyChanged();
            }
        }

        public string ContainerName
        {
            get => _containerName;
            set
            {
                _containerName = value;
                OnPropertyChanged();
            }
        }

        public string Ports
        {
            get => _ports;
            set
            {
                _ports = value;
                OnPropertyChanged();
            }
        }

        public string Environment
        {
            get => _environment;
            set
            {
                _environment = value;
                OnPropertyChanged();
            }
        }

        public string Volumes
        {
            get => _volumes;
            set
            {
                _volumes = value;
                OnPropertyChanged();
            }
        }

        public string AdditionalParams
        {
            get => _additionalParams;
            set
            {
                _additionalParams = value;
                OnPropertyChanged();
            }
        }

        public bool DetachedMode
        {
            get => _detachedMode;
            set
            {
                _detachedMode = value;
                OnPropertyChanged();
            }
        }

        public bool InteractiveMode
        {
            get => _interactiveMode;
            set
            {
                _interactiveMode = value;
                OnPropertyChanged();
            }
        }

        public bool RemoveOnExit
        {
            get => _removeOnExit;
            set
            {
                _removeOnExit = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
