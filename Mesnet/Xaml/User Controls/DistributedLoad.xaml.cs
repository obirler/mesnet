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
    along with Mesnet. If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/
using System;
using System.Globalization;
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
        public DistributedLoad(PiecewisePoly ppoly, Beam beam, int c = 200)
        {
            _beam = beam;
            _loadppoly = ppoly;
            _length = beam.Length;

            InitializeComponent();

            Draw(c);
        }

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double _length;

        private PiecewisePoly _loadppoly;

        private TextBlock starttext;

        private TextBlock mintext;

        private TextBlock maxtext;

        private TextBlock endtext;

        private CardinalSplineShape _spline;

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
            loadcanvas.Children.Clear();

            coeff = c / Global.MaxDistLoad;

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
                if (!poly.IsLinear())
                {
                    for (double i = poly.StartPoint * 100; i <= poly.EndPoint * 100; i++)
                    {
                        calculated = coeff * poly.Calculate(i / 100);
                        points.Add(new Point(i, calculated));
                    }
                }
                else
                {
                    calculated = coeff * poly.Calculate(poly.StartPoint);
                    points.Add(new Point(poly.StartPoint * 100, calculated));

                    calculated = coeff * poly.Calculate(poly.EndPoint);
                    points.Add(new Point(poly.EndPoint * 100, calculated));
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

                _spline = new CardinalSplineShape(points);
                _spline.Stroke = new SolidColorBrush(Colors.Black);
                _spline.StrokeThickness = 1;
                _spline.MouseMove += _mw.distloadmousemove;
                _spline.MouseEnter += _mw.mouseenter;
                _spline.MouseLeave += _mw.mouseleave;
                loadcanvas.Children.Add(_spline);
            }

            double max = _loadppoly.Max;
            double maxlocation = _loadppoly.MaxLocation;
            double min = _loadppoly.Min;
            double minlocation = _loadppoly.MinLocation;

            starttext = new TextBlock();
            _beam.upcanvas.Children.Add(starttext);
            starttext.Text = Math.Round(_loadppoly.Calculate(0), 1) + " kN/m";
            starttext.Foreground = new SolidColorBrush(Colors.Black);
            MinSize(starttext);
            starttext.TextAlignment = TextAlignment.Center;
            RotateAround(starttext);
            Canvas.SetLeft(starttext, -starttext.Width / 2);
            calculated = -coeff * _loadppoly.Calculate(0);
            if (calculated > 0)
            {
                Canvas.SetTop(starttext, calculated);
            }
            else
            {
                Canvas.SetTop(starttext, calculated - starttext.Height);
            }

            if (minlocation != 0 && minlocation != _length)
            {
                mintext = new TextBlock();
                mintext.Text = Math.Round(min, 1) + " kNm";
                mintext.Foreground = new SolidColorBrush(Colors.Black);
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext);

                _beam.upcanvas.Children.Add(mintext);

                Canvas.SetLeft(mintext, minlocation * 100 - mintext.Width / 2);

                calculated = -coeff * min;
                if (calculated > 0)
                {
                    Canvas.SetTop(mintext, calculated);
                }
                else
                {
                    Canvas.SetTop(mintext, calculated - mintext.Height);
                }
            }

            if (maxlocation != 0 && maxlocation != _length)
            {
                maxtext = new TextBlock();
                maxtext.Text = Math.Round(max, 1) + " kNm";
                maxtext.Foreground = new SolidColorBrush(Colors.Black);
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext);

                _beam.upcanvas.Children.Add(maxtext);

                Canvas.SetLeft(maxtext, maxlocation * 100 - maxtext.Width / 2);

                calculated = -coeff * max;

                if (calculated > 0)
                {
                    Canvas.SetTop(maxtext, calculated);
                }
                else
                {
                    Canvas.SetTop(maxtext, calculated - maxtext.Height);
                }
            }

            endtext = new TextBlock();
            _beam.upcanvas.Children.Add(endtext);
            endtext.Text = Math.Round(_loadppoly.Calculate(_beam.Length), 1) + " kN/m";
            endtext.Foreground = new SolidColorBrush(Colors.Black);
            MinSize(endtext);
            endtext.TextAlignment = TextAlignment.Center;
            RotateAround(endtext);
            Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
            calculated = -coeff * _loadppoly.Calculate(_beam.Length);
            if (calculated > 0)
            {
                Canvas.SetTop(endtext, calculated);
            }
            else
            {
                Canvas.SetTop(endtext, calculated - endtext.Height);
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
            points.Add(new Point(x - 0.5, y));
            points.Add(new Point(x - 0.5, 5));
            points.Add(new Point(x - 1.7, 5));
            points.Add(new Point(x, 0));
            points.Add(new Point(x + 1.7, 5));
            points.Add(new Point(x + 0.5, 5));
            points.Add(new Point(x + 0.5, y));
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
            points.Add(new Point(x - 0.5, y));
            points.Add(new Point(x - 0.5, - 5));
            points.Add(new Point(x - 1.7, - 5));
            points.Add(new Point(x, 0));
            points.Add(new Point(x + 1.7, - 5));
            points.Add(new Point(x + 0.5, - 5));
            points.Add(new Point(x + 0.5, y));
            var polygon = new Polygon();
            polygon.Points = points;
            polygon.Fill = new SolidColorBrush(Colors.Black);
            loadcanvas.Children.Add(polygon);
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
            if (starttext != null)
            {
                if (_beam.upcanvas.Children.Contains(starttext))
                {
                    _beam.upcanvas.Children.Remove(starttext);
                }
            }

            if (endtext != null)
            {
                if (_beam.upcanvas.Children.Contains(endtext))
                {
                    _beam.upcanvas.Children.Remove(endtext);
                }
            }

            if (mintext != null)
            {
                if (_beam.upcanvas.Children.Contains(mintext))
                {
                    _beam.upcanvas.Children.Remove(mintext);
                }
            }

            if (maxtext != null)
            {
                if (_beam.upcanvas.Children.Contains(maxtext))
                {
                    _beam.upcanvas.Children.Remove(maxtext);
                }
            }
        }
    }
}
