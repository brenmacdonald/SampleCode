/*
*SixStepperProj
*
*Control routine for six stepper motors, to be interfaced via
*LabVIEW.
*
*The motors supplied for this project were six
*Parallax unipolar stepper motors. Each motor is 4-phase and
*12V. (Part# 27964) This requires using a Darlington array.
*
*NOT TESTED! CURRENTLY A WORK IN PROGRESS!! (Mega board is ordered...)
*-B.Macdonald 2014
*/
#include <Stepper.h>

//Number of steps per revolution in the motors I have
#define STEPS 48
#define SPEED 30
#define STEPLIMIT 1000
//Define my six stepper motors
//NOTE: REQUIRES ARDUINO MEGA
/*
Stepper stepperOne(STEPS,8,9,10,11);
Stepper stepperTwo(STEPS,22,23,24,25);
Stepper stepperThree(STEPS,26,27,28,29);
Stepper stepperFour(STEPS,30,31,32,33);
Stepper stepperFive(STEPS,34,35,36,37);
Stepper stepperSix(STEPS,38,39,40,41);
*/
Stepper stepper[6] = {Stepper(STEPS,8,9,10,11),Stepper(STEPS,22,23,24,25),Stepper(STEPS,26,27,28,29),Stepper(STEPS,30,31,32,33),Stepper(STEPS,34,35,36,37),Stepper(STEPS,38,39,40,41)};
boolean activated[6] = {false,false,false,false,false,false};
//Hard stop sensors for the motors, simple metal contact in this case so
//I don't have to worry about wire count, and this is more cost effective.
//Shared contact switch input for the home position of all six motors.
int hsPin[6] = {2,3,4,5,6,7};
int homePin = 42;
int ledWarning = 43;

//This collects the number of steps it takes for a given motor to move to
//a hard stop.
int stepsToHS[6] = {0,0,0,0,0,0};

boolean bFirstLoop = true;
boolean commandRecieved = false;
boolean seekMotorNum = false;
int serialMotorNum = -1;
boolean seekMotorAction = false;
int serialMotorAction = 0;

void setup(){
  /*
  stepperOne.setSpeed(SPEED);
  stepperTwo.setSpeed(SPEED);
  stepperThree.setSpeed(SPEED);
  stepperFour.setSpeed(SPEED);
  stepperFive.setSpeed(SPEED);
  stepperSix.setSpeed(SPEED);
  */  
  for(int i=0; i<6;i++){
    pinMode(hsPin[i], INPUT);
    //Set up homing speed
    stepper[i].setSpeed(10);
  }
  pinMode(homePin, INPUT);
  pinMode(ledWarning, OUTPUT);
}

void loop(){
  /*The "first loop" serves two functions. First, it checks that all steppers are at their resting position.
  *later in the program I have a function call that should reset all motors to their zero position.
  *However, if a power outage were to occur I'd want to verify that they are all properly, manually, reset.
  *If I wanted to make this more robust, I'd put in 
  */
  if(bFirstLoop){
    if(digitalRead(homePin) == HIGH){
      HomingRoutine(6);
    }
    else{
      /*
      *Warns the user that the six steppers are not in their
      *resting position. Should only be needed if a sudden power outage
      *occured.
      */
      digitalWrite(ledWarning, HIGH);
    }
  }
  else{
    seekMotorNum = true;
    seekMotorAction = true;
    while(Serial.available() > 0){
      if(seekMotorNum){
        serialMotorNum = Serial.parseInt();
        seekMotorNum = false;
      }
      if(seekMotorAction){
        serialMotorAction = Serial.parseInt();
        seekMotorAction = false;
      }
      int remainingByte = Serial.read();
      commandRecieved = true;
    }
    if(commandRecieved){
      StateMachine(serialMotorNum, serialMotorAction);
    }
  }
}

void HomingRoutine(int motorCount){  
  digitalWrite(ledWarning, LOW);
  for(int i=0; i<motorCount; i++){
    int iterator = 0;
    while(hsPin[i] == LOW || iterator > STEPLIMIT){
      stepper[i].step(1);
      iterator++;
    }
    stepsToHS[i] = (iterator-1);
    //Resets the motor
    stepper[i].step(-stepsToHS[i]);
  }
  bFirstLoop = false;
  for(int i=0; i<motorCount;i++){       
    stepper[i].setSpeed(SPEED);
  }
}

/*NOTE: The step() function will block all other functions from taking place
*for the purposes of this project I don't need to have my motors running in sync
*so I'm free to just use this. To do something else I'd pretty much need to make
*something custom here, like having single steps with all motors that activate
*until they get to their hard stops.
*/
void ActivateMotor(int motorNum, boolean isPositive){
  int steps = 0;
  if(isPositive){
    steps = stepsToHS[motorNum];    
  }
  else{
    steps = -stepsToHS[motorNum];    
  }
  stepper[motorNum].step(steps);
  activated[motorNum] = isPositive;
}

void ResetMotors(){
  for(int i=0; i<6; i++){
    if(activated[i]){
      ActivateMotor(i,!activated[i]);
    }
  }
}

void StateMachine(int motor, int action){
  if(motor == 999){
    //Code for resetting the motors, effectively starting over.
    ResetMotors();
    return;
  }
  boolean motorState = (action==1);
  ActivateMotor(motor, motorState);
}
