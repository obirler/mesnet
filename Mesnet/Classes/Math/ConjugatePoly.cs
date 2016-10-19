using System;
using System.Collections.Generic;
using System.Linq;

namespace Mesnet.Classes.Math
{
    public class ConjugatePoly
    {
        #region Constructor Overloading:
        /// <summary>
        /// Constructor which Read String and find Terms in it. Create new Term for each
        /// Term String and add it to the Terms Collection. 
        /// </summary>
        /// <param name="PolyExpression"></param>
        public ConjugatePoly(string PolyExpression)
        {
            this._Terms = new ConjugateTermCollection();
            this.ReadPolyExpression(PolyExpression);
        }

        public ConjugatePoly(string PolyExpression, double startpoint, double endpoint)
        {
            this._Terms = new ConjugateTermCollection();
            this.ReadPolyExpression(PolyExpression);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Constructor which create a new instance of Poly with a predefined TermCollection.
        /// </summary>
        /// <param name="terms"></param>
        public ConjugatePoly(ConjugateTermCollection terms)
        {
            this.Terms = terms;
            this.Terms.Sort(ConjugateTermCollection.SortType.ASC);
        }

        public ConjugatePoly(ConjugateTermCollection terms, double startpoint, double endpoint)
        {
            this.Terms = terms;
            this.Terms.Sort(ConjugateTermCollection.SortType.ASC);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        public ConjugatePoly()
        {
            
        }

        #endregion

        #region Destructor:
        /// <summary>
        /// Clear the Term Collections
        /// </summary>
        ~ConjugatePoly()
        {
            this.Terms.Clear();
        }

        #endregion 

        #region Override methods:

        /// <summary>
        /// This will Print out the string Format of Polynomial. by Calling each Term in the collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            this.Terms.Sort(ConjugateTermCollection.SortType.DES);

            string result = string.Empty;
            foreach (ConjugateTerm t in this.Terms)
            {
                result += t.ToString();
            }
            if (result.Substring(0, 1) == "+")
                result = result.Remove(0, 1);
            return result;
        }

        #endregion

        #region Methods:
        /// <summary>
        /// Calculate the Value of Polynomial with the given X value.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Calculate(double x)
        {
            double result = 0;
            foreach (ConjugateTerm t in this.Terms)
            {
                if (t.Coefficient.Contains("("))
                {
                    int Indexofstart = t.Coefficient.IndexOf("(");
                    int Indexofend = t.Coefficient.IndexOf(")");
                    double secondcoeff;                   

                    if (Indexofstart != 0)
                    {
                        string first = t.Coefficient.Substring(0, t.Coefficient.IndexOf("("));
                        if (first=="+")
                        {
                            secondcoeff = 1;
                        }
                        if (first=="-")
                        {
                            secondcoeff = -1;
                        }
                        else
                        {
                            secondcoeff = double.Parse(t.Coefficient.Substring(0, t.Coefficient.IndexOf("(")));
                        }                                             
                    }
                    else
                    {
                        secondcoeff = 1;
                    }
                    string innercoeff = t.Coefficient.Substring(Indexofstart + 1, Indexofend - Indexofstart - 1);
                    var innerpoly= new Poly(innercoeff);

                    result += secondcoeff*System.Math.Pow(innerpoly.Calculate(x), t.Power);
                }
                else
                {
                    result += double.Parse(t.Coefficient) * System.Math.Pow(x, t.Power);
                }
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

            var diff = (endpoint - startpoint) / 100.0;

            double left = 0;
            double right = 0;

            for (double i = startpoint; i <= endpoint; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            var max = list.Max(x => x.Value);
            double maxindex = Convert.ToDouble(list.First(x => x.Value == max).Key);

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < startpoint || right > endpoint)
            {
                return System.Math.Round(max, digit);
            }

            diff = (right - left) / 100.0;

            list.Clear();

            for (double i = left; i <= right; i = i + diff)
            {
                list.Add(i, Calculate(i));
            }

            max = list.Max(x => x.Value);

            return System.Math.Round(max, digit);
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
            string ValidChars = "+-x1234567890()^.";
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
                bool start = false;
                bool end = false;
                for (int i = 0; i < PolyExpression.Length; i++)
                {
                    NextChar = PolyExpression.Substring(i, 1);
                    if (NextChar=="(")
                    {
                        start = true;
                    }
                    else if (NextChar == ")")
                    {
                        end = true;
                    } 
                    if ((NextChar == "-" | NextChar == "+") && i > 0 && !start && !end)
                    {
                        var TermItem = new ConjugateTerm(NextTerm);
                        this.Terms.Add(TermItem);
                        NextTerm = string.Empty;
                    }
                    if (start && end && (NextChar == "-" | NextChar == "+"))
                    {
                        var TermItem = new ConjugateTerm(NextTerm);
                        this.Terms.Add(TermItem);
                        NextTerm = string.Empty;
                        start = false;
                        end = false;
                    }
                    NextTerm += NextChar;
                }
                var Item = new ConjugateTerm(NextTerm);
                this.Terms.Add(Item);

                this.Terms.Sort(ConjugateTermCollection.SortType.ASC);
            }
            else
            {
                throw new Exception("Invalid Polynomial Expression");
            }
        }

        public ConjugatePoly Integrate()
        {
            var terms = new ConjugateTermCollection();
            foreach (ConjugateTerm t in this.Terms)
            {
                if (t.Coefficient.Contains("("))
                {
                    var inner = GetInnerTerm(t);
                    Poly innerpoly = new Poly(inner);
                    double power = t.Power;
                    int Indexofstart = t.Coefficient.IndexOf("(");
                    var xindex = inner.IndexOf("x");
                    double xcoeff = 0;
                    double secondcoeff = 0;
                    if (xindex == 0)
                    {
                        xcoeff = 1;
                    }
                    else
                    {
                        xcoeff = double.Parse(innerpoly.Derivate().ToString());
                    }
                    if (Indexofstart != 0)
                    {
                        secondcoeff = double.Parse(t.Coefficient.Substring(0, t.Coefficient.IndexOf("(")));
                    }
                    else
                    {
                        secondcoeff = 1;
                    }
                    double coeff = secondcoeff/(xcoeff*(t.Power + 1));
                    string newcoeff;
                    if (coeff ==1)
                    {
                        newcoeff = "(" + inner + ")";
                    }
                    else if (coeff == -1)
                    {
                        newcoeff = "-(" + inner + ")";

                    }
                    else
                    {
                        newcoeff = coeff + "(" + inner + ")";
                    }

                    double newpower = power + 1;
                    terms.Add(new ConjugateTerm(newpower,newcoeff));
                }
                else
                {
                    var pow = t.Power + 1;
                    var coeff = double.Parse(t.Coefficient) / (t.Power + 1);
                    terms.Add(new ConjugateTerm(pow, coeff.ToString()));
                }
            }
            
            return new ConjugatePoly(terms);
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

            ConjugatePoly integral = Integrate();

            result = integral.Calculate(end) - integral.Calculate(start);

            return result;
        }

        /// <summary>
        /// Finds the center of the polynom on x-axis in the range given.
        /// </summary>
        /// <param name="start">The start point in x-axis of the polynom.</param>
        /// <param name="end">The end point in x-axis of the polynom.</param>
        /// <returns>The x-axis of center of area of the polynom.</returns>
        /*public double LoadCenter(double start, double end)
        {
            double centerpoint = 0;

            var p1 = new ConjugatePoly("x");

            ConjugatePoly p2 = p1 * this;

            centerpoint = p2.DefiniteIntegral(start, end) / this.DefiniteIntegral(start, end);

            return centerpoint;
        }*/

        private string GetInnerTerm(ConjugateTerm term)
        {
            if (term.Coefficient.Contains("("))
            {
                int Indexofstart = term.Coefficient.IndexOf("(");
                int Indexofend = term.Coefficient.IndexOf(")");

                string innerterm = term.Coefficient.Substring(Indexofstart + 1, Indexofend - Indexofstart - 1);

                return innerterm;
            }
            else
            {
                return null;
            }
        }

        public string TrimEnd(string inputText, string value, StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(value))
            {
                while (!string.IsNullOrEmpty(inputText) && inputText.EndsWith(value, comparisonType))
                {
                    inputText = inputText.Substring(0, (inputText.Length - value.Length));
                }
            }

            return inputText;
        }

