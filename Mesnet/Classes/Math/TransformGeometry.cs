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
        public TransformGeometry(Point tl, Point tr, Point br, Point bl, Canvas canvas)
        {
            Height = System.Math.Sqrt(System.Math.Pow(tl.X - bl.X, 2)+ System.Math.Pow(tl.Y - bl.Y, 2));
            Width = System.Math.Sqrt(System.Math.Pow(tl.X - tr.X, 2) + System.Math.Pow(tl.Y - tr.Y, 2));
            _canvas = canvas;
            Center = new Point((tl.X+tr.X+br.X+bl.X)/4, (tl.Y + tr.Y + br.Y + bl.Y) / 4);

            InnerBottomLeft = bl;
            InnerBottomRight = br;
            InnerTopLeft = tl;
            InnerTopRight = tr;

            OuterTopLeft = Geometry.PointOnLine(InnerTopRight, InnerTopLeft, Width + 7);
            OuterTopRight = Geometry.PointOnLine(InnerTopLeft, InnerTopRight, Width + 7);
            OuterBottomLeft = Geometry.PointOnLine(InnerBottomRight, InnerBottomLeft, Width + 7);
            OuterBottomRight = Geometry.PointOnLine(InnerBottomLeft, InnerBottomRight, Width + 7);

            itle = new Ellipse();
            itre = new Ellipse();
            ibre = new Ellipse();
            ible = new Ellipse();
            otle = new Ellipse();
            otre = new Ellipse();
            obre = new Ellipse();
            oble = new Ellipse();
        }

        public TransformGeometry(Beam beam, Canvas canvas)
        {
            Height = beam.Height;
            Width = beam.Width;
            _canvas = canvas;
            Center = new Point(Canvas.GetLeft(beam) + Width/2, Canvas.GetTop(beam) + Height / 2);

            InnerBottomLeft = new Point(Center.X - Width / 2, Center.Y - Height / 2);
            InnerBottomRight = new Point(Center.X + Width / 2, Center.Y - Height / 2);
            InnerTopLeft = new Point(Center.X - Width / 2, Center.Y + Height / 2);
            InnerTopRight = new Point(Center.X + Width / 2, Center.Y + Height / 2);

            OuterTopLeft = new Point(InnerTopLeft.X - 7, InnerTopLeft.Y);
            OuterTopRight = new Point(InnerTopRight.X + 7, InnerTopRight.Y);
            OuterBottomRight = new Point(InnerBottomRight.X + 7, InnerBottomRight.Y);
            OuterBottomLeft = new Point(InnerBottomLeft.X - 7, InnerBottomLeft.Y);

            itle = new Ellipse();
            itre = new Ellipse();
            ibre = new Ellipse();
            ible = new Ellipse();
            otle = new Ellipse();
            otre = new Ellipse();
            obre = new Ellipse();
            oble = new Ellipse();
        }

        public double Height { get; set; }

        public double Width { get; set; }

        public double Rotation { get; set; }

        public Point Center;

        public Point InnerTopLeft;

        public Point InnerTopRight;

        public Point InnerBottomLeft;

        public Point InnerBottomRight;

        public Point OuterTopLeft;

        public Point OuterTopRight;

        public Point OuterBottomLeft;

        public Point OuterBottomRight;

        private Ellipse itle;

        private Ellipse itre;

        private Ellipse ibre;

        private Ellipse ible;

        private Ellipse otle;

        private Ellipse otre;

        private Ellipse obre;

        private Ellipse oble;

        private Canvas _canvas;

        private List<Point> poly;

        private bool _ellipsevisible = false;

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

            InnerBottomLeft.X = InnerBottomLeft.X + delta.X;
            InnerBottomLeft.Y = InnerBottomLeft.Y + delta.Y;
            InnerBottomRight.X = InnerBottomRight.X + delta.X;
            InnerBottomRight.Y = InnerBottomRight.Y + delta.Y;

            OuterBottomLeft.X = OuterBottomLeft.X + delta.X;
            OuterBottomLeft.Y = OuterBottomLeft.Y + delta.Y;
            OuterBottomRight.X = OuterBottomRight.X + delta.X;
            OuterBottomRight.Y = OuterBottomRight.Y + delta.Y;

            InnerTopLeft.X = InnerTopLeft.X + delta.X;
            InnerTopLeft.Y = InnerTopLeft.Y + delta.Y;
            InnerTopRight.X = InnerTopRight.X + delta.X;
            InnerTopRight.Y = InnerTopRight.Y + delta.Y;

            OuterTopLeft.X = OuterTopLeft.X + delta.X;
            OuterTopLeft.Y = OuterTopLeft.Y + delta.Y;
            OuterTopRight.X = OuterTopRight.X + delta.X;
            OuterTopRight.Y = OuterTopRight.Y + delta.Y;
        }

        private void MoveFromCenter(Point c)
        {
            InitCorners(new Point((c.X - Center.X), (c.Y - Center.Y)));
            Center.X = Center.X + (c.X - Center.X);
            Center.Y = Center.Y + (c.Y - Center.Y);
        }

        private void InitCorners(Point delta)
        {
            InnerBottomLeft.X = InnerBottomLeft.X + delta.X;
            InnerBottomLeft.Y = InnerBottomLeft.Y + delta.Y;
            InnerBottomRight.X = InnerBottomRight.X + delta.X;
            InnerBottomRight.Y = InnerBottomRight.Y + delta.Y;

            OuterBottomLeft.X = OuterBottomLeft.X + delta.X;
            OuterBottomLeft.Y = OuterBottomLeft.Y + delta.Y;
            OuterBottomRight.X = OuterBottomRight.X + delta.X;
            OuterBottomRight.Y = OuterBottomRight.Y + delta.Y;

            InnerTopLeft.X = InnerTopLeft.X + delta.X;
            InnerTopLeft.Y = InnerTopLeft.Y + delta.Y;
            InnerTopRight.X = InnerTopRight.X + delta.X;
            InnerTopRight.Y = InnerTopRight.Y + delta.Y;

            OuterTopLeft.X = OuterTopLeft.X + delta.X;
            OuterTopLeft.Y = OuterTopLeft.Y + delta.Y;
            OuterTopRight.X = OuterTopRight.X + delta.X;
            OuterTopRight.Y = OuterTopRight.Y + delta.Y;
        }

        public void Rotate(Point rotationcenter, double degree)
        {
            double qtyRadians = degree * System.Math.PI/180;
            //Move center to origin
            Point temp_orig = new Point(rotationcenter.X, rotationcenter.Y);
            Move(new Point(rotationcenter.X, rotationcenter.Y), new Point(0, 0));

            InnerBottomRight = RotatePoint(InnerBottomRight, qtyRadians);
            InnerTopRight = RotatePoint(InnerTopRight, qtyRadians);
            InnerBottomLeft = RotatePoint(InnerBottomLeft, qtyRadians);
            InnerTopLeft = RotatePoint(InnerTopLeft, qtyRadians);

            OuterBottomRight = RotatePoint(OuterBottomRight, qtyRadians);
            OuterTopRight = RotatePoint(OuterTopRight, qtyRadians);
            OuterBottomLeft = RotatePoint(OuterBottomLeft, qtyRadians);
            OuterTopLeft = RotatePoint(OuterTopLeft, qtyRadians);

            //Move center back
            Move(new Point(0, 0), temp_orig);
        }

        public void RotateAboutCenter(double degree)
        {
            double qtyRadians = degree * System.Math.PI / 180;
            //Move center to origin
            Point temp_orig = new Point(Center.X, Center.Y);
            MoveFromCenter(new Point(0, 0));

            InnerBottomRight = RotatePoint(InnerBottomRight, qtyRadians);
            InnerTopRight = RotatePoint(InnerTopRight, qtyRadians);
            InnerBottomLeft = RotatePoint(InnerBottomLeft, qtyRadians);
            InnerTopLeft = RotatePoint(InnerTopLeft, qtyRadians);

            OuterBottomRight = RotatePoint(OuterBottomRight, qtyRadians);
            OuterTopRight = RotatePoint(OuterTopRight, qtyRadians);
            OuterBottomLeft = RotatePoint(OuterBottomLeft, qtyRadians);
            OuterTopLeft = RotatePoint(OuterTopLeft, qtyRadians);

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
            ShowCorners(radius, radius);
        }

        public void ShowCorners(double innerradius, double outerradius)
        {
            if (_canvas.Children.Contains(itle))
            {
                _canvas.Children.Remove(itle);
            }

            if (_canvas.Children.Contains(itre))
            {
                _canvas.Children.Remove(itre);
            }

            if (_canvas.Children.Contains(ibre))
            {
                _canvas.Children.Remove(ibre);
            }

            if (_canvas.Children.Contains(ible))
            {
                _canvas.Children.Remove(ible);
            }

            if (_canvas.Children.Contains(otle))
            {
                _canvas.Children.Remove(otle);
            }

            if (_canvas.Children.Contains(otre))
            {
                _canvas.Children.Remove(otre);
            }

            if (_canvas.Children.Contains(obre))
            {
                _canvas.Children.Remove(obre);
            }

            if (_canvas.Children.Contains(oble))
            {
                _canvas.Children.Remove(oble);
            }

            itle.Width = innerradius;
            itle.Height = innerradius;
            itle.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(itle);
            Canvas.SetLeft(itle, InnerTopLeft.X - innerradius / 2);
            Canvas.SetTop(itle, InnerTopLeft.Y - innerradius / 2);

            itre.Width = innerradius;
            itre.Height = innerradius;
            itre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(itre);
            Canvas.SetLeft(itre, InnerTopRight.X - innerradius / 2);
            Canvas.SetTop(itre, InnerTopRight.Y - innerradius / 2);

            ibre.Width = innerradius;
            ibre.Height = innerradius;
            ibre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ibre);
            Canvas.SetLeft(ibre, InnerBottomRight.X - innerradius / 2);
            Canvas.SetTop(ibre, InnerBottomRight.Y - innerradius / 2);

            ible.Width = innerradius;
            ible.Height = innerradius;
            ible.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));

            _canvas.Children.Add(ible);
            Canvas.SetLeft(ible, InnerBottomLeft.X - innerradius / 2);
            Canvas.SetTop(ible, InnerBottomLeft.Y - innerradius / 2);

            otle.Width = outerradius;
            otle.Height = outerradius;
            otle.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 6));

            _canvas.Children.Add(otle);
            Canvas.SetLeft(otle, OuterTopLeft.X - outerradius / 2);
            Canvas.SetTop(otle, OuterTopLeft.Y - outerradius / 2);

            otre.Width = outerradius;
            otre.Height = outerradius;
            otre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 6));

            _canvas.Children.Add(otre);
            Canvas.SetLeft(otre, OuterTopRight.X - outerradius / 2);
            Canvas.SetTop(otre, OuterTopRight.Y - outerradius / 2);


            obre.Width = outerradius;
            obre.Height = outerradius;
            obre.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 6));

            _canvas.Children.Add(obre);
            Canvas.SetLeft(obre, OuterBottomRight.X - outerradius / 2);
            Canvas.SetTop(obre, OuterBottomRight.Y - outerradius / 2);

            oble.Width = outerradius;
            oble.Height = outerradius;
            oble.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 6));

            _canvas.Children.Add(oble);
            Canvas.SetLeft(oble, OuterBottomLeft.X - outerradius / 2);
            Canvas.SetTop(oble, OuterBottomLeft.Y - outerradius / 2);

            _ellipsevisible = true;
        }

        public void HideCorners()
        {
            if (_ellipsevisible)
            {
                if (_canvas.Children.Contains(itle))
                {
                    _canvas.Children.Remove(itle);
                }

                if (_canvas.Children.Contains(itre))
                {
                    _canvas.Children.Remove(itre);
                }

                if (_canvas.Children.Contains(ibre))
                {
                    _canvas.Children.Remove(ibre);
                }

                if (_canvas.Children.Contains(ible))
                {
                    _canvas.Children.Remove(ible);
                }

                if (_canvas.Children.Contains(otle))
                {
                    _canvas.Children.Remove(otle);
                }

                if (_canvas.Children.Contains(otre))
                {
                    _canvas.Children.Remove(otre);
                }

                if (_canvas.Children.Contains(obre))
                {
                    _canvas.Children.Remove(obre);
                }

                if (_canvas.Children.Contains(oble))
                {
                    _canvas.Children.Remove(oble);
                }
                _ellipsevisible = false;
            }
        }

        public bool IsInsideInner(Point point)
        {
            var list = new List<PointF>();
            list.Add(new PointF((float)InnerTopLeft.X, (float)InnerTopLeft.Y));
            list.Add(new PointF((float)InnerTopRight.X, (float)InnerTopRight.Y));
            list.Add(new PointF((float)InnerBottomRight.X, (float)InnerBottomRight.Y));
            list.Add(new PointF((float)InnerBottomLeft.X, (float)InnerBottomLeft.Y));

            var polygon = new TPolygon(list.ToArray());

            var check = polygon.PointInPolygon((float) point.X, (float) point.Y);

            if (check)
            {
                MyDebug.WriteInformation("the point is inside of the rectangle");
            }
            else
            {
                MyDebug.WriteInformation("the point is outside of the rectangle");
            }

            return check;
        }

        public bool IsInsideOuter(Point point)
        {
            var list = new List<PointF>();
            list.Add(new PointF((float)OuterTopLeft.X, (float)OuterTopLeft.Y));
            list.Add(new PointF((float)OuterTopRight.X, (float)OuterTopRight.Y));
            list.Add(new PointF((float)OuterBottomRight.X, (float)OuterBottomRight.Y));
            list.Add(new PointF((float)OuterBottomLeft.X, (float)OuterBottomLeft.Y));

            var polygon = new TPolygon(list.ToArray());

            var check = polygon.PointInPolygon((float) point.X, (float) point.Y);

            if (check)
            {
                MyDebug.WriteInformation("the point is inside of the rectangle");
            }
            else
            {
                MyDebug.WriteInformation("the point is outside of the rectangle");
            }

            return check;
        }

        public void ChangeWidth(double width)
        {
            InnerTopRight = Geometry.PointOnLine(InnerTopLeft, InnerTopRight, Width);
            InnerTopRight = Geometry.PointOnLine(InnerBottomLeft, InnerBottomRight, Width);

            OuterTopLeft = Geometry.PointOnLine(InnerTopRight, InnerTopLeft, Width + 7);
            OuterTopRight = Geometry.PointOnLine(InnerTopLeft, InnerTopRight, Width + 7);
            OuterBottomLeft = Geometry.PointOnLine(InnerBottomRight, InnerBottomLeft, Width + 7);
            OuterBottomRight = Geometry.PointOnLine(InnerBottomLeft, InnerBottomRight, Width + 7);
        }

        public Point LeftPoint
        {
            get
            {
                var x = (InnerTopLeft.X + InnerBottomLeft.X)/2;
                var y = (InnerTopLeft.Y + InnerBottomLeft.Y) / 2;
                return new Point(x, y);
            }
        }

        public Point RightPoint
        {
            get
            {
                var x = (InnerTopRight.X + InnerBottomRight.X) / 2;
                var y = (InnerTopRight.Y + InnerBottomRight.Y) / 2;
                return new Point(x, y);
            }
        }
    }
}
