/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/

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
    /// Interaction logic for Inertia.xaml
    /// </summary>
    public partial class Inertia : UserControl
    {
        public Inertia(PiecewisePoly inertiappoly, Beam beam, int c = 200)
        {
            _beam = beam;
            _inertiappoly = inertiappoly;
            _length = _beam.Length;
            _max = _inertiappoly.Max;

            if (_max < 0)
            {
                if (Math.Abs(_max) < 0.00001)
                {
                    _max = 0;
                    coeff = 1;
                }
                else
                {
                    coeff = 200 / Global.MaxInertia;
                }
            }
            else if (_max == 0)
            {
                coeff = 1;
            }
            else
            {
                coeff = 200 / Global.MaxInertia;
            }

            InitializeComponent();

            Draw(c);
        }

        private double _max;

        private double _length;

        private Beam _beam;

        public PiecewisePoly _inertiappoly;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

        public void Draw(int c)
        {
            if (starttext != null)
            {
                _beam.upcanvas.Children.Remove(starttext);
            }
            if (endtext != null)
            {
                _beam.upcanvas.Children.Remove(endtext);
            }
            if (mintext != null)
            {
                _beam.upcanvas.Children.Remove(mintext);
            }
            if (maxtext != null)
            {
                _beam.upcanvas.Children.Remove(maxtext);
            }

            coeff = c / Global.MaxInertia;
            inertiacanvas.Children.Clear();
            double calculated = 0;
            double value = 0;

            var leftpoints = new PointCollection();
            leftpoints.Add(new Point(0, -coeff * _inertiappoly.Calculate(0)));
            leftpoints.Add(new Point(0, 0));
            var leftspline = new CardinalSplineShape(leftpoints);
            leftspline.Stroke = color;
            leftspline.StrokeThickness = 1;
            inertiacanvas.Children.Add(leftspline);

            Point lastpoint = new Point(0, 0);

            foreach (Poly poly in _inertiappoly)
            {
                var points = new PointCollection();
                points.Clear();

                if (!poly.IsLinear())
                {
                    for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                    {
                        calculated = coeff * poly.Calculate(i / 100);
                        value = -calculated;
                        points.Add(new Point(i, value));
                    }
                }
                else
                {
                    calculated = coeff * poly.Calculate(poly.StartPoint);
                    value = -calculated;
                    points.Add(new Point(poly.StartPoint * 100, value));

                    calculated = coeff * poly.Calculate(poly.EndPoint);
                    value = -calculated;
                    points.Add(new Point(poly.EndPoint*100, value));
                }

                lastpoint = points.Last();
                _spline = new CardinalSplineShape(points);
                _spline.Stroke = color;
                _spline.StrokeThickness = 1;
                _spline.MouseMove += _mw.inertiamousemove;
                _spline.MouseEnter += _mw.mouseenter;
                _spline.MouseLeave += _mw.mouseleave;
                inertiacanvas.Children.Add(_spline);
            }

            var rightpoints = new PointCollection();
            var point1 = new Point(100 * _length, -coeff * _inertiappoly.Calculate(_length));
            rightpoints.Add(point1);
            var point2 = new Point(100 * _length, 0);
            rightpoints.Add(point2);
            var rightspline = new CardinalSplineShape(rightpoints);
            rightspline.Stroke = color;
            rightspline.StrokeThickness = 1;
            inertiacanvas.Children.Add(rightspline);

            double max = _inertiappoly.Max;
            double maxlocation = _inertiappoly.MaxLocation;
            double min = _inertiappoly.Min;
            double minlocation = _inertiappoly.MinLocation;

            starttext = new TextBlock();
            _beam.upcanvas.Children.Add(starttext);
            starttext.Text = Math.Round(_inertiappoly.Calculate(0), 1) + " cm^4";
            starttext.Foreground = color;
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            RotateAround(starttext);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = coeff * _inertiappoly.Calculate(0);
            value = calculated;
            Canvas.SetTop(starttext, value);

            if (minlocation != 0 && minlocation != _length)
            {
                mintext = new TextBlock();
                mintext.Text = Math.Round(min, 1) + " cm^4";
                mintext.Foreground = color;
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext);

                _beam.upcanvas.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = coeff * min;
                value = calculated;

                Canvas.SetTop(mintext, value-mintext.Height);

                var minpoints = new PointCollection();
                minpoints.Add(new Point(minlocation * 100, 0));
                minpoints.Add(new Point(minlocation * 100, -coeff * min));
                var minspline = new CardinalSplineShape(minpoints);
                minspline.Stroke = color;
                inertiacanvas.Children.Add(minspline);
            }

            if (maxlocation != 0 && maxlocation != _length)
            {
                maxtext = new TextBlock();

                maxtext.Text = Math.Round(max, 1) + " cm^4";
                maxtext.Foreground = color;
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
                maxspline.Stroke = color;
                inertiacanvas.Children.Add(maxspline);
            }

            endtext = new TextBlock();
            _beam.upcanvas.Children.Add(endtext);
            endtext.Text = Math.Round(_inertiappoly.Calculate(_beam.Length), 1) + " cm^4";
            endtext.Foreground = color;
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = coeff * _inertiappoly.Calculate(_beam.Length);
            value = calculated;
            Canvas.SetTop(endtext, value);
        }

        public PiecewisePoly InertiaPpoly
        {
            get { return _inertiappoly; }
            set { _inertiappoly = value; }
        }

        private double coeff;

        private SolidColorBrush color = new SolidColorBrush(Colors.Indigo);

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                Width = value * 100;
            }
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

        public void RemoveLabels()
        {
            _beam.upcanvas.Children.Remove(starttext);
            _beam.upcanvas.Children.Remove(endtext);
            if (maxtext != null)
            {
                _beam.upcanvas.Children.Remove(maxtext);
            }
            if (mintext != null)
            {
                _beam.upcanvas.Children.Remove(mintext);
            }
        }
    }
}