        #endregion

        #region Fields & Properties:

        /// <summary>
        /// Terms Property, Type of TermCollection
        /// </summary>
        private ConjugateTermCollection _Terms;
        public ConjugateTermCollection Terms
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
        public static ConjugatePoly operator +(ConjugatePoly p1, ConjugatePoly p2)
        {
            var result = new ConjugatePoly(p1.ToString());
            foreach (ConjugateTerm t in p2.Terms)
                result.Terms.Add(t);
            return result;
        }

        /// <summary>
        /// Minus Operations: Like Plus Operation but at first we just Make the Second Poly to the Negetive Value.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /*public static ConjugatePoly operator -(ConjugatePoly p1, ConjugatePoly p2)
        {
            var result = new ConjugatePoly(p1.ToString());
            var NegetiveP2 = new ConjugatePoly(p2.ToString());
            foreach (ConjugateTerm t in NegetiveP2.Terms)
                t.Coefficient *= -1;

            return result + NegetiveP2;
        } */

        /// <summary>
        /// Multiple Operation: For each term in the First Poly We Multiple it in the Each Term of Second Poly
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /*public static ConjugatePoly operator *(ConjugatePoly p1, ConjugatePoly p2)
        {
            var result = new ConjugateTermCollection();
            int counter = 0;
            foreach (ConjugateTerm t1 in p1.Terms)
            {
                foreach (ConjugateTerm t2 in p2.Terms)
                {
                    result.Add(new ConjugateTerm(t1.Power + t2.Power, t1.Coefficient * t2.Coefficient));
                    counter++;
                }
            }
            return new ConjugatePoly(result);
        } */

