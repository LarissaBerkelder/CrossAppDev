using LED_APP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED_APP.MVVM.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        public static void GoToConnectPage(object sender, EventArgs e)
        {
            var newThread = new System.Threading.Thread(() =>
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                    await App.Current.MainPage.Navigation.PushAsync(new ConnectPage());
                    App.Current.MainPage.Navigation.RemovePage(currentPage);
                });
            });
            newThread.Start();
        }
        public static void GoToConnectPage()
        {
            var newThread = new System.Threading.Thread(() =>
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                    await App.Current.MainPage.Navigation.PushAsync(new ConnectPage());
                    App.Current.MainPage.Navigation.RemovePage(currentPage);
                });
            });
            newThread.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
