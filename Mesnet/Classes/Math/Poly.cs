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
using System.Linq;
using Mesnet.Classes.Tools;

namespace Mesnet.Classes.Math
{
    /// <summary>
    /// 
    /// </summary>
    public class Poly
    {
        #region Constructor Overloading:
        /// <summary>
        /// Constructor which Read String and find Terms in it. Create new Term for each
        /// Term String and add it to the Terms Collection. 
        /// </summary>
        /// <param name="PolyExpression"></param>
        public Poly(string PolyExpression)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Poly"/> class from a string expression and between start point and endpoint.
        /// </summary>
        /// <param name="PolyExpression">The poly expression string.</param>
        /// <param name="startpoint">The startpoint of the polynomial.</param>
        /// <param name="endpoint">The endpoint point of the poynomial.</param>
        public Poly(string PolyExpression, double startpoint, double endpoint)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Constructor which create a new instance of Poly with a predefined TermCollection.
        /// </summary>
        /// <param name="terms"></param>
        public Poly(TermCollection terms)
        {
            this.Terms = terms;
            this.Terms.Sort(TermCollection.SortType.ASC);
        }

        public Poly()
        {           
        }

        #endregion

        #region Destructor:
        /// <summary>
        /// Clear the Term Collections
        /// </summary>
        ~Poly()
        {
            if (Terms != null)
            {
                Terms.Clear();
            }
        }

        #endregion 

        #region Override methods:

        /// <summary>
        /// This will Print out the string Format of Polynomial. by Calling each Term in the collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Terms != null)
            {
                this.Terms.Sort(TermCollection.SortType.DES);

                if (Terms.Count > 0)
                {
                    foreach (Term t in this.Terms)
                    {
                        result += t.ToString();
                    }
                    if (result.Substring(0, 1) == "+")
                        result = result.Remove(0, 1);
                }
                else
                {
                    result = "0";
                }
            }
            else
            {
                result = "0";
            }

            return result;
        }

        public string GetString(int digit)
        {
            this.Terms.Sort(TermCollection.SortType.DES);

            string result = string.Empty;

            if (Terms.Count > 0)
            {
                foreach (Term t in this.Terms)
                {
                    result += t.GetString(digit);
                }
                if (result.Substring(0, 1) == "+")
                    result = result.Remove(0, 1);
            }
            else
            {
                result = "0";
            }

            return result;
        }

        #endregion

        #region Methods:

