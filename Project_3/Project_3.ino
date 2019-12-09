#include <SerialCommand.h>
/*
 * Jordan Machalek
 */

// Attributes
SerialCommand sCmd;

// Joystick
//const int swPin = 2; // digital switch for when stick is depressed
const int swXPin = A0;
const int swYPin = A1;
int swXVal = 0;
int swYVal = 0;

// Velostat
const int pressPin0 = A0;
const int pressPin1 = A1;
const int pressPin2 = A2;

// Pressure Sensor Values
int press0Value = 0; // First pressure sensor
int press1Value = 0; // Second pressure sensor
int press2Value = 0; // Third pressure sensor

void setup() {
  Serial.begin(9600);
  while(!Serial) { sCmd.addCommand("PING", pingHandler); }

  pinMode(swPin, INPUT);
  // Velostat Pressure Sensors
  pinMode(pressPin0, INPUT);
  pinMode(pressPin1, INPUT);
  pinMode(pressPin2, INPUT);
}

void loop() {
  if(Serial.available() > 0){
    sCmd.readSerial();
  }

  // Get joystick readings
  swXVal = analogRead(swXPin);
  swYVal = analogRead(swYPin);

  // Get readings from Velostat pressure sensors
  press0Value = analogRead(pressPin0);
  press1Value = analogRead(pressPin1);
  press2Value = analogRead(pressPin2);

//  Serial.print("Switch:  ");
//  Serial.print();
//  Serial.print("\n");
//  Serial.print("X-axis: ");
//  Serial.print(analogRead(swXPin));
//  Serial.print("\n");
//  Serial.print("Y-axis: ");
//  Serial.println(analogRead(swYPin));
//  Serial.print("\n\n");
//  delay(500);
}

void pingHandler(const char *cmd) {
  Serial.println("PONG");
}

void echoHandler() {
  char *arg;
  arg = sCmd.next(); // get the next argument provided on the serial command

  if(arg!= NULL){
    Serial.println(arg);
  }
  else {
    Serial.println("Nothing to echo");
  }
}
