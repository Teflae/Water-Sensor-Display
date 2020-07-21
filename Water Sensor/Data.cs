using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace Water_Sensor
{
    class Data
    {
        private SerialPort Serial;
        private string Buffer = "";

        public List<TurbidityDataset> Dataset = new List<TurbidityDataset>();
        public string DataType = "Unknown";

        public event EventHandler DataRecived;
        public bool IsConnected
        {
            get
            {
                return Serial.IsOpen;
            }
        }

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
            Serial.WriteLine("<In[water|water-turbidity]>");
        }

        public void Disconnect()
        {
            Serial.Close();
        }

        private void CheckSerial(object sender, SerialDataReceivedEventArgs e)
        {
            string[] input = Serial.ReadExisting().Split('\n');
            input[0] = Buffer + input[0];
            Buffer = input[input.Length - 1];
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i].First() == '<')
                {
                    DataType = input[i].Substring(1, input[i].Length - 2);
                }
                else
                {
                    string[] spl = input[i].Split('\t');
                    if (spl.Length >= 101) Dataset.Add(new TurbidityDataset(input[i], spl));
                }
            }
            DataRecived?.Invoke(this, new EventArgs());
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
