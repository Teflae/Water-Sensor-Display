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
        private List<double> Diffuse_Data_Storage = new List<double>();
        private List<double> Absorb_Data_Storage = new List<double>();
        public MainWindow()
        {
            InitializeComponent();
            Draw_Graph_Axis();
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
                    Diffuse_Data_Storage.Add(Convert.ToDouble(Sensor.Dataset[i].Diffuse / TurbidityDataset.DiffuseMaxRange));
                    if (Diffuse_Data_Storage.Count > 30) Diffuse_Data_Storage.RemoveAt(0);

                    Absorb_Data_Storage.Add(Convert.ToDouble(Sensor.Dataset[i].Absorb / TurbidityDataset.DiffuseMaxRange));
                    if (Absorb_Data_Storage.Count > 30) Absorb_Data_Storage.RemoveAt(0);
                    PassiveTextBlock.Text = Convert.ToString(Math.Round(Sensor.Dataset[i].Diffuse, 3));
                    ActiveTextBlock.Text = Convert.ToString(Math.Round(Sensor.Dataset[i].Absorb, 3));
                }
                if(Count < l) DrawTurbidityGraph();
                Count = l;
                if (OutputTextBlock.Text.Length > 700) OutputTextBlock.Text = OutputTextBlock.Text.Remove(700);
                StatusTextBlock.Text = Sensor.DataType;
            });

        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text += Sensor.GetRawDataLine() + '\n';
        }

        private void DrawTurbidityGraph()
        {
            // Make some data sets.
            const double margin = 10;
            double Diffuse_xmin = margin;
            double Diffuse_xmax = TurbidityGraph.Width - margin;
            double Diffuse_ymin = margin;
            double Diffuse_ymax = TurbidityGraph.Height - margin;
            double Diffuse_yarea = TurbidityGraph.Height - 2 * margin;

            const double Diffuse_step = 10;
            double Absorb_xmin = margin;
            double Absorb_xmax = TurbidityGraph.Width - margin;
            double Absorb_ymin = margin;
            double Absorb_ymax = TurbidityGraph.Height - margin;
            double Absorb_yarea = TurbidityGraph.Height - 2 * margin;

            const double Absorb_step = 10;
            Brush Diffuse_brushes = Brushes.Red;
            Brush Absorb_brushes = Brushes.Blue;
            int Diffuse_Required_Index = 0;
            int Absorb_Required_Index = 0;
            double Diffuse_y = Diffuse_Data_Storage.ElementAt(0);
            double Absorb_y = Absorb_Data_Storage.ElementAt(0);
            Polyline Diffuse_polyline = new Polyline();
            Polyline Absorb_polyline = new Polyline();
            TurbidityGraph.Children.Clear();
            PointCollection Diffuse_points = new PointCollection();
            PointCollection Absorb_points = new PointCollection();
            Draw_Graph_Axis();
            try
            {
                for (double Diffuse_x = Diffuse_xmin; Diffuse_x <= Diffuse_xmax; Diffuse_x += Diffuse_step)
                {
                    Diffuse_y = Diffuse_Data_Storage[Diffuse_Required_Index] * Diffuse_yarea;
                    if (Diffuse_y < Diffuse_ymin) Diffuse_y = Diffuse_ymin;
                    if (Diffuse_y > Diffuse_ymax) Diffuse_y = Diffuse_ymax;
                    Diffuse_points.Add(new Point(Diffuse_x, Diffuse_y));
                    Diffuse_Required_Index++;
                }
                for (double Absorb_x = Absorb_xmin; Absorb_x <= Absorb_xmax; Absorb_x += Absorb_step)
                {
                    Absorb_y = Absorb_Data_Storage[Absorb_Required_Index] * Absorb_yarea;
                    if (Absorb_y < Absorb_ymin) Absorb_y = Absorb_ymin;
                    if (Absorb_y > Absorb_ymax) Absorb_y = Absorb_ymax;
                    Absorb_points.Add(new Point(Absorb_x, Absorb_y));
                    Absorb_Required_Index++;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Unable to Access index of " + Absorb_Required_Index);
            }

            Diffuse_polyline.StrokeThickness = 2;
            Diffuse_polyline.Stroke = Diffuse_brushes;
            Diffuse_polyline.Points = Diffuse_points;

            TurbidityGraph.Children.Add(Diffuse_polyline);
            Absorb_polyline.StrokeThickness = 2;
            Absorb_polyline.Stroke = Absorb_brushes;
            Absorb_polyline.Points = Absorb_points;

            TurbidityGraph.Children.Add(Absorb_polyline);
        }

        private void Draw_Graph_Axis()
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

