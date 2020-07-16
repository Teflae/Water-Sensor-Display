﻿using System;
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
            const double margin = 10;
            double xmin = margin;
            double xmax = xmin + 30;
            double ymax = TurbidityGraph.Height - margin;
            double ymin = margin;
            const double step = 30;
            // Random until Algorith for data labels is finished other wise it is almost impossible for me to use the python data.
            Brush brushes = Brushes.Red;
            Random rand = new Random();
            int last_y = rand.Next((int)ymin, (int)ymax);
            PointCollection points = new PointCollection();
            for (double x = xmin; x <= xmax; x += step)
            {
                last_y = rand.Next(last_y - 20, last_y + 20);
                if (last_y < ymin) last_y = (int)ymin;
                if (last_y > ymax) last_y = (int)ymax;
                points.Add(new Point(x, last_y));
            }

            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = brushes;
            polyline.Points = points;

            TurbidityGraph.Children.Add(polyline);
            xmin += 30;
        }
    }

}

