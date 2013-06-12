#include <LiquidCrystal.h>

LiquidCrystal lcd(12, 11, 5, 4, 3, 2);
int buttonPin = 6;
int backlightPin = 10;
int backlightValue = 255;
int fadeAmount = 5;
char incomingChar; 

int idle = 0;

void setup() {  
  lcd.begin(16, 2);
  Serial.begin(9600);  
  pinMode(backlightPin,OUTPUT);
  pinMode(buttonPin,INPUT);
  digitalWrite(backlightPin,HIGH); 
  lcd.write("-----NODATA-----");  
}

void loop() {
  if (digitalRead(buttonPin) == HIGH)
  {    
    if (backlightValue==255)
    {
      backlightValue = fadeAmount;
    }
    else
      backlightValue += fadeAmount;

    analogWrite(backlightPin,backlightValue);
    delay(50);
  }

  if (Serial.available() > 0) {   
    idle = 1;  
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
  }
  else
  {        
    if(idle==0)
    {    
      lcd.write("-----NODATA-----");  
      idle=1;
    }
  }

}










