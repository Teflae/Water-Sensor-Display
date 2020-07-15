void setup() {
  Serial.begin(9600);
  Serial.println("$echo");
}

void loop() {
  if (Serial.available() > 0) {
      Serial.println(Serial.readString());
  }
}
