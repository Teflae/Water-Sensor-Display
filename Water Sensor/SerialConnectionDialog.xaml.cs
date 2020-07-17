using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace Water_Sensor
{
    public partial class SerialConnectionDialog
    {
        public string Port;
        public int BaudRate;
        private List<string> Ports;
        private List<int> BaudRates;
        public SerialConnectionDialog()
        {
            InitializeComponent();
            Ports = SerialPort.GetPortNames().ToList();
            PortComboBox.ItemsSource = Ports;
            PortComboBox.SelectedIndex = 0;
            BaudRates = new int[15] { 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 74800, 115200, 230400, 250000, 500000, 1000000, 2000000 }.ToList();
            BaudRateComboBox.ItemsSource = BaudRates;
            BaudRateComboBox.SelectedIndex = 4;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Port = Ports[PortComboBox.SelectedIndex];
            BaudRate = BaudRates[BaudRateComboBox.SelectedIndex];
            this.DialogResult = true;
        }
    }
}
