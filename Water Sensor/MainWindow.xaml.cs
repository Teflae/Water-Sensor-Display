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
        private Data Sensor;
        private int Count = 0;
        private List<double> Data_Storage = new List<double>();
        public MainWindow()
        {
            InitializeComponent();
            Draw_Graph();
            Sensor = new Data();
            Sensor.DataRecived += DataRecived;
        }
 
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Sensor.IsConnected)
            {
                Sensor.Disconnect();
                ConnectButton.Content = "Connect";
                OutputTextBlock.Text = "";
            }
            else
            {
                SerialConnectionDialog Dialog = new SerialConnectionDialog();
                Dialog.Owner = this;
                bool IsAccepted = (bool)Dialog.ShowDialog();
                if (IsAccepted)
                {
                    try
                    {
                        Sensor.Connect(Dialog.Port, Dialog.BaudRate);
                        ConnectButton.Content = "Disconnect";
                    }
                    catch (Exception ex)
                    {
                        OutputTextBlock.Text += ex.Message + '\n';
                    }
                }
            }
        }

        private void DataRecived(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                int l = Sensor.Dataset.Count;
                for (int i = Count; i < l; i++)
                {
                    OutputTextBlock.Text = String.Format("{0}\t{1}\n{2}", Sensor.Dataset[i].Absorb, Sensor.Dataset[i].Diffuse, OutputTextBlock.Text);
                    //OutputTextBlock.Text = String.Format("{0}\n{1}", Sensor.Dataset[i].RawString, OutputTextBlock.Text);
                    Data_Storage.Add(Convert.ToDouble(Sensor.Dataset));
                    if (Data_Storage.Count > 30) Data_Storage.RemoveAt(0);
                }
                Count = l;
                if (OutputTextBlock.Text.Length > 200) OutputTextBlock.Text = OutputTextBlock.Text.Remove(200);
                StatusTextBlock.Text = Sensor.DataType;
            });
            // Make some data sets.
            const double margin = 10;
            double xmin = margin;
            double xmax = Data_Storage.IndexOf(Convert.ToInt32(OutputTextBlock.Text)) * 10;
            //Console.WriteLine(xmax);
            double ymin = margin;
            double ymax = TurbidityGraph.Height - margin;
            const double step = 10;
            Brush brushes = Brushes.Red;
            Random rand = new Random();
            int Required_Index = 0;
            double last_y = Data_Storage.ElementAt(0);
            Polyline polyline = new Polyline();
            TurbidityGraph.Children.Clear();
            PointCollection points = new PointCollection();
            Draw_Graph();
            try
            {
                for (double x = xmin; x <= xmax; x += step)
                {
                    last_y = Data_Storage.ElementAt(Required_Index);
                    if (last_y < ymin) last_y = (int)ymin;
                    if (last_y > ymax) last_y = (int)ymax;
                    points.Add(new Point(x, last_y));
                    Required_Index++;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Unable to Access index of " + Required_Index);
            }

            polyline.StrokeThickness = 2;
            polyline.Stroke = brushes;
            polyline.Points = points;

            TurbidityGraph.Children.Add(polyline);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text += Sensor.GetRawDataLine() + '\n';
        }

        private void Draw_Graph()
        {
            const double margin = 10;
            double xmin = margin;
            double xmax = TurbidityGraph.Width - margin;
            double ymax = TurbidityGraph.Height - margin;
            const double step = 10;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(TurbidityGraph.Width, ymax)));
            for (double x = xmin + step;
                x <= TurbidityGraph.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            TurbidityGraph.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, TurbidityGraph.Height)));
            for (double y = step; y <= TurbidityGraph.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            TurbidityGraph.Children.Add(yaxis_path);
        }
    }

}

