#include <Arduino.h>
#include "BluetoothModule.h"
#include "ControlLED.h"   

#define PIN_POTENTIOMETER 36

BluetoothModule bluetoothModule;
ControlLED controlLED;

void whenConnected();
void parseMessage(const char* message);


String lastMessage;
int effect = 0; 

void setup() {

  Serial.begin(9600);
  bluetoothModule.ble_init();
  controlLED.controlLedInit();

  Serial.print("Waiting for device to connect ...");
  while (!bluetoothModule.deviceConnected)
  {
    Serial.print(".");
    delay(1000);
  }

  int analogValue = analogRead(PIN_POTENTIOMETER);
  controlLED.setBrightness(map(analogValue, 0, 4095, 0, 255));
}

void loop() {
  //* If the device is disconnected restart advertising 
  if(!bluetoothModule.deviceConnected && !bluetoothModule.advertisingRestarted){
      Serial.println("Device not connected. Initializing Bluetooth...");
      bluetoothModule.restartAdvertising();
      delay(1000); 
  }

  if(bluetoothModule.deviceConnected){
      whenConnected();
  }
}


void whenConnected(){
  if(!lastMessage.equals(bluetoothModule.message)){
    lastMessage = bluetoothModule.message;
    parseMessage(bluetoothModule.message);
  }

  if(effect == 0) controlLED.noEffect();
  if(effect == 1) controlLED.blink();
  if(effect == 2) controlLED.fade();

  int analogValue = analogRead(PIN_POTENTIOMETER);
  controlLED.setBrightness(map(analogValue, 0, 4095, 0, 255));

  bluetoothModule.sendMessage(String(map(analogValue, 0,4095,0,100)));
}

void parseMessage(const char* message){
  if (strcmp(message, "NOEFFECT") == 0) {
        effect = 0;
    } else if (strcmp(message, "BLINK") == 0) {
        effect = 1;
    } else if (strcmp(message, "FADING") == 0) {
        effect = 2;
    } else if (strncmp(message, "RGB", 3) == 0) {
        int red, green, blue;

        sscanf(message, "RGB,%d,%d,%d", &red, &green, &blue);
        red = constrain(red, 0, 255);
        green = constrain(green, 0, 255);
        blue = constrain(blue, 0, 255);
        controlLED.setRGB(CRGB(red, green, blue));
    }
}
