//Turbidity Sensor Version 3.3
int a, b;
int i = 0;
int Lz = 10;
int Dz = 10;
bool Lo = false;
String Data = "";
void setup()
{
  Serial.begin(38400);
  Serial.println("<'rate':'0.01','data':[{'x':'time'},{'x':'water'},{'x':'control','loop':'20'},{'x':'turbidity','var':'absorb'},{'x':'turbidity','var':'diffuse'}]>\n");
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(7, INPUT_PULLUP);
}

void loop()
{
  i++;
  if (i >= Lz) {
    if (Lo) {
      Lo = false;
      digitalWrite(8, LOW);
      Serial.println(Data);
      Data = "";
    }
    else {
      Lo = true;
      digitalWrite(8, HIGH);
    }
    i = 0;
  }  
  delay(Dz);
  a = analogRead(A0);
  b = analogRead(A1);
  Data += String(millis()) + '\t' + (digitalRead(7) ? "0" : "1") + '\t' + (Lo ? "1" : "0") + '\t' + String(a) + '\t' + String(b) + '\t';


  // put your main code here, to run repeatedly:
  if (Serial.available() > 0)
  {
    Dz = Serial.parseInt();
  }
}
