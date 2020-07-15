#Turbidity Sensor Version 3.0 Emulator.
import serial
import time

#Actual Vars
i = 0
Lz = 20
Dz = 10
Lo = False

#Simulation Vars
x = 0 #timing
xl = 0 #Laser power
xw = False #Water contact
#void setup()
p = input("Port: ")
serialPort = serial.Serial(port = p, baudrate=9600,
                           bytesize=8, timeout=2, stopbits=serial.STOPBITS_ONE)
serialPort.write(b"$100Hz[water|water-turbidity(laser|-|+)]\n")

# void loop()
while True:
    #Laser on/off
    i = i + 1
    if i > Lz:
        if Lo:
            Lo = False
        else:
            Lo = True
        i = 0

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
    _xw = b"0"
    if xw:
        _xw = b"1"

    _Lo = b"0"
    if Lo:
        _Lo = b"1"
    _a = str.encode(str(round(a)))
    _b = str.encode(str(round(b)))

    serialPort.write(_xw)
    serialPort.write(b"\t")
    serialPort.write(_Lo)
    serialPort.write(b"\t")
    serialPort.write(_a)
    serialPort.write(b"\t")
    serialPort.write(_b)
    serialPort.write(b"\n")
    time.sleep(Dz / 1000)
    