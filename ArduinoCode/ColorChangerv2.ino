#include <Adafruit_NeoPixel.h>

#define PIN 6

struct RGB{
  byte r;
  byte g;
  byte b;
};

Adafruit_NeoPixel strip = Adafruit_NeoPixel(16, PIN, NEO_GRB + NEO_KHZ800);
int analogPin = 0;

//Cool RGB = 42, 115, 250 - HSR = 219, 83, 98
//new Cool = 32,96,223 - HSL = 220,75,50
//Warm RGB = 250,163,42 - HSR = 35, 83,98
//new Warm 223,143,32 - HSL = 35,75,50
//RGB coolColor = {42,115,250};
//RGB warmColor = {250,163,42};
//RGB currentColor = {0,0,0};

byte red;
byte green;
byte blue;

float coolHue = 220; //out of 360, aka degree
float warmHue = 5;

float coolTemp = 30;  //range 10.0 C to 32.2 C
float warmTemp = 15.0;

float currentTemp = 0.0;

//used for converting HSL to RGB
float c = 0.75; //c=(1-abs(2*L -1)) * S
float m = 0.125; //m= L - C/2


boolean doTask = true; //This will force the strip to activate on its first go


float HueInterpolate(float temperature){
  float slope = (coolHue-warmHue)/(coolTemp-warmTemp);
  float b = coolHue - (slope*coolTemp);
  
  return (slope*temperature)+b;
}

void HSLToRGB(float hue){
  float x = c*(1.0-abs(fmod(hue/60,2.0)-1));
  float r1,g1,b1;
  if(hue < 60){
    r1 = c;
    g1 = x;
    b1 = 0;
  }
  else if( hue >= 60 && hue < 120){
    r1 = x;
    g1 = c;
    b1 = 0;
  }
  else if( hue >= 120 && hue < 180){
    r1 = 0;
    g1 = c;
    b1 = x;
  }
  else if( hue >= 180 && hue < 240){
    r1 = 0;
    g1 = x;
    b1 = c;
  }
  else if( hue >= 240 && hue < 300){
    r1 = x;
    g1 = 0;
    b1 = c;
  }
  else if( hue >= 300 && hue < 360){
    r1 = c;
    g1 = 0;
    b1 = x;
  }
  else{
    r1 = g1 = b1 = 1;
    return;
  }
  
  red = 255*(r1+m);
  green = 255*(g1+m);
  blue = 255*(b1+m);
}

void ReadTemperature(){
  float voltage = 0.0;
  float avTemp = 0.0;//Filter added to make color transition smooth.
  for(int i=0;i<10;i++){
    voltage = analogRead(analogPin)*(3.3/1024.0);
    avTemp += (voltage - 0.5) * 100.0;
  }
  float newTemp = avTemp/10.0;
  if(newTemp != currentTemp){
    currentTemp = newTemp;
    doTask=true;
    Serial.print("Current temp: ");
    Serial.println(currentTemp);
    Serial.print("Fahrenheit: ");
    Serial.println(currentTemp * 1.8 + 32.0);
  }
}

void CycleColors(){
  for(int i = 50; i <= 90; i++){
      float currentHue = HueInterpolate((float)i);
      Serial.print("NewHue: ");
      Serial.println(currentHue);
      HSLToRGB(currentHue);
      Serial.print("Red: ");
      Serial.print(red);
      Serial.print(" Green: ");
      Serial.print(green);
      Serial.print(" Blue: ");
      Serial.println(blue);
      strip.setPixelColor(0,red,green,blue);
      strip.show();
      delay(100);
  }
}

void setup(){
  Serial.begin(9600);
  analogReference(EXTERNAL);//I use the 3.3v analog line as a reference. Better precision for the temp sensor in exchange for constraining the temperature range.
  strip.begin();
  strip.show();
}

void loop(){
  ReadTemperature();
  if(doTask){
    float currentHue = HueInterpolate((float)currentTemp);
    HSLToRGB(currentHue);
    for(int i=0; i<strip.numPixels();i++){
      strip.setPixelColor(i,red,green,blue);
    }
    strip.show();
  }
  //doTask = false;
  delay(100);
}


