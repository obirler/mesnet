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
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MoreLinq;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ConcentratedLoad.xaml
    /// </summary>
    public partial class ConcentratedLoad : UserControl
    {
        public ConcentratedLoad(List<KeyValuePair<double, double>> loads, double length)
        {
            InitializeComponent();

            _loads = loads;

            _length = length;

            _max = _loads.MaxBy(x => x.Value).Value;

            if (_max > 200)
            {
                coeff = 200 / _max;
                Height = 200;
            }
            if (_max < -200)
            {
                coeff = -200 / _max;
                Height = 200;
            }
            else
            {
                if (_max > 0)
                {
                    coeff = 1;
                    Height = _max;
                }
                else
                {
                    coeff = 1;
                    Height = -_max;
                }
            }

            Width = 100 * _length;

            draw();
        }

        /// <summary>
        /// The load list. List<KeyValuePair<xpos, loadmagnitude>>
        /// </summary>
        private List<KeyValuePair<double, double>> _loads;

        private double _max;

        private double _length;

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

        public List<KeyValuePair<double, double>> Loads
        {
            get { return _loads; }
        }

        private void draw()
        {
            foreach (KeyValuePair<double, double> load in _loads)
            {
                DrawArrow(load.Key * 100, load.Value);
            }
        }

        /// <summary>
        /// Draws the arrow under the load spline.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void DrawArrow(double x, double y)
        {
            var points = new PointCollection();

            var tbl = new TextBlock();

            var polygon = new Polygon();

            if (y > 0)
            {
                if (y >= 15)
                {
                    points.Add(new Point(x - 2, coeff * _max - y));
                    points.Add(new Point(x - 2, coeff * _max - 10));
                    points.Add(new Point(x - 5, coeff * _max - 10));
                    points.Add(new Point(x, coeff * _max));
                    points.Add(new Point(x + 5, coeff * _max - 10));
                    points.Add(new Point(x + 2, coeff * _max - 10));
                    points.Add(new Point(x + 2, coeff * _max - y));

                    tbl.Text = y + " kN";
                    UpdateTextBlock(tbl);
                    loadcanvas.Children.Add(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, coeff * _max - y - tbl.Height - 5);
                }
                else
                {
                    points.Add(new Point(x - 2, coeff * _max - 15));
                    points.Add(new Point(x - 2, coeff * _max - 10));
                    points.Add(new Point(x - 5, coeff * _max - 10));
                    points.Add(new Point(x, coeff * _max));
                    points.Add(new Point(x + 5, coeff * _max - 10));
                    points.Add(new Point(x + 2, coeff * _max - 10));
                    points.Add(new Point(x + 2, coeff * _max - 15));

                    tbl.Text = y + " kN";
                    UpdateTextBlock(tbl);
                    loadcanvas.Children.Add(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, coeff * _max - 15 - tbl.Height - 5);
                }

                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                loadcanvas.Children.Add(polygon);
            }
            else
            {
                if (y <= -15)
                {
                    points.Add(new Point(x - 2, -coeff * _max - y));
                    points.Add(new Point(x - 2, -coeff * _max + 10));
                    points.Add(new Point(x - 5, -coeff * _max + 10));
                    points.Add(new Point(x, -coeff * _max));
                    points.Add(new Point(x + 5, -coeff * _max + 10));
                    points.Add(new Point(x + 2, -coeff * _max + 10));
                    points.Add(new Point(x + 2, -coeff * _max - y));

                    tbl.Text = y + " kN";
                    UpdateTextBlock(tbl);
                    loadcanvas.Children.Add(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -coeff * _max - y + 5);
                }
                else
                {
                    points.Add(new Point(x - 2, -coeff * _max + 15));
                    points.Add(new Point(x - 2, -coeff * _max + 10));
                    points.Add(new Point(x - 5, -coeff * _max + 10));
                    points.Add(new Point(x, -coeff * _max));
                    points.Add(new Point(x + 5, -coeff * _max + 10));
                    points.Add(new Point(x + 2, -coeff * _max + 10));
                    points.Add(new Point(x + 2, -coeff * _max + 15));

                    tbl.Text = y + " kN";
                    UpdateTextBlock(tbl);
                    loadcanvas.Children.Add(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -coeff * _max + 20);
                }

                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                loadcanvas.Children.Add(polygon);
            }

        }

        private void UpdateTextBlock(TextBlock textBlock)
        {
            string candidate = textBlock.Text;
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);

            textBlock.Width = formattedText.Width;
            textBlock.Height = formattedText.Height;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
