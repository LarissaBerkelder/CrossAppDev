using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.EventArgs;
using System.Diagnostics;
using Plugin.BLE.Abstractions.Exceptions;
using Exception = System.Exception;
using Microsoft.Maui.Dispatching;
using LED_APP.Views;

namespace LED_APP.Services
{
    public class BluetoothService
    {
        public event EventHandler<DeviceEventArgs> ConnectionLost;

        private readonly IAdapter _adapter;

        private List<IDevice> _bluetooth_devices;

        private IDevice _device;
        private IService _service;
        private ICharacteristic _characteristicSend;
        private ICharacteristic _characteristicRetrieve;
        
        public BluetoothService()
        {
            _bluetooth_devices = new List<IDevice>();
            _adapter = CrossBluetoothLE.Current.Adapter;

            _adapter.DeviceDiscovered += (s, a) => _bluetooth_devices.Add(a.Device);
            _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        public async Task ScanForDevicesAsync()
        {
            // Ask for location permission needed to connect to bluetooth device
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            // Clear previous device list
            _bluetooth_devices.Clear();
            // Start scanning for bluetooth devices
            await _adapter.StartScanningForDevicesAsync();
        }

        public async Task<bool> ConnectToDeviceAsync(string deviceName)
        {
            if (_bluetooth_devices.FirstOrDefault(d => d.Name == deviceName) == null) return false;

            _device = _bluetooth_devices.FirstOrDefault(d => d.Name == deviceName);

            var connectParameters = new ConnectParameters(false, true);
            try
            {
                await _adapter.ConnectToDeviceAsync(_device, connectParameters);
                return true;
            }
            catch (DeviceConnectionException e)
            {
                Debug.WriteLine($"Could not connect to device {e}");
                return false;
            }
        }

        public async Task GetServiceDeviceAsync(Guid SERVICE_UUID)
        {
            var services = await _device.GetServicesAsync();
            _service = services.FirstOrDefault(d => d.Id == SERVICE_UUID);
        }

        public async Task GetCharacteristicsAsync()
        {
            var characteristics = await _service.GetCharacteristicsAsync();

            foreach (var characteristic in characteristics)
            {
                if (characteristic.CanWrite) _characteristicSend = characteristic;
                if (characteristic.CanRead) _characteristicRetrieve = characteristic;
            }
        }

        public async Task SendAsync(string message)
        {
            Debug.WriteLine($"Trying to send message {message}");
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            try
            {
                await _characteristicSend.WriteAsync(messageBytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending command: {message}. Exception: {ex}");
            }
            await Task.Delay(100);
        }

        public async Task<string> ReadAsync()
        {
            string message = null;
            try
            {
                var message_bytes = await _characteristicRetrieve.ReadAsync();
                message = Encoding.UTF8.GetString(message_bytes.data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving message. Exception: {ex}");
            }
            return message;
        }

        public async Task DisconnectDeviceAsync(IDevice device)
        {
            await _adapter.DisconnectDeviceAsync(device);
        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs e)
        {
            Debug.WriteLine("Device connection lost");
            // Raise the ConnectionLost event
            ConnectionLost?.Invoke(this, e);
        }
    }
}
