using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Water_Sensor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Data Sensor;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Sensor = new Data();
                OutputTextBlock.Text = "ready\n";
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text += ex.Message + '\n';
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text += Sensor.GetRawDataLine() + '\n';
        }
    }
}
