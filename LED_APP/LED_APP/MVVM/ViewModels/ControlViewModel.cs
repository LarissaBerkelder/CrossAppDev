using LED_APP.MVVM.Models;
using LED_APP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LED_APP.MVVM.ViewModels
{
    public class ControlViewModel : ViewModelBase
    {
        private readonly BluetoothService _bluetoothService;

        public ControlModel ControlModel {  get; set; }

        private System.Timers.Timer _timer;

        public ControlViewModel(BluetoothService bluetoothService)
        {
            // Bluetooth services 
            _bluetoothService = bluetoothService;
            _bluetoothService.ConnectionLost -= GoToConnectPage;
            _bluetoothService.ConnectionLost += ConnectionLost;

            // Initialize the control model and the button click command
            ControlModel = new ControlModel(_bluetoothService);

            // Button to reconnect with device when connection is lost
            ControlModel.Header.Command = new Command(GoToConnectPage);

            // Button to save a selected color in the collection view
            ControlModel.SavedColors.SaveCommand = new Command(SaveColor);

            // Button to send a saved color to the bluetooth device
            ControlModel.SavedColors.SendCommand = new Command<SavedColorItem>(SendColor);

            // Button to select the effect
            ControlModel.Effects.EffectCommand = new Command<EffectItem>(SelectEffect);
            
            ControlModel.Effects.EffectCollection = new ObservableCollection<EffectItem> 
            {
                new() {BorderColor = Color.FromArgb("#2CA2B5"), Source = "noeffect.svg", EffectName = "No effect", Command = ControlModel.Effects.EffectCommand, EffectMessage = "NOEFFECT"},
                new() {BorderColor = Color.FromArgb("#181818"), Source = "blink.png", EffectName = "Blinking", Command = ControlModel.Effects.EffectCommand, EffectMessage = "BLINK"},
                new() {BorderColor = Color.FromArgb("#181818"), Source = "fade.png", EffectName = "Fading", Command = ControlModel.Effects.EffectCommand, EffectMessage = "FADING"}
            };

            // Initialize timer for retrieving brightness
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void ConnectionLost(object sender, EventArgs e)
        {
            ControlModel.Header.ConnectIndicator = Color.FromArgb("#BD0000");
            ControlModel.Header.ConnectionStatus = "Disconnnected";
            ControlModel.Header.ButtonVisible = true;

            // Stop the timer
            _timer?.Stop();
            _timer?.Dispose();
        }

        private void SaveColor()
        {
            // The current selected color
            Color color = ControlModel.ColorPicker.ControlSelectedColor;

            // Check if the color is not already saved in the collection
            if (!ControlModel.SavedColors.SavedColorsCollection.Any(item => item.Color == color))
            {
                if (ControlModel.SavedColors.SavedColorsCollection.Count >= 4)
                {
                    ControlModel.SavedColors.SavedColorsCollection.RemoveAt(0);
                }
                ControlModel.SavedColors.SavedColorsCollection.Add(new SavedColorItem { Color = color, Command = ControlModel.SavedColors.SendCommand });
            }
        }

        private async void SendColor(SavedColorItem item)
        {
            string message = "RGB," + (item.Color.Red * 255).ToString() + "," + (item.Color.Green * 255).ToString() + "," + (item.Color.Blue * 255).ToString();
            await _bluetoothService.SendAsync(message);
        }

        private async void SelectEffect(EffectItem item)
        {
            // Send effect to the device
            await _bluetoothService.SendAsync(item.EffectMessage);

            // Change the border colors to show the selected effect
            foreach(var effect in ControlModel.Effects.EffectCollection)
            {
                if (effect.EffectName == item.EffectName) effect.BorderColor = Color.FromArgb("#2CA2B5");
                else effect.BorderColor = Color.FromArgb("#181818");
            }

        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var message = await _bluetoothService.ReadAsync();
            if (int.TryParse(message, out int intValue))
            {
                if (intValue >= 0 && intValue <= 100)
                {
                    const double maxWidth = 300;
                    ControlModel.Brightness.BrightnessValue = maxWidth * (intValue / 100.0);
                }
            }
        }
    }
}
