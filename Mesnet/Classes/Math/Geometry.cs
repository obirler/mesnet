using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mesnet.Classes.Math
{
    static class Geometry
    {
        /// <summary>
        /// Finds the third point on the line that goes on p1 and p2.
        /// </summary>
        /// <param name="p1">The first point on the line.</param>
        /// <param name="p2">The second point on the line.</param>
        /// <param name="length">The distance of the third point from p1.</param>
        /// <returns>The third point on the line whose distance from the first point is given</returns>
        public static Point PointOnLine(Point p1, Point p2, double length)
        {
            double oldlength = System.Math.Sqrt(System.Math.Pow(p2.X - p1.X, 2) + System.Math.Pow(p2.Y - p1.Y, 2));
            Point p3 = new Point();
            p3.X = length / oldlength * (p2.X - p1.X) + p1.X;
            p3.Y = length / oldlength * (p2.Y - p1.Y) + p1.Y;
            return p3;
        }
    }
}
