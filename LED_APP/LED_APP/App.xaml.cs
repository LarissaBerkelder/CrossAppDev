using LED_APP.MVVM.ViewModels;
using LED_APP.MVVM.Views;
using LED_APP.Services;
using LED_APP.Views;

namespace LED_APP
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            MainPage = new NavigationPage(new ConnectPage());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BluetoothService>();
            services.AddTransient<ConnectViewModel>();
            services.AddTransient<ControlViewModel>();
        }

    }
}
