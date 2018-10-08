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
