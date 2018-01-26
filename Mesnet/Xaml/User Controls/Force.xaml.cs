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
    /// Interaction logic for Force.xaml
    /// </summary>
    public partial class Force : UserControl
    {
        public Force(PiecewisePoly forceppoly, Beam beam, int c = 200)
        {
            _beam = beam;
            _forceppoly = forceppoly;
            _length = _beam.Length;
            _max = _forceppoly.Max;

            if (_max < 0)
            {
                if (Math.Abs(_max) < 0.00001)
                {
                    _max = 0;
                    coeff = 1;
                }
                else
                {
                    coeff = 200 / Global.MaxForce;
                }
            }
            else if (_max == 0)
            {
                coeff = 1;
            }
            else
            {
                coeff = 200 / Global.MaxForce;
            }
            InitializeComponent();

            Draw(c);
        }

        private double _max;

        private Beam _beam;

        private double _length;

        public PiecewisePoly _forceppoly;

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
            forcecanvas.Children.Clear();

            coeff = c / Global.MaxForce;
            double calculated = 0;
            double value = 0;

            var leftpoints = new PointCollection();
            leftpoints.Add(new Point(0, coeff * _forceppoly.Calculate(0)));
            leftpoints.Add(new Point(0, 0));
            var leftspline = new CardinalSplineShape(leftpoints);
            leftspline.Stroke = color;
            leftspline.StrokeThickness = 1;
            forcecanvas.Children.Add(leftspline);

            Point lastpoint = new Point(0, 0);

            var lastcollection = new PointCollection();
            
            foreach (Poly poly in _forceppoly)
            {
                var points = new PointCollection();

                for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                {
                    calculated = coeff * poly.Calculate(i / 100);
                    if (points.Count == 0)
                    {
                        var point = new Point(i, calculated);
                        points.Add(point);
                    }
                    else
                    {
                        points.Add(new Point(i, calculated));
                    }
                }

                if (lastcollection != null && lastcollection.Count > 0)
                {
                    if (lastcollection.Last().Y != points.First().Y)
                    {
                        var interpoints = new PointCollection();
                        interpoints.Add(lastcollection.Last());
                        interpoints.Add(points.First());
                        var interspline = new CardinalSplineShape(interpoints);
                        interspline.Stroke = color;
                        interspline.StrokeThickness = 1;
                        forcecanvas.Children.Add(interspline);
                    }
                }

                lastpoint = points.Last();
                _spline = new CardinalSplineShape(points);
                _spline.Stroke = color;
                _spline.StrokeThickness = 1;
                _spline.MouseMove += _mw.forcemousemove;
                _spline.MouseEnter += _mw.mouseenter;
                _spline.MouseLeave += _mw.mouseleave;
                forcecanvas.Children.Add(_spline);

                lastcollection = points;
            }

            var rightpoints = new PointCollection();
            var point1 = new Point(100 * _length, coeff * _forceppoly.Calculate(_length));
            rightpoints.Add(point1);
            var point2 = new Point(100 * _length, 0);
            rightpoints.Add(point2);
            var rightspline = new CardinalSplineShape(rightpoints);
            rightspline.Stroke = color;
            rightspline.StrokeThickness = 1;
            forcecanvas.Children.Add(rightspline);

            double max = _forceppoly.Max;
            double maxlocation = _forceppoly.MaxLocation;
            double min = _forceppoly.Min;
            double minlocation = _forceppoly.MinLocation;

            starttext = new TextBlock();
            _beam.upcanvas.Children.Add(starttext);
            starttext.Text = Math.Round(_forceppoly.Calculate(0), 1) + " kN";
            starttext.Foreground = color;
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            RotateAround(starttext);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = coeff * _forceppoly.Calculate(0);

            if (calculated > 0)
            {
                Canvas.SetTop(starttext, -calculated - starttext.Height);
            }
            else
            {
                Canvas.SetTop(starttext, -calculated);
            }
            
            if (minlocation != 0 && minlocation != _length)
            {
                mintext = new TextBlock();
                mintext.Text = Math.Round(min, 1) + " kN";
                mintext.Foreground = color;
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext);

                _beam.upcanvas.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = coeff * min;
                value = calculated;

                if (calculated > 0)
                {
                    Canvas.SetTop(mintext, -calculated - mintext.Height);
                }
                else
                {
                    Canvas.SetTop(mintext, -calculated);
                }

                var minpoints = new PointCollection();
                minpoints.Add(new Point(minlocation * 100, 0));
                minpoints.Add(new Point(minlocation * 100, calculated));
                var minspline = new CardinalSplineShape(minpoints);
                minspline.Stroke = color;
                forcecanvas.Children.Add(minspline);
            }

            if (maxlocation != 0 && maxlocation != _length)
            {
                maxtext = new TextBlock();

                maxtext.Text = Math.Round(max, 1) + " kN";
                maxtext.Foreground = color;
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext);

                _beam.upcanvas.Children.Add(maxtext);

                Canvas.SetLeft(maxtext, maxlocation * 100 - maxtext.Width / 2);

                calculated = coeff * max;
                value = calculated;

                if (calculated > 0)
                {
                    Canvas.SetTop(maxtext, -calculated - maxtext.Height);
                }
                else
                {
                    Canvas.SetTop(maxtext, -calculated);
                }

                var maxpoints = new PointCollection();
                maxpoints.Add(new Point(maxlocation * 100, 0));
                maxpoints.Add(new Point(maxlocation * 100, calculated));
                var maxspline = new CardinalSplineShape(maxpoints);
                maxspline.Stroke = color;
                forcecanvas.Children.Add(maxspline);
            }

            endtext = new TextBlock();
            _beam.upcanvas.Children.Add(endtext);
            endtext.Text = Math.Round(_forceppoly.Calculate(_beam.Length), 1) + " kN";
            endtext.Foreground = color;
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = coeff * _forceppoly.Calculate(_beam.Length);

            if (calculated > 0)
            {
                Canvas.SetTop(endtext, -calculated - endtext.Height);
            }
            else
            {
                Canvas.SetTop(endtext, -calculated);
            }
        }

        public PiecewisePoly ForcePpoly
        {
            get { return _forceppoly; }
            set { _forceppoly = value; }
        }

        private double coeff;

        private SolidColorBrush color = new SolidColorBrush(Colors.Blue);

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
