/*
 * Jordan Machalek
 * Adapted fron built-in Arduino SerialCallResponseASCII example
 */

// Attributes
// Joystick
//const int swPin = 2; // digital switch for when stick is depressed
const int swXPin = A0;
const int swYPin = A1;
// Joystick values
int swXVal = 0;
int swYVal = 0;
// Velostat
const int pressPin0 = A0;
const int pressPin1 = A1;
const int pressPin2 = A2;
// Velostat Values
int press0Value = 0; // First pressure sensor
int press1Value = 0; // Second pressure sensor
int press2Value = 0; // Third pressure sensor

void setup() {
  Serial.begin(9600);
  while(!Serial) { ; } // wait for serial connection

  //pinMode(swPin, INPUT);
  pinMode(swXPin, INPUT);
  pinMode(swYPin, INPUT);
  // Velostat Pressure Sensors
  pinMode(pressPin0, INPUT);
  pinMode(pressPin1, INPUT);
  pinMode(pressPin2, INPUT);
}

void loop() {
  if(Serial.available() > 0){
    // get call byte from Unity
    inByte = Serial.read();

    // Get joystick readings
    swXVal = analogRead(swXPin);
    swYVal = analogRead(swYPin);
  
    // Get readings from Velostat pressure sensors
    press0Value = analogRead(pressPin0);
    press1Value = analogRead(pressPin1);
    press2Value = analogRead(pressPin2);
  
    // Write parsable string of data to Unity
    Serial.print(swXVal); // Joystick X
    Serial.print(",");
    Serial.print(swYVal); // Joystick Y
    Serial.print(",");
    Serial.print(press0Value); // Velostat 1
    Serial.print(",");
    Serial.print(press1Value); // Velostat 2
    Serial.print(",");
    Serial.println(press2Value); // Velostat 3    
  }

  
}

void establishContact() {
  while (Serial.available() <= 0) {
    Serial.println("0,0,0");   // send an initial string
    delay(300);
  }
}