        public void Parse(string PolyExpression)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
        }

        /// <summary>
        /// Calculate the Value of Polynomial with the given X value.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Calculate(double x)
        {
            double result = 0;
            foreach (Term t in this.Terms)
            {
                result += t.Coefficient *System.Math.Pow(x, t.Power);
            }
            return result;
        }

        /// <summary>
        /// Finds the maximum value of the polynomial using numerical method.
        /// </summary>
        /// <param name="startpoint">The start point of the calculation.</param>
        /// <param name="endpoint">The end point of the calculation.</param>
        /// <param name="digit">The desired digit that will be rounded.</param>
        /// <returns>The maximum value of the polynomial</returns>
        public double Maximum(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var list = new Dictionary<double, double>();

            var diff = 1/1000.0;

            double left = 0;
            double right = 0;

            for (double i = startpoint; i < endpoint; i=i+diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(endpoint, Calculate(endpoint));

            var max = list.Max(x => x.Value);
            double maxindex = Convert.ToDouble(list.First(x => x.Value == max).Key);

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < startpoint || right > endpoint)
            {
                return System.Math.Round(max, digit);
            }

            diff = (right - left)/100.0;

            list.Clear();

            for (double i = left; i < right; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(right, Calculate(right));

            max= list.Max(x => x.Value);

            return System.Math.Round(max, digit);
        }

        public double Maximum()
        {
            return Maximum(_startpoint, _endpoint);
        }

        public double MaxLocation(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var list = new Dictionary<double, double>();

            var diff = 1 / 1000.0;

            double left = 0;
            double right = 0;

            for (double i = startpoint; i < endpoint; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(endpoint, Calculate(endpoint));

            var max = list.Max(x => x.Value);
            double maxindex = Convert.ToDouble(list.First(x => x.Value == max).Key);

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < startpoint)
            {
                return System.Math.Round(startpoint, digit); 
            }
            if(right > endpoint)
            {
                return System.Math.Round(endpoint, digit);
            }

            diff = (right - left) / 100.0;

            list.Clear();

            for (double i = left; i < right; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(right, Calculate(right));

            max = list.Max(x => x.Value);

            double loc = 0;

            foreach (var item in list)
            {
                if (item.Value == max)
                {
                    loc = item.Key;
                }
            }

            return System.Math.Round(loc, digit);
        }

        public double Minimum(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var list = new Dictionary<double, double>();

            var diff = 1 / 1000.0;

            double left = 0;
            double right = 0;

            for (double i = startpoint; i < endpoint; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(endpoint, Calculate(endpoint));

            var min = list.Min(x => x.Value);
            double minindex = Convert.ToDouble(list.First(x => x.Value == min).Key);

            left = minindex - diff;

            right = minindex + diff;

            if (left < startpoint || right > endpoint)
            {
                return System.Math.Round(min, digit);
            }

            diff = (right - left) / 100.0;

            list.Clear();

            for (double i = left; i < right; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(right, Calculate(right));

            min = list.Min(x => x.Value);

            return System.Math.Round(min, digit);
        }

        public double Minimum()
        {
            return Minimum(_startpoint, _endpoint);
        }

        public double MinLocation(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var list = new Dictionary<double, double>();

            var diff = 1 / 100.0;

            double left = 0;
            double right = 0;

            for (double i = startpoint; i < endpoint; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(endpoint, Calculate(endpoint));

            var min = list.Min(x => x.Value);
            double minindex = Convert.ToDouble(list.First(x => x.Value == min).Key);

            left = minindex - diff;

            right = minindex + diff;

            if (left < startpoint)
            {
                return System.Math.Round(startpoint, digit);
            }
            if (right > endpoint)
            {
                return System.Math.Round(endpoint, digit);
            }

            diff = (right - left) / 100.0;

            list.Clear();

            for (double i = left; i < right; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            list.Add(right, Calculate(right));

            min = list.Min(x => x.Value);

            double loc = 0;

            foreach (var item in list)
            {
                if (item.Value == min)
                {
                    loc = item.Key;
                }
            }

            return System.Math.Round(loc, digit);
        }

        /// <summary>
        /// Static method which Validate the input Expression
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool ValidateExpression(string Expression)
        {
            if (Expression.Length == 0)
                return false;

            Expression = Expression.Trim();
            Expression = Expression.Replace(" ", "");
            while (Expression.IndexOf("--") > -1 | Expression.IndexOf("++") > -1 | Expression.IndexOf("^^") > -1 | Expression.IndexOf("xx") > -1)
            {
                Expression = Expression.Replace("--", "-");
                Expression = Expression.Replace("++", "+");
                Expression = Expression.Replace("^^", "^");
                Expression = Expression.Replace("xx", "x");
            }
            string ValidChars = "+-x1234567890^.";
            bool result = true;
            foreach (char c in Expression)
            {
                if (ValidChars.IndexOf(c) == -1)
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// Read Method will Identify any Term in the Expression and Create a new Instance of 
        /// Term Class and add it to the TermCollection
        /// </summary>
        /// <param name="PolyExpression">input string of Polynomial Expression</param>
        private void ReadPolyExpression(string PolyExpression)
        {
            if (ValidateExpression(PolyExpression))
            {
                string NextChar = string.Empty;
                string NextTerm = string.Empty;
                for (int i = 0; i < PolyExpression.Length; i++)
                {
                    NextChar = PolyExpression.Substring(i, 1);
                    if ((NextChar == "-" | NextChar == "+") & i > 0)
                    {
                        Term TermItem = new Term(NextTerm);
                        this.Terms.Add(TermItem);
                        NextTerm = string.Empty;
                    }
                    NextTerm += NextChar;
                }
                Term Item = new Term(NextTerm);
                this.Terms.Add(Item);

                this.Terms.Sort(TermCollection.SortType.ASC);
            }
            else
            {
                MyDebug.WriteError("Invalid Polynomial Expression : " + PolyExpression);
                throw new Exception("Invalid Polynomial Expression");
            }
        }

        public Poly Integrate()
        {
            var terms = new TermCollection();
            foreach (Term t in this.Terms)
            {
                var pow = t.Power + 1;
                var coeff = t.Coefficient / (t.Power + 1);
                terms.Add(new Term(pow, coeff));
            }

            return new Poly(terms);
        }

        /// <summary>
        /// Calculates the definite integral with given range.
        /// </summary>
        /// <param name="start">Start value of integral.</param>
        /// <param name="end">End of the integral.</param>
        /// <returns></returns>
        public double DefiniteIntegral(double start, double end)
        {
            double result = 0;

            Poly integral = Integrate();

            result = integral.Calculate(end) - integral.Calculate(start);

            return result;
        }

        public Poly Derivate()
        {
            var terms = new TermCollection();
            foreach (Term t in this.Terms)
            {
                var pow = t.Power - 1;
                var coeff = t.Coefficient * t.Power;
                terms.Add(new Term(pow, coeff));
            }

            return new Poly(terms);
        }

        /// <summary>
        /// Finds the center of the polynom on x-axis in the range given.
        /// </summary>
        /// <param name="start">The start point in x-axis of the polynom.</param>
        /// <param name="end">The end point in x-axis of the polynom.</param>
        /// <returns>The x-axis of center of area of the polynom.</returns>
        public double LoadCenter(double start, double end)
        {
            double centerpoint = 0;

            Poly p1 = new Poly("x");

            Poly p2 = p1 * this;

            centerpoint = p2.DefiniteIntegral(start, end)/this.DefiniteIntegral(start, end);

            return centerpoint;
        }

        /// <summary>
        /// Conjugates polynomial with the specified l. Conjageting is basically converting x in polynomial into (l-x).
        /// </summary>
        /// <param name="x">The desired conjugation length</param>
        /// <returns>Conjugated polynomial</returns>
        public ConjugatePoly Conjugate(double length)
        {
            var terms = new ConjugateTermCollection();
            foreach (Term t in this.Terms)
            {
                if (t.Power > 0)
                {
                    string newcoeff;
                    if (t.Coefficient == 1)
                    {
                        newcoeff = "(" + length + "-x)";
                    }
                    else if (t.Coefficient == -1)
                    {
                        newcoeff= "-(" + length + "-x)";
                    }
                    else
                    {
                       newcoeff = t.Coefficient + "(" + length + "-x)";
                    }
                    var newterm = new ConjugateTerm(t.Power, newcoeff);
                    terms.Add(newterm);
                }
                else if (t.Power == 0)
                {
                    var newterm = new ConjugateTerm(t.Power, t.Coefficient.ToString());
                    terms.Add(newterm);
                }               
            }
            var conjugatepoly = new ConjugatePoly(terms);
            conjugatepoly.StartPoint = length - EndPoint;
            conjugatepoly.EndPoint = length - StartPoint;
            return conjugatepoly;
        }               

        #endregion

        #region Fields & Properties:

        /// <summary>
        /// Terms Property, Type of TermCollection
        /// </summary>
        private TermCollection _Terms;

        public TermCollection Terms
        {
            get
            {
                return _Terms;
            }
            set
            {
                _Terms = value;
            }
        }

        private double _startpoint;

        private double _endpoint;

        /// <summary>
        /// Read-Only Property return the Length of TermCollection which means length of Polynomial Expression.
        /// </summary>
        public int Lentgh
        {
            get
            {
                return this.Terms.Length;
            }
        }

        public double StartPoint
        {
            get { return _startpoint; }
            set { _startpoint = value; }
        }

        public double EndPoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        #endregion

        #region Operator OverLoading:

        /// <summary>
        /// Plus Operator: 
        /// Add Method of TermsCollection will Check the Power of each Term And if it's already 
        /// exists in the Collection Just Plus the Coefficient of the Term and This Mean Plus Operation.
        /// So We Simply Add the Terms of Second Poly to the First one.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator +(Poly p1, Poly p2)
        {
            if (p1.ToString() == "0")
            {
                return p2;
            }
            else if (p2.ToString() == "0")
            {
                return p1;
            }

            Poly result = new Poly(p1.ToString());
            foreach (Term t in p2.Terms)
                result.Terms.Add(t);
            return result;
        }

        /// <summary>
        /// Minus Operations: Like Plus Operation but at first we just Make the Second Poly to the Negetive Value.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator -(Poly p1, Poly p2)
        {
            if (p1.ToString() == "0")
            {
                return -1*p2;
            }
            else if (p2.ToString() == "0")
            {
                return p1;
            }

            Poly result = new Poly(p1.ToString());
            Poly NegetiveP2 = new Poly(p2.ToString());
            foreach (Term t in NegetiveP2.Terms)
                t.Coefficient *= -1;

            return result + NegetiveP2;
        }

        /// <summary>
        /// Multiple Operation: For each term in the First Poly We Multiple it in the Each Term of Second Poly
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator *(Poly p1, Poly p2)
        {
            TermCollection result = new TermCollection();
            int counter = 0;
            foreach (Term t1 in p1.Terms)
            {
                foreach (Term t2 in p2.Terms)
                {
                    result.Add(new Term(t1.Power + t2.Power, t1.Coefficient * t2.Coefficient));
                    counter++;
                }
            }
            return new Poly(result);
        }

        /// <summary>
        /// Divide operation.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator /(Poly p1, Poly p2)
        {
            p1.Terms.Sort(TermCollection.SortType.DES);
            p2.Terms.Sort(TermCollection.SortType.DES);
            TermCollection resultTerms = new TermCollection();
            if (p1.Terms[0].Power < p2.Terms[0].Power)
                throw new Exception("Invalid Division: P1.MaxPower is Lower than P2.MaxPower");
            while (p1.Terms[0].Power > p2.Terms[0].Power)
            {
                Term NextResult = new Term(p1.Terms[0].Power - p2.Terms[0].Power, p1.Terms[0].Coefficient / p2.Terms[0].Coefficient);
                resultTerms.Add(NextResult);
                Poly TempPoly = NextResult;

                Poly NewPoly = TempPoly * p2;
                p1 = p1 - NewPoly;
            }
            return new Poly(resultTerms);
        }

        /// <summary>
        /// this will Create a new Poly by the Value of 1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Poly operator ++(Poly p1)
        {
            Poly p2 = new Poly("1");
            p1 = p1 + p2;
            return p1;
        }

        /// <summary>
        /// this will Create a new Poly by the Value of -1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Poly operator --(Poly p1)
        {
            Poly p2 = new Poly("-1");
            p1 = p1 + p2;
            return p1;
        }

        /// <summary>
        /// Implicit Conversion : this will Convert the single Term to the Poly. 
        /// First it Creates a new Instance of TermCollection and Add The Term to it. 
        /// Second Creates a new Poly by the TermCollection and Return it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static implicit operator Poly(Term t)
        {
            TermCollection Terms = new TermCollection();
            Terms.Add(t);
            return new Poly(Terms);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static implicit operator Poly(string expression)
        {
            return new Poly(expression);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Poly(int value)
        {
            return new Poly(value.ToString());
        }
        #endregion
    }
}
