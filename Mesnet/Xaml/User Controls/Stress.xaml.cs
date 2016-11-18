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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes;
using Mesnet.Classes.Math;
using Mesnet.Classes.Ui;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Stress.xaml
    /// </summary>
    public partial class Stress : UserControl
    {
        public Stress(DotCollection stresslist, Beam beam)
        {
            InitializeComponent();
            _beam = beam;
            _stress = stresslist;
            coeff = 200 / Global.maxstress;

            draw();
        }

        private DotCollection _stress;

        private Beam _beam;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double coeff;

        private void draw()
        {
            bool red = false;
            var points = new PointCollection();
            for (int i = 0; i < _stress.Count; i++)
            {               
                var point = new Point(_stress[i].Key * 100, _stress[i].Value * coeff);
                points.Add(point);
                if (i == 0)
                {
                    if (Math.Abs(_stress[0].Value) >= _beam.MaxAllowableStress)
                    {
                        red = true;
                    }
                    else
                    {
                        red = false;
                    }
                }
                else
                {
                    if (red)
                    {
                        if (Math.Abs(_stress[i].Value) < _beam.MaxAllowableStress)
                        {
                            addspline(points, red);
                            red = false;
                            points = new PointCollection();
                        }
                    }
                    else
                    {
                        if (Math.Abs(_stress[i].Value) >= _beam.MaxAllowableStress)
                        {
                            addspline(points, red);
                            red = true;
                            points = new PointCollection();
                        }
                    }
                }
            }

            if (points.Count > 0)
            {
                addspline(points, red);
            }

            if (_stress[0].Value != 0)
            {
                var leftline = new PointCollection();
                leftline.Add(new Point(0, 0));
                leftline.Add(new Point(0, _stress[0].Value * coeff));

                var leftspline = new CardinalSplineShape(leftline);
                if (Math.Abs(_stress[0].Value) >= _beam.MaxAllowableStress)
                {
                    leftspline.Stroke = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    leftspline.Stroke = new SolidColorBrush(Colors.Lime);
                }
                leftspline.StrokeThickness = 1;
                stresscanvas.Children.Add(leftspline);

                var lefttext = new TextBlock();
                stresscanvas.Children.Add(lefttext);
                lefttext.Text = Math.Round(_stress[0].Value, 1) + " MPa";
                lefttext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(lefttext);
                lefttext.TextAlignment = TextAlignment.Center;
                RotateAround(lefttext);
                if (_stress[0].Value > 0)
                {
                    Canvas.SetTop(lefttext, _stress[0].Value * coeff + lefttext.Height);
                }
                else
                {
                    Canvas.SetTop(lefttext, _stress[0].Value * coeff - lefttext.Height);
                }
                
                Canvas.SetLeft(lefttext, -lefttext.Width/2);
            }

            if (_stress[_stress.Count-1].Value != 0)
            {
                var rightline = new PointCollection();
                rightline.Add(new Point(_beam.Length * 100, 0));
                rightline.Add(new Point(_beam.Length * 100, _stress[_stress.Count-1].Value * coeff));

                var rightspline = new CardinalSplineShape(rightline);
                if (Math.Abs(_stress[_stress.Count - 1].Value) >= _beam.MaxAllowableStress)
                {
                    rightspline.Stroke = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    rightspline.Stroke = new SolidColorBrush(Colors.Lime);
                }
                rightspline.StrokeThickness = 1;
                stresscanvas.Children.Add(rightspline);

                var righttext = new TextBlock();
                stresscanvas.Children.Add(righttext);
                righttext.Text = Math.Round(_stress[_stress.Count-1].Value, 1) + " MPa";
                righttext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(righttext);
                righttext.TextAlignment = TextAlignment.Center;
                RotateAround(righttext);
                if (_stress[_stress.Count - 1].Value > 0)
                {
                    Canvas.SetTop(righttext, _stress[_stress.Count - 1].Value * coeff + righttext.Height);
                }
                else
                {
                    Canvas.SetTop(righttext, _stress[_stress.Count - 1].Value * coeff - righttext.Height);
                }

                Canvas.SetLeft(righttext, _beam.Length * 100 -righttext.Width/2);
            }

            if (_stress.YMaxPosition != 0 && _stress.YMaxPosition != _beam.Length)
            {
                var maxtext = new TextBlock();
                stresscanvas.Children.Add(maxtext);
                maxtext.Text = Math.Round(_stress.YMax, 1) + " MPa";
                maxtext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(maxtext);
                maxtext.TextAlignment = TextAlignment.Center;
                RotateAround(maxtext);
                Canvas.SetTop(maxtext, _stress.YMax * coeff);
                Canvas.SetLeft(maxtext, _stress.YMaxPosition * 100 - maxtext.Width / 2);

                var maxline = new PointCollection();
                maxline.Add(new Point(_stress.YMaxPosition * 100, 0));
                maxline.Add(new Point(_stress.YMaxPosition * 100, _stress.YMax * coeff));

                var maxspline = new CardinalSplineShape(maxline);
                if (_stress.YMax >= _beam.MaxAllowableStress)
                {
                    maxspline.Stroke = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    maxspline.Stroke = new SolidColorBrush(Colors.Lime);
                }
                maxspline.StrokeThickness = 1;
                stresscanvas.Children.Add(maxspline);
            }

            if (_stress.YMinPosition != 0 && _stress.YMinPosition != _beam.Length)
            {
                var mintext = new TextBlock();
                stresscanvas.Children.Add(mintext);
                mintext.Text = Math.Round(_stress.YMin, 1) + " MPa";
                mintext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(mintext);
                mintext.TextAlignment = TextAlignment.Center;
                RotateAround(mintext);
                Canvas.SetTop(mintext, _stress.YMin * coeff);
                Canvas.SetLeft(mintext, _stress.YMinPosition * 100 - mintext.Width / 2);

                var maxline = new PointCollection();
                maxline.Add(new Point(_stress.YMinPosition * 100, 0));
                maxline.Add(new Point(_stress.YMinPosition * 100, _stress.YMin * coeff));

                var maxspline = new CardinalSplineShape(maxline);
                if (Math.Abs(_stress.YMin) >= _beam.MaxAllowableStress)
                {
                    maxspline.Stroke = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    maxspline.Stroke = new SolidColorBrush(Colors.Lime);
                }
                maxspline.StrokeThickness = 1;
                stresscanvas.Children.Add(maxspline);
            }

            if (_stress.YMax >= _beam.MaxAllowableStress)
            {
                var line = new PointCollection();
                line.Add(new Point(0, _beam.MaxAllowableStress * coeff));
                line.Add(new Point(_beam.Length * 100, _beam.MaxAllowableStress * coeff));

                var spline = new CardinalSplineShape(line);
                spline.Stroke = new SolidColorBrush(Colors.Red);
                spline.StrokeThickness = 1;
                stresscanvas.Children.Add(spline);

                var starttext = new TextBlock();
                stresscanvas.Children.Add(starttext);
                starttext.Text = Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                starttext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(starttext);
                starttext.TextAlignment = TextAlignment.Center;
                RotateAround(starttext);

                Canvas.SetTop(starttext, _beam.MaxAllowableStress * coeff - starttext.Height);
                Canvas.SetLeft(starttext, -starttext.Width/2);

                var endtext = new TextBlock();
                stresscanvas.Children.Add(endtext);
                endtext.Text = Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                endtext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(endtext);
                endtext.TextAlignment = TextAlignment.Center;
                RotateAround(endtext);

                Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width/2);
                Canvas.SetTop(endtext, _beam.MaxAllowableStress * coeff - endtext.Height);
            }

            if (Math.Abs(_stress.YMin) >= _beam.MaxAllowableStress)
            {
                var line = new PointCollection();
                line.Add(new Point(0, -_beam.MaxAllowableStress * coeff));
                line.Add(new Point(_beam.Length * 100, -_beam.MaxAllowableStress * coeff));

                var spline = new CardinalSplineShape(line);
                spline.Stroke = new SolidColorBrush(Colors.Red);
                spline.StrokeThickness = 1;
                stresscanvas.Children.Add(spline);

                var starttext = new TextBlock();
                stresscanvas.Children.Add(starttext);
                starttext.Text = Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                starttext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(starttext);
                starttext.TextAlignment = TextAlignment.Center;
                RotateAround(starttext);
                Canvas.SetTop(starttext, -_beam.MaxAllowableStress * coeff);

                Canvas.SetLeft(starttext, -starttext.Width / 2);

                var endtext = new TextBlock();
                stresscanvas.Children.Add(endtext);
                endtext.Text = Math.Round(_beam.MaxAllowableStress, 1) + " MPa";
                endtext.Foreground = new SolidColorBrush(Colors.Red);
                MinSize(endtext);
                endtext.TextAlignment = TextAlignment.Center;
                RotateAround(endtext);

                Canvas.SetLeft(endtext, _beam.Length * 100 - endtext.Width / 2);
                Canvas.SetTop(endtext, -_beam.MaxAllowableStress * coeff);
            }                     
        }

        public double Calculate(double x)
        {
            return _stress.Calculate(x);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        private void addspline(PointCollection points, bool isred)
        {
            var spline = new CardinalSplineShape(points);
            if (isred)
            {
                spline.Stroke = new SolidColorBrush(Colors.Red);
            }
            else
            {
                spline.Stroke = new SolidColorBrush(Colors.Lime);
            }           
            spline.StrokeThickness = 1;
            spline.MouseMove += _mw.stressmousemove;
            spline.MouseEnter += _mw.mouseenter;
            spline.MouseLeave += _mw.mouseleave;
            stresscanvas.Children.Add(spline);
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
    }
}
