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
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Mesnet.Classes.Math
{
    public class TransformGeometry
    {        
        public TransformGeometry(Point origin, double height, double width)
        {
            Height = height;
            Width = width;
            Center = origin;

            BottomLeft = new Point(Center.X - Width / 2, Center.Y - Height / 2);
            BottomRight = new Point(Center.X + Width / 2, Center.Y - Height / 2);
            TopLeft = new Point(Center.X - Width / 2, Center.Y + Height / 2);
            TopRight = new Point(Center.X + Width / 2, Center.Y + Height / 2);
        }

        public TransformGeometry(Point tl, Point tr, Point br, Point bl, Canvas canvas)
        {
            Height = tr.X-tl.X;
            Width = bl.Y-tl.Y;
            _canvas = canvas;
            Center = new Point((tl.X+tr.X+br.X+bl.X)/4, (tl.Y + tr.Y + br.Y + bl.Y) / 4);

            BottomLeft = bl;
            BottomRight = br;
            TopLeft = tl;
            TopRight = tr;

            tle = new Ellipse();
            tre = new Ellipse();
            bre = new Ellipse();
            ble = new Ellipse();
        }

        public TransformGeometry(Beam beam, Canvas canvas)
        {
            Height = beam.Height;
            Width = beam.Width;
            _canvas = canvas;
            Center = new Point(Canvas.GetLeft(beam) + Width/2, Canvas.GetTop(beam) + Height / 2);

            BottomLeft = new Point(Center.X - Width / 2, Center.Y - Height / 2);
            BottomRight = new Point(Center.X + Width / 2, Center.Y - Height / 2);
            TopLeft = new Point(Center.X - Width / 2, Center.Y + Height / 2);
            TopRight = new Point(Center.X + Width / 2, Center.Y + Height / 2);
        }

        public double Height { get; set; }

        public double Width { get; set; }

        public double Rotation { get; set; }

        public Point Center;

        public Point TopLeft;

        public Point TopRight;

        public Point BottomLeft;

        public Point BottomRight;

        private Ellipse tle;

        private Ellipse tre;

        private Ellipse bre;

        private Ellipse ble;

        private Canvas _canvas;

        private List<Point> poly;

        private bool _ellipsevisible = false;

        public void Update(Beam beam)
        {
            Height = beam.Height;
            Width = beam.Width;
            Center = new Point(Canvas.GetLeft(beam) + Width / 2, Canvas.GetTop(beam) + Height / 2);

            BottomLeft = new Point(Center.X - Width / 2, Center.Y - Height / 2);
            BottomRight = new Point(Center.X + Width / 2, Center.Y - Height / 2);
            TopLeft = new Point(Center.X - Width / 2, Center.Y + Height / 2);
            TopRight = new Point(Center.X + Width / 2, Center.Y + Height / 2);
        }

        private void Move(Point from, Point to)
        {
            InitCorners(new Point((to.X - from.X), (to.Y - from.Y)));
            from.X = from.X + (to.X - from.X);
            from.Y = from.Y + (to.Y - from.Y);
        }

        public void Move(Vector delta)
        {
            Center.X = Center.X + delta.X;
            Center.Y = Center.Y + delta.Y;

            BottomLeft.X = BottomLeft.X + delta.X;
            BottomLeft.Y = BottomLeft.Y + delta.Y;

            BottomRight.X = BottomRight.X + delta.X;
            BottomRight.Y = BottomRight.Y + delta.Y;

            TopLeft.X = TopLeft.X + delta.X;
            TopLeft.Y = TopLeft.Y + delta.Y;

            TopRight.X = TopRight.X + delta.X;
            TopRight.Y = TopRight.Y + delta.Y;
        }

        private void MoveFromCenter(Point c)
        {
            InitCorners(new Point((c.X - Center.X), (c.Y - Center.Y)));
            Center.X = Center.X + (c.X - Center.X);
            Center.Y = Center.Y + (c.Y - Center.Y);
        }

        private void InitCorners(Point c)
        {
            BottomRight.X = (BottomRight.X + c.X);
            BottomRight.Y = (BottomRight.Y + c.Y);

            BottomLeft.X = (BottomLeft.X + c.X);
            BottomLeft.Y = (BottomLeft.Y + c.Y);

            TopRight.X = (TopRight.X + c.X);
            TopRight.Y = (TopRight.Y + c.Y);

            TopLeft.X = (TopLeft.X + c.X);
            TopLeft.Y = (TopLeft.Y + c.Y);
        }

        public void Rotate(Point rotationcenter, double degree)
        {
            double qtyRadians = degree * System.Math.PI/180;
            //Move center to origin
            Point temp_orig = new Point(rotationcenter.X, rotationcenter.Y);
            Move(new Point(rotationcenter.X, rotationcenter.Y), new Point(0, 0));

            BottomRight = RotatePoint(BottomRight, qtyRadians);
            TopRight = RotatePoint(TopRight, qtyRadians);
            BottomLeft = RotatePoint(BottomLeft, qtyRadians);
            TopLeft = RotatePoint(TopLeft, qtyRadians);

            //Move center back
            Move(new Point(0, 0), temp_orig);
        }

        public void RotateAboutCenter(double degree)
        {
            double qtyRadians = degree * System.Math.PI / 180;
            //Move center to origin
            Point temp_orig = new Point(Center.X, Center.Y);
            MoveFromCenter(new Point(0, 0));

            BottomRight = RotatePoint(BottomRight, qtyRadians);
            TopRight = RotatePoint(TopRight, qtyRadians);
            BottomLeft = RotatePoint(BottomLeft, qtyRadians);
            TopLeft = RotatePoint(TopLeft, qtyRadians);
           
            //Move center back
            MoveFromCenter(temp_orig);
            //drawrectcorners(5);
        }

        Point RotatePoint(Point p, double qtyRadians)
        {
            Point temb_br = new Point(p.X, p.Y);
            p.X = temb_br.X * System.Math.Cos(qtyRadians) - temb_br.Y * System.Math.Sin(qtyRadians);
            p.Y = temb_br.Y * System.Math.Cos(qtyRadians) + temb_br.X * System.Math.Sin(qtyRadians);

            return p;
        }

        public void ShowCorners(double radius)
        {
            if (_canvas.Children.Contains(tle))
            {
                _canvas.Children.Remove(tle);
            }

            if (_canvas.Children.Contains(tre))
            {
                _canvas.Children.Remove(tre);
            }

            if (_canvas.Children.Contains(bre))
            {
                _canvas.Children.Remove(bre);
            }

            if (_canvas.Children.Contains(ble))
            {
                _canvas.Children.Remove(ble);
            }

            tle.Width = radius;
            tle.Height = radius;
            tle.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(tle);
            Canvas.SetLeft(tle, this.TopLeft.X - radius / 2);
            Canvas.SetTop(tle, this.TopLeft.Y - radius / 2);

            
            tre.Width = radius;
            tre.Height = radius;
            tre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(tre);
            Canvas.SetLeft(tre, this.TopRight.X - radius / 2);
            Canvas.SetTop(tre, this.TopRight.Y - radius / 2);

            
            bre.Width = radius;
            bre.Height = radius;
            bre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(bre);
            Canvas.SetLeft(bre, this.BottomRight.X - radius / 2);
            Canvas.SetTop(bre, this.BottomRight.Y - radius / 2);

            ble.Width = radius;
            ble.Height = radius;
            ble.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ble);
            Canvas.SetLeft(ble, this.BottomLeft.X - radius / 2);
            Canvas.SetTop(ble, this.BottomLeft.Y - radius / 2);

            _ellipsevisible = true;
        }

        public void HideCorners()
        {
            if (_ellipsevisible)
            {
                if (_canvas.Children.Contains(tle))
                {
                    _canvas.Children.Remove(tle);
                }

                if (_canvas.Children.Contains(tre))
                {
                    _canvas.Children.Remove(tre);
                }

                if (_canvas.Children.Contains(bre))
                {
                    _canvas.Children.Remove(bre);
                }

                if (_canvas.Children.Contains(ble))
                {
                    _canvas.Children.Remove(ble);
                }
                _ellipsevisible = false;
            }
        }

        public void drawrectcorners(double radius)
        {
            var ellipse1 = new Ellipse();
            ellipse1.Width = radius;
            ellipse1.Height = radius;
            ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100,255,255,0));

            _canvas.Children.Add(ellipse1);
            Canvas.SetLeft(ellipse1, this.TopLeft.X - radius/2);
            Canvas.SetTop(ellipse1, this.TopLeft.Y - radius/2);

            var ellipse2 = new Ellipse();
            ellipse2.Width = radius;
            ellipse2.Height = radius;
            ellipse2.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ellipse2);
            Canvas.SetLeft(ellipse2, this.TopRight.X - radius/2);
            Canvas.SetTop(ellipse2, this.TopRight.Y - radius/2);

            var ellipse3 = new Ellipse();
            ellipse3.Width = radius;
            ellipse3.Height = radius;
            ellipse3.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ellipse3);
            Canvas.SetLeft(ellipse3, this.BottomRight.X - radius/2);
            Canvas.SetTop(ellipse3, this.BottomRight.Y - radius/2);

            var ellipse4 = new Ellipse();
            ellipse4.Width = radius;
            ellipse4.Height = radius;
            ellipse4.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ellipse4);
            Canvas.SetLeft(ellipse4, this.BottomLeft.X - radius/2);
            Canvas.SetTop(ellipse4, this.BottomLeft.Y - radius/2);

            var ellipse5 = new Ellipse();
            ellipse5.Width = radius;
            ellipse5.Height = radius;
            ellipse5.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));

            _canvas.Children.Add(ellipse5);
            Canvas.SetLeft(ellipse5, this.LeftPoint.X - radius / 2);
            Canvas.SetTop(ellipse5, this.LeftPoint.Y - radius / 2);

            var ellipse6 = new Ellipse();
            ellipse6.Width = radius;
            ellipse6.Height = radius;
            ellipse6.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));

            _canvas.Children.Add(ellipse6);
            Canvas.SetLeft(ellipse6, this.RightPoint.X - radius / 2);
            Canvas.SetTop(ellipse6, this.RightPoint.Y - radius / 2);
        }

        public bool IsInside(Point point)
        {
            var list = new List<PointF>();
            list.Add(new PointF((float)TopLeft.X, (float)TopLeft.Y));
            list.Add(new PointF((float)TopRight.X, (float)TopRight.Y));
            list.Add(new PointF((float)BottomRight.X, (float)BottomRight.Y));
            list.Add(new PointF((float)BottomLeft.X, (float)BottomLeft.Y));

            var polygon = new TPolygon(list.ToArray());

            var check = polygon.PointInPolygon((float) point.X, (float) point.Y);

            if (check)
            {
                MyDebug.WriteInformation("IsInside", "the point is inside of the rectangle");
            }
            else
            {
                MyDebug.WriteInformation("IsInside", "the point is outside of the rectangle");
            }

            return check;
        }

        public Point LeftPoint
        {
            get
            {
                var x = (TopLeft.X + BottomLeft.X)/2;
                var y = (TopLeft.Y + BottomLeft.Y) / 2;
                return new Point(x, y);
            }
        }

        public Point RightPoint
        {
            get
            {
                var x = (TopRight.X + BottomRight.X) / 2;
                var y = (TopRight.Y + BottomRight.Y) / 2;
                return new Point(x, y);
            }
        }
    }
}
