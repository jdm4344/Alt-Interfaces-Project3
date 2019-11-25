#include <SerialCommand.h>
/*
 * Jordan Machalek
 */

// Attributes
SerialCommand sCmd;
 
void setup() {
  Serial.begin(9600);
  while(!Serial) { sCmd.addCommand("PING", pingHandler; }
}

void loop() {
  if(Serial.available() > 0){
    sCmd.readSerial();
  }
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
