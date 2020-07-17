//Turbidity Sensor Version 3.0
int a, b;
int i = 0;
int Lz = 20;
int Dz = 10;
bool Lo = false;
void setup()
{
  Serial.begin(9600);
  Serial.println("$100Hz[water|water-turbidity(laser|-|+)]");
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(7, INPUT_PULLUP);
}

void loop()
{
  i++;
  if (i > Lz) {
    if (Lo) {
      Lo = false;
      digitalWrite(8, LOW);
    }
    else {
      Lo = true;
      digitalWrite(8, HIGH);
    }
    i = 0;
  }
  a = analogRead(A0);
  b = analogRead(A1);
  Serial.print((digitalRead(7) ? 0 : 1));
  Serial.print('\t');
  Serial.print((Lo ? 0 : 1));
  Serial.print('\t');
  Serial.print(a);
  Serial.print('\t');
  Serial.println(b);
  delay(Dz);
  // put your main code here, to run repeatedly:
  if (Serial.available() > 0)
  {
    Dz = Serial.parseInt();
  }
}
