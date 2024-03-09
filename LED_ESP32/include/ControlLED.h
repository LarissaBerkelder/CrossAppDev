#ifndef ControlLED_h
#define ControlLED_h

#include <Arduino.h>
#include <FastLED.h>


#define NUM_LEDS 85
#define DATA_PIN_LEDS 5  

class ControlLED {
public:
    void controlLedInit();

    void setBrightness(int value);
    void setRGB(CRGB value);

    void noEffect();
    void blink();
    void fade();

private:
    CRGB rgbValue = CRGB(255, 0, 0); 
    int effectLED = 0; 
};




#endif