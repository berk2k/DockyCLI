using Docky.Core.Models;
using Docky.Core.Services;
using Docky.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Docky.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDockerService _dockerService;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _dockerService = new DockerService(); 
            _viewModel = new MainViewModel(_dockerService);
            DataContext = _viewModel;
        }

        
    }

}