        /// <summary>
        /// Divide operation.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /*public static ConjugatePoly operator /(ConjugatePoly p1, ConjugatePoly p2)
        {
            p1.Terms.Sort(ConjugateTermCollection.SortType.DES);
            p2.Terms.Sort(ConjugateTermCollection.SortType.DES);
            var resultTerms = new ConjugateTermCollection();
            if (p1.Terms[0].Power < p2.Terms[0].Power)
                throw new Exception("Invalid Division: P1.MaxPower is Lower than P2.MaxPower");
            while (p1.Terms[0].Power > p2.Terms[0].Power)
            {
                ConjugateTerm NextResult = new ConjugateTerm(p1.Terms[0].Power - p2.Terms[0].Power, p1.Terms[0].Coefficient / p2.Terms[0].Coefficient);
                resultTerms.Add(NextResult);
                ConjugatePoly TempPoly = NextResult;

                ConjugatePoly NewPoly = TempPoly * p2;
                p1 = p1 - NewPoly;
            }
            return new ConjugatePoly(resultTerms);
        } */

        /// <summary>
        /// this will Create a new Poly by the Value of 1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static ConjugatePoly operator ++(ConjugatePoly p1)
        {
            var p2 = new ConjugatePoly("1");
            p1 = p1 + p2;
            return p1;
        }

        /// <summary>
        /// this will Create a new Poly by the Value of -1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static ConjugatePoly operator --(ConjugatePoly p1)
        {
            var p2 = new ConjugatePoly("-1");
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
        public static implicit operator ConjugatePoly(ConjugateTerm t)
        {
            var Terms = new ConjugateTermCollection();
            Terms.Add(t);
            return new ConjugatePoly(Terms);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static implicit operator ConjugatePoly(string expression)
        {
            return new ConjugatePoly(expression);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator ConjugatePoly(int value)
        {
            return new ConjugatePoly(value.ToString());
        }
        #endregion
    }
}
