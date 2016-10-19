using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mesnet.Classes;
using Mesnet.Classes.Math;
using Mesnet.Classes.Ui;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Load.xaml
    /// </summary>
    public partial class DistributedLoad : UserControl
    {
        public DistributedLoad(PiecewisePoly ppoly, double length)
        {
            _loadppoly = ppoly;

            _length = length;

            _max = ppoly.Max;

            if (_max > 600)
            {
                coeff = 600 / _max;
                Height = 600;
            }
            else if (_max < 0)
            {
                coeff = 1;
                Height = 0;
                Margin = new Thickness(0, -_max, 0, 0);
            }
            else
            {
                coeff = 1;
                Height = _max;
            }

            InitializeComponent();

            draw();
        }

        public DistributedLoad()
        {

        }

        private double _max;

        private double _length;

        private PiecewisePoly _loadppoly;

        private double coeff;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                Width = value * 100;
            }
        }

        public PiecewisePoly LoadPpoly
        {
            get { return _loadppoly; }
        }

        private void Scale()
        {
            loadcanvas.Children.Clear();

            if (_max > 200)
            {
                coeff = 200 / _max;
                Height = 200;
            }
            else if (_max < 0)
            {
                coeff = 1;
                Height = 0;
                Margin = new Thickness(0, -_max, 0, 0);
            }
            else
            {
                coeff = 200 / Global.maxload;
                Height = 200 * _max / Global.maxload;
            }

            draw();
        }

        private void draw()
        {
            double calculated = 0;

            double value = 0;

            foreach (Poly poly in _loadppoly)
            {
                var points = new PointCollection();
                points.Clear();

                double diff = (poly.EndPoint - poly.StartPoint) * 100;

                int arrownumber = 0;

                arrownumber = Convert.ToInt32(Math.Round(diff / 10, 0));

                //draw spline

                for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                {
                    calculated = coeff * poly.Calculate(i / 100);
                    value = _max * coeff - calculated;
                    points.Add(new Point(i, value));
                }

                //draw arrows

                for (int i = 0; i <= arrownumber; i++)
                {
                    double tobecalc = poly.StartPoint * 100 + diff * i / arrownumber;
                    calculated = coeff * poly.Calculate(tobecalc / 100);
                    if (calculated >= 5)
                    {
                        drawarrow(tobecalc, calculated);
                    }
                    else if (calculated <= -5)
                    {
                        drawnegativearrow(tobecalc, calculated);
                    }
                }

                var spline = new CardinalSplineShape(points);
                spline.Stroke = new SolidColorBrush(Colors.Black);
                spline.StrokeThickness = 1;
                spline.MouseMove += mousemove;
                spline.MouseEnter += mouseenter;
                spline.MouseLeave += mouseleave;
                loadcanvas.Children.Add(spline);
            }

            Panel.SetZIndex(viewbox, 1);
        }

        private void drawnegative()
        {
            double calculated = 0;

            double value = 0;

            foreach (Poly poly in _loadppoly)
            {
                var points = new PointCollection();

                points.Clear();

                for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                {
                    calculated = coeff * poly.Calculate(i / 100);
                    value = _max * coeff - calculated;
                    points.Add(new Point(i, value));
                    if (i % 10 == 0 && calculated >= 5)
                    {
                        drawarrow(i, calculated);
                    }
                    else if (i % 10 == 0 && calculated <= -5)
                    {
                        drawnegativearrow(i, calculated - _max);
                    }
                }
                var spline = new CardinalSplineShape(points);
                spline.Stroke = new SolidColorBrush(Colors.Black);
                spline.StrokeThickness = 1;
                spline.MouseMove += mousemove;
                spline.MouseEnter += mouseenter;
                spline.MouseLeave += mouseleave;
                loadcanvas.Children.Add(spline);
            }

            Panel.SetZIndex(viewbox, 1);
        }

        /// <summary>
        /// Draws the arrow under the load spline.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void drawarrow(double x, double y)
        {
            var points = new PointCollection();
            points.Add(new Point(x - 0.5, coeff * _max - y));
            points.Add(new Point(x - 0.5, coeff * _max - 5));
            points.Add(new Point(x - 1.7, coeff * _max - 5));
            points.Add(new Point(x, coeff * _max));
            points.Add(new Point(x + 1.7, coeff * _max - 5));
            points.Add(new Point(x + 0.5, coeff * _max - 5));
            points.Add(new Point(x + 0.5, coeff * _max - y));
            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = new SolidColorBrush(Colors.Black);
            loadcanvas.Children.Add(polygon);
        }

        /// <summary>
        /// Draws the arrow when the load spline is negative.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void drawnegativearrow(double x, double y)
        {
            var points = new PointCollection();
            points.Add(new Point(x - 0.5, coeff * _max - y));
            points.Add(new Point(x - 0.5, coeff * _max + 5));
            points.Add(new Point(x - 1.7, coeff * _max + 5));
            points.Add(new Point(x, coeff * _max));
            points.Add(new Point(x + 1.7, coeff * _max + 5));
            points.Add(new Point(x + 0.5, coeff * _max + 5));
            points.Add(new Point(x + 0.5, coeff * _max - y));
            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = new SolidColorBrush(Colors.Black);
            loadcanvas.Children.Add(polygon);
        }

        private void mousemove(object sender, MouseEventArgs e)
        {
            var mousepoint = e.GetPosition(loadcanvas);
            Canvas.SetTop(viewbox, mousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, mousepoint.X + 12 / Global.Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(_loadppoly.Calculate(mousepoint.X / 100), 4);
            viewbox.Height = 20 / Global.Scale;
        }

        private void mouseenter(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Visible;
        }

        private void mouseleave(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Collapsed;
        }
    }
}
