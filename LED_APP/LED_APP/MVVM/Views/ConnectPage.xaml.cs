using LED_APP.MVVM.ViewModels;

namespace LED_APP.Views;

public partial class ConnectPage : ContentPage
{
	public ConnectPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).ServiceProvider.GetService<ConnectViewModel>();
    }
}