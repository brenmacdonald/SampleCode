

int led = 2;
int commLed = 3;

int pinController;
boolean seekControl;
int pinAction;
boolean seekAction;

int serialInput;
boolean firstRun = true;
boolean pinCommand = false;

void setup() {                
  // initialize the digital pin as an output.
  pinMode(led, OUTPUT);
  pinMode(commLed, OUTPUT);
  Serial.begin(9600);
  Serial.setTimeout(50);
}

void loop(){
  seekControl = true;
  seekAction = true;
  //GetSerial();
  while(Serial.available() >0){
    if(seekControl){
      pinController = Serial.parseInt();
      Serial.println(pinController);
      seekControl = false;
    }
    if(seekAction){
      pinAction = Serial.parseInt();
      Serial.println(pinAction);
      seekAction = false;
    }
    int remainingByte = Serial.read();    
    pinCommand = true;
  }
  if(pinCommand){
    PinStateMachine(pinController, pinAction);    
    pinCommand = false;
  }
}

void PinStateMachine(int pinComm, int pinAct){
  Serial.print("I have entered the state machine: ");
  Serial.print(pinComm);
  Serial.print(",");
  Serial.println(pinAct);
  switch(pinComm){
    case 100:
    {
      Serial.println("I am in Case 100");   
      //control red led state      
      //GetSerial();      
      switch(pinAct){
        case 0:
        {
          Serial.println("I should turn off red LED");
          digitalWrite(led, LOW);
          break;
        }
        case 1:
        {
          Serial.println("I should turn on red LED");
          digitalWrite(led, HIGH);
          break;
        }
      }
      break;
    }
    case 200:
    {
      Serial.println("I am case 200");
      //GetSerial();      
      switch(pinAct){
        case 0:
        {
          Serial.println("I should turn off blue LED");
          digitalWrite(commLed, LOW);
          break;
        }
        case 1:
        {
          Serial.println("I should turn on blue LED");
          digitalWrite(commLed, HIGH);
          break;
        }
      }
      break;
    }
  }
}
