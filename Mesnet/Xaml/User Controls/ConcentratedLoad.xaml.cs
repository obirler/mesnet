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
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Mesnet.Classes;
using MoreLinq;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ConcentratedLoad.xaml
    /// </summary>
    public partial class ConcentratedLoad : UserControl
    {
        public ConcentratedLoad(List<KeyValuePair<double, double>> loads, Beam beam, int c = 200)
        {           
            _beam = beam;

            _loads = loads;

            _length = _beam.Length;

            _max = _loads.MaxBy(x => x.Value).Value;

            if (_max < 0)
            {
                if (Math.Abs(_max) < 0.00001)
                {
                    _max = 0;
                    coeff = 1;
                }
                else
                {
                    coeff = 200 / Global.MaxConcLoad;
                }
            }
            else if (_max == 0)
            {
                coeff = 1;
            }
            else
            {
                coeff = 200 / Global.MaxConcLoad;
            }

            InitializeComponent();

            _labellist = new List<TextBlock>();

            Draw(c);
        }

        private Beam _beam;

        /// <summary>
        /// The load list. List<KeyValuePair<xpos, loadmagnitude>>
        /// </summary>
        private List<KeyValuePair<double, double>> _loads;

        private List<TextBlock> _labellist;

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

        public void Draw(int c)
        {
            coeff = c / Global.MaxConcLoad;
            loadcanvas.Children.Clear();
            RemoveLabels();
            foreach (KeyValuePair<double, double> load in _loads)
            {
                DrawArrow(load.Key * 100, load.Value, coeff);
            }
        }

        /// <summary>
        /// Draws the arrow under the load spline.
        /// </summary>
        /// <param name="x">The upper x point of the arrow.</param>
        /// <param name="y">The upper y point of the arrow.</param>
        private void DrawArrow(double x, double y, double c)
        {                  
            var points = new PointCollection();
            var tbl = new TextBlock();
            var polygon = new Polygon();

            if (y > 0)
            {
                if (c*y >= 15)
                {
                    points.Add(new Point(x - 2, c*y));
                    points.Add(new Point(x - 2, 10));
                    points.Add(new Point(x - 5, 10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, 10));
                    points.Add(new Point(x + 2, 10));
                    points.Add(new Point(x + 2, c*y));

                    tbl.Text = y + " kN";
                    _beam.upcanvas.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -c*y - tbl.Height);
                }
                else
                {
                    points.Add(new Point(x - 2, 15));
                    points.Add(new Point(x - 2, 10));
                    points.Add(new Point(x - 5,  10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, 10));
                    points.Add(new Point(x + 2, 10));
                    points.Add(new Point(x + 2, 15));

                    tbl.Text = y + " kN";
                    _beam.upcanvas.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -15 - tbl.Height);
                }            

                _labellist.Add(tbl);
          
                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                loadcanvas.Children.Add(polygon);
                Canvas.SetLeft(polygon, 0);
                Canvas.SetTop(polygon, 0);
            }
            else
            {
                if (c*y <= -15)
                {
                    points.Add(new Point(x - 2, c*y));
                    points.Add(new Point(x - 2, - 10));
                    points.Add(new Point(x - 5, - 10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, - 10));
                    points.Add(new Point(x + 2, - 10));
                    points.Add(new Point(x + 2, c*y));

                    tbl.Text = y + " kN";
                    _beam.upcanvas.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, -c*y + tbl.Height / 4);
                }
                else
                {
                    points.Add(new Point(x - 2, - 15));
                    points.Add(new Point(x - 2, - 10));
                    points.Add(new Point(x - 5, - 10));
                    points.Add(new Point(x, 0));
                    points.Add(new Point(x + 5, - 10));
                    points.Add(new Point(x + 2, - 10));
                    points.Add(new Point(x + 2, - 15));

                    tbl.Text = y + " kN";
                    _beam.upcanvas.Children.Add(tbl);
                    MinSize(tbl);
                    tbl.TextAlignment = TextAlignment.Center;
                    RotateAround(tbl);

                    Canvas.SetLeft(tbl, x - tbl.Width / 2);
                    Canvas.SetTop(tbl, 15 + tbl.Height / 4);
                }

                _labellist.Add(tbl);

                polygon.Points = points;
                polygon.Fill = new SolidColorBrush(Colors.Black);
                loadcanvas.Children.Add(polygon);
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

            foreach (TextBlock label in _labellist)
            {
                label.Visibility = Visibility.Visible;
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;

            foreach (TextBlock label in _labellist)
            {
                label.Visibility = Visibility.Collapsed;
            }
        }

        public void RemoveLabels()
        {
            foreach (TextBlock label in _labellist)
            {
                _beam.upcanvas.Children.Remove(label);
            }
            _labellist.Clear();
        }
    }
}
