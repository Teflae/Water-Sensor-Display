#Turbidity Sensor Version 3.0 Emulator.
import serial
import time

#Actual Vars
i = 0
Lo = False
Data = ""

#Simulation Vars
x = 0 #timing
xl = 0 #Laser power
xw = False #Water contact
st = time.time()
#void setup()
p = input("Port (COM2): ")
if p == "":
    p = "COM2"
b = input("Baud (38400): ")
if b == "":
    b = "38400"
d = input("Dz (10): ")
if d == "":
    d = "10"
l = input("Lz (10): ")
if l == "":
    l = "10"
Dz = int(d)
Lz = int(l)
serialPort = serial.Serial(port = p, baudrate=int(b),
                           bytesize=8, timeout=2, stopbits=serial.STOPBITS_ONE)
serialPort.write(b"<'rate':'0.01','data':[{'x':'time'},{'x':'water'},{'x':'control','loop':'20'},{'x':'turbidity','var':'absorb'},{'x':'turbidity','var':'diffuse'}]>\n")

# void loop()
while True:
    #Laser on/off
    i = i + 1
    if i >= Lz:
        if Lo:
            Lo = False
            Data = Data + "\n"
            serialPort.write(str.encode(Data))
        else:
            Lo = True
        i = 0

    time.sleep(Dz / 1000)

    #Simulation
    x = x + 1
    if x > 200:
        x = 0
    xa = x + 200 #ambient
    if Lo:
        xl = 600
    else:
        xl = xl * 0.8
        pass

    a = xa + xl
    b = 39

    #Conversions because python is a bad programing language.
    _xw = "0"
    if xw:
        _xw = "1"

    _Lo = "0"
    if Lo:
        _Lo = "1"
    _t = str(round((time.time() - st) * 1000))
    _a = str(round(a))
    _b = str(round(b))

    Data = Data + _t + "\t" + _xw + "\t" + _Lo  + "\t" + _a + "\t" + _b + "\t"
    