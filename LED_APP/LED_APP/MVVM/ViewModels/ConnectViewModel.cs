using LED_APP.MVVM.Models;
using LED_APP.MVVM.Views;
using LED_APP.Services;
using LED_APP.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED_APP.MVVM.ViewModels
{
    public class ConnectViewModel : ViewModelBase
    {
        private readonly BluetoothService _bluetoothService;

        public ConnectModel ConnectModel { get; set; }
        
        public ConnectViewModel(BluetoothService bluetoothService)
        {
            // Bluetooth services 
            _bluetoothService = bluetoothService;
            _bluetoothService.ConnectionLost += GoToConnectPage;


            // Initialize the connect model and the button click command
            ConnectModel = new ConnectModel();
            ConnectModel.ConnectButton.Command = new Command(ConnectButtonClickedAsync); 

        }


        private async void ConnectButtonClickedAsync()
        {
            // Change the UI 
            ConnectModel.ConnectButton.ButtonVisible = false;
            ConnectModel.ActivityRunning = true;

            // Connect with bluetooth 
            if(await ConnectBluetoothAsync())
            {
                await App.Current.MainPage.Navigation.PushAsync(new ControlPage());
            }
            else GoToConnectPage();     
        }

        public async Task<bool> ConnectBluetoothAsync()
        {
            // Start to connect to the bluetooth device
            ConnectModel.ConnectLabel = "Scanning for devices";
            await _bluetoothService.ScanForDevicesAsync();

            if (await _bluetoothService.ConnectToDeviceAsync("LEDSTRIP"))
            {
                Debug.WriteLine("Connected to LEDSTRIP");
                ConnectModel.ConnectLabel = "Connected to LEDSTRIP";
            }
            else return false;

            Debug.WriteLine("Get service");
            await _bluetoothService.GetServiceDeviceAsync(new Guid("a71ec97a-1f14-11ee-be56-0242ac120002"));

            Debug.WriteLine("Get characterstics");
            await _bluetoothService.GetCharacteristicsAsync();

            return true;
        }

    }
}
