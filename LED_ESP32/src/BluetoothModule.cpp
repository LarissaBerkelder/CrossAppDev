#include "BluetoothModule.h"

void BluetoothModule::ble_init() {
    BLEDevice::init("LEDSTRIP");
    pServer = BLEDevice::createServer();
    pServer->setCallbacks(new MyServerCallbacks(this));

    BLEService *pService = pServer->createService(SERVICE_UUID);
    
    receiveCharacteristic = pService->createCharacteristic(
        RECEIVE_CHARACTERISTIC_UUID,
        BLECharacteristic::PROPERTY_WRITE |
        BLECharacteristic::PROPERTY_NOTIFY);

    sendCharacteristics = pService->createCharacteristic(
        SEND_CHARACTERISTIC_UUID,
        BLECharacteristic::PROPERTY_READ |
        BLECharacteristic::PROPERTY_NOTIFY);

    receiveCharacteristic->setCallbacks(new MyCharacteristicCallbacks(this));

    BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
    pAdvertising->addServiceUUID(SERVICE_UUID);

    pServer->getAdvertising()->start();
    pService->start();
}

void BluetoothModule::restartAdvertising(){
    pServer->startAdvertising();
    advertisingRestarted = true; 
}

void BluetoothModule::sendMessage(String analogValue){
    uint8_t *message_bytes = (uint8_t *)analogValue.c_str();
    size_t length = analogValue.length();
    if(analogValue.length() > 0){
        sendCharacteristics->setValue(message_bytes, length);
    }
}

void MyServerCallbacks::onConnect(BLEServer *pServer) {
    Serial.println("\nDevice connected");
    bluetoothModule->deviceConnected = true;
}

void MyServerCallbacks::onDisconnect(BLEServer *pServer) {
    bluetoothModule->deviceConnected = false; 
    bluetoothModule->advertisingRestarted = false; 
}

void MyCharacteristicCallbacks::onWrite(BLECharacteristic *pCharacteristic) {
    uint8_t *retrieved = pCharacteristic->getData();
    const char *retrieved_message = (const char *)retrieved;
    bluetoothModule->message = retrieved_message;
    Serial.println("Retrieved message: ");
    Serial.println(retrieved_message);
}