using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes;
using Mesnet.Classes.Math;
using Mesnet.Classes.Ui;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Moment.xaml
    /// </summary>
    public partial class Moment : UserControl
    {
        public Moment(PiecewisePoly momentppoly, Beam beam)
        {
            _beam = beam;
            _momentppoly = momentppoly;

            _length = beam.Length;

            _max = _momentppoly.Max;

            if (_max < 0)
            {
                if (Math.Abs(_max) < 0.00001)
                {
                    _max = 0;
                    coeff = 1;
                    //Height = 200;
                }
                else
                {
                    coeff = -1 * 200 / Global.maxmoment;
                    //Height = 200 * _max / Global.maxmoment;
                }

            }
            else if (_max == 0)
            {
                coeff = 1;
                //Height = 200;
            }
            else
            {
                coeff = 200 / Global.maxmoment;
                //Height = 200 * _max / Global.maxmoment;
            }

            //Height = 0;

            //coeff = 1;

            InitializeComponent();

            draw();
        }

        public Moment()
        {

        }

        private Beam _beam;

        private double _max;

        private double _length;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private PiecewisePoly _momentppoly;

        public PiecewisePoly MomentPpoly
        {
            get { return _momentppoly; }
            set { _momentppoly = value; }
        }

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

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

        private void draw()
        {
            double calculated = 0;

            double value = 0;

            var leftpoints = new PointCollection();
            leftpoints.Add(new Point(0, -coeff * _momentppoly.Calculate(0)));
            leftpoints.Add(new Point(0, 0));
            var leftspline = new CardinalSplineShape(leftpoints);
            leftspline.Stroke = new SolidColorBrush(Colors.Red);
            leftspline.StrokeThickness = 1;
            momentcanvas.Children.Add(leftspline);

            Point lastpoint = new Point(0, 0);

            foreach (Poly poly in _momentppoly)
            {
                var points = new PointCollection();

                points.Clear();

                for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                {
                    calculated = coeff * poly.Calculate(i / 100);
                    value = -calculated;
                    if (points.Count == 0)
                    {
                        var point = new Point(i, value);
                        points.Add(point);
                    }
                    else
                    {
                        points.Add(new Point(i, value));
                    }
                }

                lastpoint = points.Last();
                var spline = new CardinalSplineShape(points);
                spline.Stroke = new SolidColorBrush(Colors.Red);
                spline.StrokeThickness = 1;
                spline.MouseMove += _mw.momentmousemove;
                spline.MouseEnter += _mw.mouseenter;
                spline.MouseLeave += _mw.mouseleave;
                momentcanvas.Children.Add(spline);
            }

            var rightpoints = new PointCollection();
            var point1 = new Point(100 * _length, -coeff * _momentppoly.Calculate(_length));
            rightpoints.Add(point1);
            var point2 = new Point(100 * _length, 0);
            rightpoints.Add(point2);
            var rightspline = new CardinalSplineShape(rightpoints);
            rightspline.Stroke = new SolidColorBrush(Colors.Red);
            rightspline.StrokeThickness = 1;
            momentcanvas.Children.Add(rightspline);

            double max = _momentppoly.Max;
            double maxlocation = _momentppoly.MaxLocation;
            double min = _momentppoly.Min;
            double minlocation = _momentppoly.MinLocation;

            starttext = new TextBlock();
            _beam.upcanvas.Children.Add(starttext);
            starttext.Text = Math.Round(_momentppoly.Calculate(0), 1).ToString();
            starttext.Foreground = new SolidColorBrush(Colors.Red);
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            //starttext.UpdateLayout();
            RotateAround(starttext);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = coeff * _momentppoly.Calculate(0);
            value = calculated;
            Canvas.SetTop(starttext, value - starttext.Height);

            if (minlocation != 0 && minlocation != _length)
            {
                mintext = new TextBlock();
                mintext.Text = Math.Round(min, 1).ToString();
                mintext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext);

                _beam.upcanvas.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = coeff * min;
                value = calculated;

                Canvas.SetTop(mintext, value);

                var minpoints = new PointCollection();
                minpoints.Add(new Point(minlocation * 100, 0));
                minpoints.Add(new Point(minlocation * 100, -coeff * min));
                var minspline = new CardinalSplineShape(minpoints);
                minspline.Stroke = new SolidColorBrush(Colors.Red);
                momentcanvas.Children.Add(minspline);
            }

            if (maxlocation != 0 && maxlocation != _length)
            {
                maxtext = new TextBlock();

                maxtext.Text = Math.Round(max, 1).ToString();
                maxtext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext);

                _beam.upcanvas.Children.Add(maxtext);

                Canvas.SetLeft(maxtext, maxlocation * 100 - maxtext.Width / 2);

                calculated = coeff * max;
                value = calculated;

                Canvas.SetTop(maxtext, value);

                var maxpoints = new PointCollection();
                maxpoints.Add(new Point(maxlocation * 100, 0));
                maxpoints.Add(new Point(maxlocation * 100, -coeff * max));
                var maxspline = new CardinalSplineShape(maxpoints);
                maxspline.Stroke = new SolidColorBrush(Colors.Red);
                momentcanvas.Children.Add(maxspline);
            }

            endtext = new TextBlock();
            _beam.upcanvas.Children.Add(endtext);
            endtext.Text = Math.Round(_momentppoly.Calculate(_beam.Length), 1).ToString();
            endtext.Foreground = new SolidColorBrush(Colors.Red);
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = coeff * _momentppoly.Calculate(_beam.Length);
            value = calculated;
            Canvas.SetTop(endtext, value - endtext.Height);
        }

        private void MinSize(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);
            textBlock.Width = formattedText.Width;
            textBlock.Height = formattedText.Height;
        }

        private void RotateAround(TextBlock textBlock)
        {
            var rotate = new RotateTransform();
            rotate.CenterX = textBlock.Width / 2;
            rotate.CenterY = textBlock.Height / 2;
            rotate.Angle = -_beam.Angle;
            textBlock.RenderTransform = rotate;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            starttext.Visibility = Visibility.Visible;

            if (mintext != null)
            {
                mintext.Visibility = Visibility.Visible;
            }

            if (maxtext != null)
            {
                maxtext.Visibility = Visibility.Visible;
            }

            endtext.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;

            starttext.Visibility = Visibility.Collapsed;

            if (mintext != null)
            {
                mintext.Visibility = Visibility.Collapsed;
            }

            if (maxtext != null)
            {
                maxtext.Visibility = Visibility.Collapsed;
            }

            endtext.Visibility = Visibility.Collapsed;
        }
    }
}
