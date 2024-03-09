#include "ControlLED.h" 

CRGB leds[NUM_LEDS];

void ControlLED::controlLedInit(){
    FastLED.addLeds<WS2812, DATA_PIN_LEDS, GRB>(leds, NUM_LEDS);
    for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(255, 0, 0); 
    }
    FastLED.show();
}

void ControlLED::setBrightness(int value){
    FastLED.setBrightness(value);
    fill_solid(leds, NUM_LEDS, rgbValue);
    FastLED.show();
}

void ControlLED::setRGB(CRGB value){
    rgbValue = value; 
    FastLED.show();
}

void ControlLED::noEffect(){
  fill_solid(leds, NUM_LEDS, rgbValue);
  FastLED.show();
}

void ControlLED::blink(){
    fill_solid(leds, NUM_LEDS, rgbValue);
    FastLED.show();
    delay(300); 
    fill_solid(leds, NUM_LEDS, CRGB::Black);
    FastLED.show();
    delay(300); 
}

void ControlLED::fade(){
    for (int i = 0; i < 255; i++) {
        fill_solid(leds, NUM_LEDS, CRGB(rgbValue.r * i / 255, rgbValue.g * i / 255, rgbValue.b * i / 255));
        FastLED.show();
        delay(10);  
    }
}
