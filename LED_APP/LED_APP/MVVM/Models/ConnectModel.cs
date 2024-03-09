using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED_APP.MVVM.Models
{
    public class ConnectModel : ModelBase       
    {
        private String _connectLabel; 
        public String ConnectLabel
        {
            get => _connectLabel;
            set => SetProperty(ref _connectLabel, value);
        }
       
        private bool _activityRunning;
        public bool ActivityRunning
        {
            get => _activityRunning;
            set => SetProperty(ref _activityRunning, value);
        }

        public ConnectButton ConnectButton { get; set; }

        public ConnectModel()
        {
            ConnectLabel = "Connect to the device";
            ConnectButton = new ConnectButton() { ButtonVisible = true };
            ActivityRunning = false;    

        }

    }

    public class ConnectButton : ModelBase
    {
        private bool _buttonVisible;
        public bool ButtonVisible
        {
            get => _buttonVisible;
            set => SetProperty(ref _buttonVisible, value);
        }

        public Command Command { get; set; }

    }

}
