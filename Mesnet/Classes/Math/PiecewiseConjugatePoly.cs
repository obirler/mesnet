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
using System.Collections;
using System.Collections.Generic;

namespace Mesnet.Classes.Math
{
    public class PiecewiseConjugatePoly : CollectionBase
    {
        public PiecewiseConjugatePoly()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiecewiseConjugatePoly"/> class. With this constructor, polynomials will be sorted automatically.
        /// </summary>
        /// <param name="polylist">The polylist.</param>
        public PiecewiseConjugatePoly(List<ConjugatePoly> polylist)
        {
            _sortlist = polylist;

            _sortlist.Sort((a, b) => a.StartPoint.CompareTo(b.StartPoint));

            foreach (ConjugatePoly poly in _sortlist)
            {
                List.Add(poly);
            }
        }

        private List<ConjugatePoly> _sortlist = new List<ConjugatePoly>();

        /// <summary>
        /// Adds a polynomial to the piecewisepoly.
        /// </summary>
        /// <param name="poly">The poly.</param>
        public void Add(ConjugatePoly poly)
        {
            List.Add(poly);

            _sortlist.Clear();

            foreach (ConjugatePoly pol in List)
            {
                _sortlist.Add(pol);
            }

            List.Clear();

            foreach (ConjugatePoly pol in _sortlist)
            {
                List.Add(pol);
            }
        }

        public ConjugatePoly this[int index]
        {
            get { return ((ConjugatePoly)List[index]); }
            set { List[index] = value; }
        }

        public int Length
        {
            get
            {
                return List.Count;
            }
        }

        public int IndexOf(ConjugatePoly value)
        {
            return (List.IndexOf(value));

        }

        public ConjugatePoly Last()
        {
            return (ConjugatePoly)List[Count - 1];
        }

        public void Insert(int index, ConjugatePoly value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Removes a specified polynomial value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(ConjugatePoly value)
        {
            List.Remove(value);
        }

        public bool Contains(ConjugatePoly value)
        {
            return (List.Contains(value));
        }

        /// <summary>
        /// Sorts the polynomials according to their start point.
        /// </summary>
        public void Sort()
        {
            _sortlist.Clear();

            foreach (ConjugatePoly pol in List)
            {
                _sortlist.Add(pol);
            }

            _sortlist.Sort((a, b) => a.StartPoint.CompareTo(b.StartPoint));

            List.Clear();

            foreach (ConjugatePoly pol in _sortlist)
            {
                List.Add(pol);
            }
        }

        public PiecewiseConjugatePoly Integrate()
        {
            var ppoly = new PiecewiseConjugatePoly();
            foreach (ConjugatePoly poly in List)
            {
                var ply = poly.Integrate();
                ply.StartPoint = poly.StartPoint;
                ply.EndPoint = poly.EndPoint;
                ppoly.Add(ply);
            }

            return ppoly;
        }

        public double DefiniteIntegral(double start, double end)
        {
            double value = 0;
            foreach (ConjugatePoly poly in List)
            {
                if (poly.StartPoint >= start || poly.EndPoint <= end)
                {
                    double left = 0;
                    double right = 0;
                    if (poly.StartPoint >= start)
                    {
                        left = poly.StartPoint;
                    }
                    else if (poly.StartPoint < start)
                    {
                        left = start;
                    }

                    if (poly.EndPoint <= end)
                    {
                        right = poly.EndPoint;
                    }
                    else if (poly.EndPoint > end)
                    {
                        right = end;
                    }

                    if (left >= end)
                    {
                        break;
                    }
                    value += poly.DefiniteIntegral(left, right);
                }
            }

            return value;
        }

        public double Calculate(double x)
        {
            double value = 0;
            foreach (ConjugatePoly poly in List)
            {
                if (x >= poly.StartPoint && x <= poly.EndPoint)
                {
                    value = poly.Calculate(x);
                    return value;
                }
            }
            return value;
        }

        public double Max
        {
            get
            {
                double result = 0;
                double polymax = 0;

                foreach (ConjugatePoly poly in List)
                {
                    polymax = poly.Maximum(poly.StartPoint, poly.EndPoint);
                    if (polymax > result)
                    {
                        result = polymax;
                    }
                }
                return result;
            }
        }
    }
}
