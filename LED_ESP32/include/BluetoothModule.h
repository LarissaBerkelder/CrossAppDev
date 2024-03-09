#ifndef BluetoothModule_h
#define BluetoothModule_h

#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>

#define SERVICE_UUID "a71ec97a-1f14-11ee-be56-0242ac120002"

#define SEND_CHARACTERISTIC_UUID "a71ecbfa-1f14-11ee-be56-0242ac120002"
#define RECEIVE_CHARACTERISTIC_UUID "b22ecbfa-1f14-11ee-be56-0242ac120003"

class BluetoothModule {
public:
    bool deviceConnected = false;
    bool advertisingRestarted = false;
    const char* message = "";

    void ble_init();
    void restartAdvertising();
    void sendMessage(String analogValue);

private:
    BLEServer *pServer = NULL;
    BLECharacteristic *sendCharacteristics = NULL;
    BLECharacteristic *receiveCharacteristic = NULL;

};

class MyServerCallbacks : public BLEServerCallbacks {
    BluetoothModule* bluetoothModule;

public:
    MyServerCallbacks(BluetoothModule* module): bluetoothModule(module) {}
    void onConnect(BLEServer *pServer);
    void onDisconnect(BLEServer *pServer);
};

class MyCharacteristicCallbacks : public BLECharacteristicCallbacks {
    BluetoothModule* bluetoothModule;
    
public:
    MyCharacteristicCallbacks(BluetoothModule* module): bluetoothModule(module) {}
    void onWrite(BLECharacteristic *pCharacteristic);
};


#endif