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
        private int TurbidityGraphDataLength = 100;
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
                    //OutputTextBlock.Text = String.Format("{0}\t{1}\n{2}", Sensor.Dataset[i].Absorb, Sensor.Dataset[i].Diffuse, OutputTextBlock.Text); For Debugging Purposes
                    Diffuse_Data_Storage.Add(Convert.ToDouble(Sensor.Dataset[i].Diffusion));
                    Absorb_Data_Storage.Add(Convert.ToDouble(Sensor.Dataset[i].Absorption));
                    if (Diffuse_Data_Storage.Count > TurbidityGraphDataLength)
                    {
                        Diffuse_Data_Storage.RemoveAt(0);
                        Absorb_Data_Storage.RemoveAt(0);
                    };
                    PassiveTextBlock.Text = String.Format("({0:0.000})", Sensor.Dataset[i].Diffuse);
                    ActiveTextBlock.Text = String.Format("({0:0.000})", Sensor.Dataset[i].Absorb);

                    AmbientTextBlock.Text = String.Format("({0:0.000})", Sensor.Dataset[i].Ambient);

                    AbsorbtionTextBlock.Text = String.Format("{0:0.0%}", Sensor.Dataset[i].Absorption);
                    DiffusionTextBlock.Text = String.Format("{0:0.0%}", Sensor.Dataset[i].Diffusion);
                }
                if (Count < l) DrawTurbidityGraph();
                Count = l;
                if (OutputTextBlock.Text.Length > 700) OutputTextBlock.Text = OutputTextBlock.Text.Remove(700);
                StatusTextBlock.Text = Sensor.DataType;
            });

        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DrawTurbidityGraph()
        {
            // Pre-calculate margins and chart area for preformance
            double margin = 10;
            double xmin = margin;
            double xmax = TurbidityGraph.ActualWidth - margin;
            double xarea = TurbidityGraph.ActualWidth - 2 * margin;
            double ymin = margin;
            double ymax = TurbidityGraph.ActualHeight - margin;
            double yarea = TurbidityGraph.ActualHeight - 2 * margin;

            double step = xarea / TurbidityGraphDataLength;
            Brush DiffuseColour = Brushes.Red;
            Brush AbsorbColour = Brushes.Blue;
            //int i = 0;
            double x = xmin;
            Polyline Diffuse_polyline = new Polyline();
            Polyline Absorb_polyline = new Polyline();
            TurbidityGraph.Children.Clear();
            PointCollection Diffuse_points = new PointCollection();
            PointCollection Absorb_points = new PointCollection();
            Draw_Graph_Axis();

            int length = Diffuse_Data_Storage.Count;
            for (int i = 0; i < length; i++)
            {
                double Diffuse_y = yarea - Diffuse_Data_Storage[i] * yarea;
                double Absorb_y = yarea - Absorb_Data_Storage[i] * yarea;
                if (Diffuse_y < ymin) Diffuse_y = ymin;
                if (Diffuse_y > ymax) Diffuse_y = ymax;
                Diffuse_points.Add(new Point(x, Diffuse_y));
                if (Absorb_y < ymin) Absorb_y = ymin;
                if (Absorb_y > ymax) Absorb_y = ymax;
                Absorb_points.Add(new Point(x, Absorb_y));
                x += step;
            }

            Diffuse_polyline.StrokeThickness = 2;
            Diffuse_polyline.Stroke = DiffuseColour;
            Diffuse_polyline.Points = Diffuse_points;

            TurbidityGraph.Children.Add(Diffuse_polyline);
            Absorb_polyline.StrokeThickness = 2;
            Absorb_polyline.Stroke = AbsorbColour;
            Absorb_polyline.Points = Absorb_points;

            TurbidityGraph.Children.Add(Absorb_polyline);
        }

        private void Draw_Graph_Axis()
        {
            double margin = 10;
            double xmin = margin;
            double xmax = TurbidityGraph.ActualWidth - margin;
            double xarea = TurbidityGraph.ActualWidth - 2 * margin;
            double ymin = margin;
            double ymax = TurbidityGraph.ActualHeight - margin;
            double yarea = TurbidityGraph.ActualHeight - 2 * margin;
            double xstep = xarea / 10;
            double ystep = yarea / 10;

            //Make the axis
            GeometryGroup axis_geom = new GeometryGroup();
            axis_geom.Children.Add(new LineGeometry(
                new Point(xmin, ymax), new Point(xmax, ymax)));
            axis_geom.Children.Add(new LineGeometry(
                new Point(xmin, ymax), new Point(xmin, ymin)));

            Path axis_path = new Path();
            axis_path.StrokeThickness = 1;
            axis_path.Stroke = Brushes.Black;
            axis_path.Data = axis_geom;

            TurbidityGraph.Children.Add(axis_path);

            // Make the horizontal grid.
            GeometryGroup xgrid_geom = new GeometryGroup();
            for (double x = xmin + xstep; x <= xarea; x += xstep)
            {
                xgrid_geom.Children.Add(new LineGeometry(
                    new Point(x, ymin),
                    new Point(x, ymax)));
            }

            Path xgrid_path = new Path();
            xgrid_path.StrokeThickness = 1;
            xgrid_path.Stroke = Brushes.Gray;
            xgrid_path.Data = xgrid_geom;

            TurbidityGraph.Children.Add(xgrid_path);

            // Make the vertical grid.
            GeometryGroup ygrid_geom = new GeometryGroup();
            for (double y = ymin + ystep; y <= yarea; y += ystep)
            {
                ygrid_geom.Children.Add(new LineGeometry(
                    new Point(xmin, y),
                    new Point(xmax, y)));
            }

            Path ygrid_path = new Path();
            ygrid_path.StrokeThickness = 1;
            ygrid_path.Stroke = Brushes.Gray;
            ygrid_path.Data = ygrid_geom;

            TurbidityGraph.Children.Add(ygrid_path);
        }

        private void LaserMaxTuningTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double lmt = Convert.ToDouble(LaserMaxTuningTextBox.Text);
                if (lmt > 100 && lmt < 100000)
                {
                    TurbidityDataset.LaserMax = lmt;
                }
            }
            catch (Exception) {}
        }

        private void TurbidityGraph_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawTurbidityGraph();
        }
    }

}

