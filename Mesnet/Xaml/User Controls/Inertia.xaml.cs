using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.Math;
using Mesnet.Classes.Ui;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Inertia.xaml
    /// </summary>
    public partial class Inertia : UserControl
    {
        public Inertia(PiecewisePoly inertiappoly, double length)
        {
            _inertiappoly = inertiappoly;

            _length = length;

            _max = _inertiappoly.Max;

            coeff = 100 / _max;

            Height = 100;

            InitializeComponent();

            draw();
        }

        private double _max;

        private double _length;

        public PiecewisePoly _inertiappoly;

        private void draw()
        {
            double calculated = 0;

            double value = 0;

            var leftpoints = new PointCollection();
            leftpoints.Add(new Point(0, 0));
            leftpoints.Add(new Point(0, coeff * _inertiappoly.Calculate(0)));
            var leftspline = new CardinalSplineShape(leftpoints);
            leftspline.Stroke = new SolidColorBrush(Colors.Green);
            leftspline.StrokeThickness = 1;
            inertiacanvas.Children.Add(leftspline);

            foreach (Poly poly in _inertiappoly)
            {
                var points = new PointCollection();
                points.Clear();

                for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                {
                    calculated = (coeff * poly.Calculate(i / 100));
                    points.Add(new Point(i, calculated));
                }
                var spline = new CardinalSplineShape(points);
                spline.Stroke = new SolidColorBrush(Colors.Green);
                spline.StrokeThickness = 1;
                inertiacanvas.Children.Add(spline);
            }

            var rightpoints = new PointCollection();
            var point1 = new Point(100 * _length, coeff * _inertiappoly.Calculate(0));
            rightpoints.Add(point1);
            var point2 = new Point(100 * _length, coeff * _inertiappoly.Calculate(100 * _length));
            rightpoints.Add(point2);
            var rightspline = new CardinalSplineShape(rightpoints);
            rightspline.Stroke = new SolidColorBrush(Colors.Green);
            rightspline.StrokeThickness = 1;
            inertiacanvas.Children.Add(rightspline);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public PiecewisePoly InertiaPpoly
        {
            get { return _inertiappoly; }
            set { _inertiappoly = value; }
        }

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
    }
}
