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

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

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
                spline.MouseMove += _mw.distloadmousemove;
                spline.MouseEnter += _mw.mouseenter;
                spline.MouseLeave += _mw.mouseleave;
                loadcanvas.Children.Add(spline);
            }
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
    }
}
