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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mesnet.Classes.Math
{
    public class PiecewisePoly:CollectionBase
    {
        public PiecewisePoly()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiecewisePoly"/> class. With this constructor, polynomials will be sorted automatically.
        /// </summary>
        /// <param name="polylist">The polylist.</param>
        public PiecewisePoly(List<Poly> polylist)
        {
            _sortlist = polylist;

            _sortlist.Sort((a, b) => a.StartPoint.CompareTo(b.StartPoint));

            foreach (Poly poly in _sortlist)
            {
                List.Add(poly);
            }            
        }

        private List<Poly> _sortlist = new List<Poly>();

        /// <summary>
        /// Adds a polynomial to the piecewisepoly.
        /// </summary>
        /// <param name="poly">The poly.</param>
        public void Add(Poly poly)
        {
            List.Add(poly);
            Sort();
        }

        public Poly this[int index]
        {
            get { return ((Poly)List[index]); }
            set { List[index] = value; }
        }

        public int Length
        {
            get
            {
                return List.Count;
            }
        }

        public int IndexOf(Poly value)
        {
            return (List.IndexOf(value));
        }

        public Poly Last()
        {
            return (Poly) List[Count - 1];
        }

        public void Insert(int index, Poly value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Removes a specified polynomial value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(Poly value)
        {
            List.Remove(value);
            _sortlist.Remove(value);
        }

        public bool Contains(Poly value)
        {
            return (List.Contains(value));
        }

        /// <summary>
        /// Sorts the polynomials according to their start point.
        /// </summary>
        public void Sort()
        {
            _sortlist.Clear();

            foreach (Poly pol in List)
            {
                _sortlist.Add(pol);
            }

            _sortlist.Sort((a, b) => a.StartPoint.CompareTo(b.StartPoint));

            List.Clear();

            foreach (Poly pol in _sortlist)
            {
                List.Add(pol);
            }
        }

        public PiecewisePoly Integrate()
        {
            var ppoly = new PiecewisePoly();
            foreach (Poly poly in List)
            {
                var ply = poly.Integrate();
                ply.StartPoint = poly.StartPoint;
                ply.EndPoint = poly.EndPoint;
                ppoly.Add(ply);
            }
            return ppoly;
        }

        public PiecewisePoly Derivate()
        {
            var ppoly = new PiecewisePoly();
            foreach (Poly poly in List)
            {
                var ply = poly.Derivate();
                ply.StartPoint = poly.StartPoint;
                ply.EndPoint = poly.EndPoint;
                ppoly.Add(ply);
            }
            return ppoly;
        }

        public double DefiniteIntegral(double start, double end)
        {
            double value = 0;
            foreach (Poly poly in List)
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
            foreach (Poly poly in List)
            {
                if (x >= poly.StartPoint && x <= poly.EndPoint)
                {
                    value = poly.Calculate(x);
                    return value;
                }
            }
            return value;
        }

        public PiecewiseConjugatePoly Conjugate(double length)
        {
            List<ConjugatePoly> conjugatelist = new List<ConjugatePoly>();

            foreach (Poly poly in List)
            {
                var conjugatepoly = poly.Conjugate(length);
                conjugatelist.Add(conjugatepoly);
            }
            return new PiecewiseConjugatePoly(conjugatelist);
        }

        public PiecewisePoly Propagate(double length)
        {
            List<Poly> propagatelist = new List<Poly>();
            foreach (Poly poly in List)
            {
                var propagatepoly = poly.Propagate(length);
                propagatelist.Add(propagatepoly);
            }
            return new PiecewisePoly(propagatelist);
        }

        public List<Poly> PolyList()
        {
            var polylist = new List<Poly>();
            foreach (Poly poly in List)
            {
                polylist.Add(poly);
            }
            return polylist;
        }

        public double Degree()
        {
            double result = Double.MinValue;

            foreach (Poly poly in List)
            {
                var degree = poly.Degree();
                if (degree > result)
                {
                    result = degree;
                }
            }
            return result;
        }

        public double Max
        {
            get
            {
                double result = Double.MinValue;

                if (List.Count > 0)
                {                   
                    double polymax = 0;

                    foreach (Poly poly in List)
                    {
                        polymax = poly.Maximum(poly.StartPoint, poly.EndPoint);
                        if (polymax > result)
                        {
                            result = polymax;
                        }
                    }
                }
                else
                {
                    result = 0;
                }
                
                return result;
            }
        }

        public double MaxAbs
        {
            get
            {
                double result = Double.MinValue;

                if (List.Count > 0)
                {
                    double polymax = 0;

                    foreach (Poly poly in List)
                    {
                        polymax = poly.MaximumAbs(poly.StartPoint, poly.EndPoint);
                        if (polymax > result)
                        {
                            result = polymax;
                        }
                    }
                }
                else
                {
                    result = 0;
                }

                return result;
            }
        }

        public double MaxLocation
        {
            get
            {
                double result = Double.MinValue;
                double polymax = 0;
                double location = 0;

                foreach (Poly poly in List)
                {
                    polymax = poly.Maximum(poly.StartPoint, poly.EndPoint);
                    if (polymax > result)
                    {
                        result = polymax;
                        location = poly.MaxLocation(poly.StartPoint, poly.EndPoint);
                    }
                }
                return location;
            }
        }

        public double Min
        {
            get
            {
                double result = Double.MaxValue;
                double polymax = 0;

                foreach (Poly poly in List)
                {
                    polymax = poly.Minimum(poly.StartPoint, poly.EndPoint);
                    if (polymax < result)
                    {
                        result = polymax;
                    }
                }
                return result;
            }
        }

        public double MinLocation
        {
            get
            {
                double result = Double.MaxValue;
                double polymin = 0;
                double location = 0;

                foreach (Poly poly in List)
                {
                    polymin = poly.Minimum(poly.StartPoint, poly.EndPoint);
                    if (polymin < result)
                    {
                        result = polymin;
                        location = poly.MinLocation(poly.StartPoint, poly.EndPoint);
                    }
                }
                return location;
            }
        }

        public static PiecewisePoly operator *(PiecewisePoly p1, PiecewisePoly p2)
        {
            var plylist = new List<Poly>();
            var intervallist = new List<double>();

            foreach (Poly poly1 in p1)
            {
                if (!intervallist.Contains(poly1.StartPoint))
                {
                   intervallist.Add(poly1.StartPoint); 
                }
                if (!intervallist.Contains(poly1.EndPoint))
                {
                    intervallist.Add(poly1.EndPoint);
                }              
            }

            foreach (Poly poly2 in p2)
            {
                if (!intervallist.Contains(poly2.StartPoint))
                {
                    intervallist.Add(poly2.StartPoint);
                }
                if (!intervallist.Contains(poly2.EndPoint))
                {
                    intervallist.Add(poly2.EndPoint);
                }
            }

            intervallist.Sort();

            Poly tempp1= new Poly();
            Poly tempp2 = new Poly();
            Poly tempp = new Poly();

            for (int i = 0; i < intervallist.Count; i++)
            {
                if (i+1 < intervallist.Count)
                {
                    foreach (Poly item1 in p1)
                    {
                        if (item1.StartPoint == intervallist[i])
                        {
                            tempp1 = item1;
                        }
                    }

                    foreach (Poly item2 in p2)
                    {
                        if (item2.StartPoint == intervallist[i])
                        {
                            tempp2 = item2;
                        }
                    }
                    tempp = tempp1 * tempp2;
                    tempp.StartPoint = intervallist[i];
                    tempp.EndPoint = intervallist[i + 1];
                    plylist.Add(tempp);
                }                
            }

            return new PiecewisePoly(plylist);
        }

        public static PiecewisePoly operator +(PiecewisePoly p1, PiecewisePoly p2)
        {
            var plylist = new List<Poly>();
            var intervallist = new List<double>();

            if (p1.Count == 0 || p1 == null)
            {
                return p2;
            }
            if (p2.Count == 0 || p2 == null)
            {
                return p1;
            }

            foreach (Poly poly1 in p1)
            {
                if (!intervallist.Contains(poly1.StartPoint))
                {
                    intervallist.Add(poly1.StartPoint);
                }
                if (!intervallist.Contains(poly1.EndPoint))
                {
                    intervallist.Add(poly1.EndPoint);
                }
            }

            foreach (Poly poly2 in p2)
            {
                if (!intervallist.Contains(poly2.StartPoint))
                {
                    intervallist.Add(poly2.StartPoint);
                }
                if (!intervallist.Contains(poly2.EndPoint))
                {
                    intervallist.Add(poly2.EndPoint);
                }
            }

            intervallist.Sort();

            Poly tempp1 = new Poly();
            Poly tempp2 = new Poly();
            Poly tempp = new Poly();

            for (int i = 0; i < intervallist.Count; i++)
            {
                if (i + 1 < intervallist.Count)
                {
                    foreach (Poly item1 in p1)
                    {
                        if (item1.StartPoint == intervallist[i])
                        {
                            tempp1 = item1;
                        }
                    }

                    foreach (Poly item2 in p2)
                    {
                        if (item2.StartPoint == intervallist[i])
                        {
                            tempp2 = item2;
                        }
                    }
                    tempp = tempp1 + tempp2;
                    tempp.StartPoint = intervallist[i];
                    tempp.EndPoint = intervallist[i + 1];
                    plylist.Add(tempp);
                }
            }

            return new PiecewisePoly(plylist);
        }
    }
}
