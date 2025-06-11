using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Docky.Core.Services;
using Docky.Desktop.ViewModels;


namespace Docky.Desktop
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public ServiceProvider Services { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            
        }

        private void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<IDockerService, DockerService>();

            
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
