using LED_APP.MVVM.ViewModels;

namespace LED_APP.MVVM.Views;

public partial class ControlPage : ContentPage
{
	public ControlPage()
	{
		InitializeComponent();
        BindingContext = ((App)Application.Current).ServiceProvider.GetService<ControlViewModel>();

    }
}