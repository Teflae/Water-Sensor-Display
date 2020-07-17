using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Linq;

namespace Water_Sensor
{
    class Data
    {
        public static int[] DataExample = new int[4] { 204, 334, 850, 360 };
        private SerialPort Serial;
        public event EventHandler<DataRecivedEventArgs> DataRecived;
        public bool IsRunning = true;


        public Data()
        {
            Serial = new SerialPort();
        }

        public void Connect(string Port, int BaudRate)
        {
            Serial.PortName = Port;
            Serial.BaudRate = BaudRate;
            Serial.DataReceived += CheckSerial;
            Serial.Open();
            Serial.WriteLine("$In[water|water-turbidity]");
        }

        public void Disconnect()
        {
            Serial.Close();
        }

        public void CheckSerial(object sender, SerialDataReceivedEventArgs e)
        {
            DataRecived(this, new DataRecivedEventArgs(Serial.ReadExisting()));
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

        private static double AnalogueToDigitalScale = 5.0/1023.0;
        public static double x = 12000000.0;
        public static double b = -1.4;

        private static double TorValueToLux(int Analogue) {
            double RVoltage = Analogue * AnalogueToDigitalScale;
            double LDRVoltage = 5 - RVoltage;
            double LDRResistance = LDRVoltage / RVoltage * 10000;
            return x * Math.Pow(LDRResistance, b);
        }
    }

    public class DataRecivedEventArgs : EventArgs
    {
        public string RawString;
        public string Output;
        public int[] RawData;

        public DataRecivedEventArgs(string raw)
        {
            try
            {
                RawString = raw;
                RawData = raw.Split('\t').Select(int.Parse).ToArray();            
                Output = String.Format("{0}: {1}",RawData.Length,RawString);
            }
            catch (Exception)
            {
                Output = String.Format("?[{0}]", RawString);
            }
        }
    }
}
