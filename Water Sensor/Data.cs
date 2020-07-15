using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Water_Sensor
{
    class Data
    {
        public static int[] DataExample = new int[4] { 204, 334, 850, 360 };
        public SerialPort Serial;

        public Data(string Port = "COM1")
        {
            Serial = new SerialPort();
            Serial.PortName = Port;
            Serial.BaudRate = 9600;
            Serial.ReadTimeout = 4000;
            Serial.Open();
            Serial.WriteLine("$In[water|water-turbidity]");
        }

        public string GetRawDataLine()
        {
            try
            {
                return Serial.ReadLine();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
