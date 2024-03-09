using LED_APP.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LED_APP.MVVM.Models
{
    public class ControlModel : ModelBase
    {
        public Header Header { get; set; }
        public ColorPicker ColorPicker { get; set; }
        public SavedColors SavedColors { get; set; }
        public Brightness Brightness { get; set; }
        public Effects Effects { get; set; }

        public ControlModel(BluetoothService bluetoothService)
        {
            Header = new Header() { ConnectIndicator = Color.FromArgb("#0BBD00"), ConnectionStatus = "Connected", ButtonVisible = false };

            ColorPicker = new ColorPicker(bluetoothService);

            SavedColors = new SavedColors() { SavedColorsCollection = new ObservableCollection<SavedColorItem>()};

            Brightness = new Brightness() { BrightnessValue = 0 };

            Effects = new Effects();  
        }
    }

    public class Header : ModelBase
    {
        private Color _connectIndicator;
        public Color ConnectIndicator
        {
            get => _connectIndicator;
            set => SetProperty(ref _connectIndicator, value);
        }

        private string _connectionStatus;
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        private bool _buttonVisible;
        public bool ButtonVisible
        {
            get => _buttonVisible;
            set => SetProperty(ref _buttonVisible, value);
        }

        public Command Command { get; set; }
    }

    public class ColorPicker : ModelBase
    {
        private readonly BluetoothService _bluetoothService;

        public ColorPicker(BluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        private DateTime lastColorChangeTime = DateTime.MinValue;
        private const int ColorChangeDelayMilliseconds = 500;

        private Color _controlSelectedColor;
        public Color ControlSelectedColor
        {
            get => _controlSelectedColor;
            set
            {
                SetProperty(ref _controlSelectedColor, value);
                if (DateTime.Now - lastColorChangeTime > TimeSpan.FromMilliseconds(ColorChangeDelayMilliseconds))
                {
                    Debug.WriteLine($"Selected color: {_controlSelectedColor}");
                    SendSelectedColor();
                    lastColorChangeTime = DateTime.Now;
                }
            }

        }

        public async void SendSelectedColor()
        {
            string message = "RGB," + (ControlSelectedColor.Red * 255).ToString() + "," + (ControlSelectedColor.Green * 255).ToString() + "," + (ControlSelectedColor.Blue * 255).ToString();
            await _bluetoothService.SendAsync(message);
        }
    }


    public class SavedColors : ModelBase
    {
        public Command SaveCommand { get; set; }

        private ObservableCollection<SavedColorItem> _savedColorsCollection;
        public ObservableCollection<SavedColorItem> SavedColorsCollection
        {
            get => _savedColorsCollection;
            set => SetProperty(ref _savedColorsCollection, value);
        }
        public Command<SavedColorItem> SendCommand;
    }


    public class SavedColorItem : BindableObject
    {
        public Color Color { get; set; }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SavedColorItem));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }


    public class Brightness : ModelBase
    {
        private double _brightnessValue;
        public double BrightnessValue
        {
            get => _brightnessValue;
            set => SetProperty(ref _brightnessValue, value);
        }
    }


    public class Effects : ModelBase
    {
        private ObservableCollection<EffectItem> _effectCollection;
        public ObservableCollection<EffectItem> EffectCollection
        {
            get => _effectCollection;
            set => SetProperty(ref _effectCollection, value);
        }

        public Command<EffectItem> EffectCommand;
    }

    public class EffectItem : BindableObject, INotifyPropertyChanged
    {
        private Color _borderColor; 
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }

        public string Source { get; set; }
        public string EffectName { get; set; }

        public string EffectMessage { get; set; }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EffectItem));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

    }

}
