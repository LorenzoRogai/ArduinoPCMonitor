#include <LiquidCrystal.h>
#include <DateTime.h>

LiquidCrystal lcd(12, 11, 5, 4, 3, 2);
int buttonPin = 6;
int backlightPin = 10;
int backlightValue = 255;
int fadeAmount = 5;
char incomingChar; 

int charsRead = 0;

void setup() {  
  lcd.begin(16, 2);
  Serial.begin(9600);  
  pinMode(backlightPin,OUTPUT);
  pinMode(buttonPin,INPUT);
  digitalWrite(backlightPin,HIGH); 
}

void loop() {
  if (digitalRead(buttonPin) == HIGH)
  {
    charsRead = 0;
    if (backlightValue==255)
    {
      backlightValue = fadeAmount;
    }
    else
      backlightValue += fadeAmount;

    analogWrite(backlightPin,backlightValue);   
  }

  if (Serial.available() > 0) {     
    // read the incoming byte:
    incomingChar = Serial.read();
    if(incomingChar == 'r')   {
      lcd.clear();
      lcd.setCursor(0,0);
    }     
    else if(incomingChar == 'n')   {
      lcd.setCursor(0,1);      
    }   
    else 
      lcd.print(incomingChar);  

    charsRead++;
    if (charsRead==34)
    {
      delay(1000);
      charsRead=0;
    }
  }
  else
  {        
    charsRead = 0;
    lcd.clear();
    lcd.write("-----NODATA-----");  
    delay(1000);
  }

}








