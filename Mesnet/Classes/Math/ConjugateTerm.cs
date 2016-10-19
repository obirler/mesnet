using System;

namespace Mesnet.Classes.Math
{
    public class ConjugateTerm
    {
        #region Constructors:

        /// <summary>
        /// Simple Constructor which Create a new Instanse of Conjugate Term Class
        /// With 2 parameters
        ///  
        /// </summary>
        /// <param name="power"></param>
        /// <param name="coefficient"></param>
        public ConjugateTerm(double power, string coefficient)
        {
            if (coefficient.Contains("(") && !coefficient.Contains(")"))
            {
                throw new InvalidOperationException("Unmatched Parenthesis");
            }
            else if (coefficient.Contains(")") && !coefficient.Contains("("))
            {
                throw new InvalidOperationException("Unmatched Parenthesis");
            }

            this.Power = power;
            this.Coefficient = coefficient;
        }

        /// <summary>
        /// Constructor Overload which Create a new Instance of Conjugate Term Class
        /// With a simple string and try to read the Power and Coefficient
        /// by identifing the input string
        /// </summary>
        /// <param name="TermExpression"></param>
        public ConjugateTerm(string TermExpression)
        {
            if (TermExpression.Length > 0)
            {
                if (TermExpression.Contains("(") && !TermExpression.Contains(")"))
                {
                    throw new InvalidOperationException("Unmatched Parenthesis");
                }
                else if (TermExpression.Contains(")") && !TermExpression.Contains("("))
                {
                    throw new InvalidOperationException("Unmatched Parenthesis");
                }

                if (TermExpression.Contains("(") && TermExpression.Contains(")"))
                {
                    if (TermExpression.IndexOf("^") > -1)
                    {
                        string CoefficientString = TermExpression.Substring(0, TermExpression.IndexOf("^"));
                        int Indexofstart = TermExpression.IndexOf("(");
                        int Indexofend = TermExpression.IndexOf(")");
                        if (Indexofstart != 0)
                        {
                            string secondcoeff = TermExpression.Substring(0, TermExpression.IndexOf("("));

                            string innercoeff = TermExpression.Substring(Indexofstart+1, Indexofend- Indexofstart-1);
                            var pol = new Poly(innercoeff);
                            CoefficientString = secondcoeff + "(" + pol.ToString() + ")";
                        }
                        else
                        {
                            string innercoeff = TermExpression.Substring(Indexofstart + 1, Indexofend - Indexofstart - 1);
                            var pol = new Poly(innercoeff);
                            CoefficientString = "(" + pol.ToString() + ")";
                        }
                        int IndexofX = TermExpression.IndexOf("^");
                        string PowerString = TermExpression.Substring(IndexofX + 1,
                            (TermExpression.Length - 1) - IndexofX);

                        Coefficient = CoefficientString;

                        Power = double.Parse(PowerString);
                    }
                    else
                    {
                        Coefficient = TermExpression;
                        Power = 1;
                    }
                }                
                else
                {
                    if (TermExpression.IndexOf("x^") > -1)
                    {
                        string CoefficientString = TermExpression.Substring(0, TermExpression.IndexOf("x^"));
                        int IndexofX = TermExpression.IndexOf("x^");
                        string PowerString = TermExpression.Substring(IndexofX + 2,
                            (TermExpression.Length - 1) - (IndexofX + 1));

                        if (CoefficientString == "-")
                            Coefficient = "-1";
                        else if (CoefficientString == "+" | CoefficientString == "")
                            Coefficient = "1";
                        else
                            Coefficient = CoefficientString;

                        Power = double.Parse(PowerString);
                    }
                    else if (TermExpression.IndexOf("x") > -1)
                    {
                        this.Power = 1;
                        string CoefficientString = TermExpression.Substring(0, TermExpression.IndexOf("x"));
                        if (CoefficientString == "-")
                            this.Coefficient = "-1";
                        else if (CoefficientString == "+" | CoefficientString == "")
                            this.Coefficient = "1";
                        else
                            this.Coefficient = CoefficientString;
                    }
                    else
                    {
                        this.Power = 0;
                        this.Coefficient = TermExpression;
                    }
                }
            }
            else
            {
                this.Power = 0;
                this.Coefficient = "0";
            }
        }
        #endregion

        #region Override Methods:
        /// <summary>
        /// This Override will push the Term in a String Form to the output.
        /// ToString() method will write the String Format of the Term Like: 3x^2
        /// Which means [Coefficient]x^[Power].
        /// This Method Also check if it's needed to have x^,x,- or + in the pattern.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string Result = string.Empty;
            if (Coefficient != "0")
            {
                if (Coefficient.Contains("("))
                {
                    if (!Coefficient.StartsWith("-"))
                    {
                        Result += "+";
                    }
                    Result += Coefficient;

                    if (Power == 0)
                    {                        
                        return "1";
                    }
                    else if (Power == 1)
                    {
                        return Result;
                    }
                    else
                    {
                        Result += "^";
                        Result += Power;
                        return Result;
                    }
                }
                else
                {
                    var coeff = double.Parse(Coefficient);
                    if (coeff > 0)
                        Result += "+";
                    else
                        Result += "-";

                    if (Power == 0)
                        Result += (coeff < 0 ? coeff * -1 : coeff).ToString();
                    else if (Power == 1)
                    {
                        if (coeff == 1)
                        {
                            Result += "x";
                        }
                        else if (coeff == -1)
                        {
                            Result += "x";
                        }
                        else
                        {
                            Result += string.Format("{0}x", (coeff < 0 ? coeff * -1 : coeff));
                        }
                    }
                    else
                    {
                        if (coeff == 1)
                        {
                            Result += "x^" + Power;
                        }
                        else if (coeff == -1)
                        {
                            Result += "x^" + Power;
                        }
                        else
                        {
                            Result += string.Format("{0}x^{1}", (coeff < 0 ? coeff * -1 : coeff), Power);
                        }

                    }
                }                               
            }
            return Result;    
        }

        #endregion

        #region Fields & Properties:
        /// <summary>
        /// Private field to hold the Power Value.
        /// </summary>
        private double _Power;

        /// <summary>
        /// Private field to hold the Cofficient Value.
        /// </summary>
        private string _coefficient;

        /// <summary>
        /// Power Property
        /// Notice: Set Method Check if the value is Negetive and Make it Positive.
        /// </summary>
        public double Power
        {
            get
            {
                return _Power;
            }
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("The power can not be less than zero!");                
                else
                    _Power = value;

            }
        }

        /// <summary>
        /// Coefficient Property
        /// </summary>
        public string Coefficient
        {
            get
            {
                return _coefficient;
            }
            set
            {
                if (value.Contains("(") && !value.Contains(")"))
                {
                    throw new InvalidOperationException("Unmatched Parenthesis");
                }
                else if (value.Contains(")") && !value.Contains("("))
                {
                    throw new InvalidOperationException("Unmatched Parenthesis");
                }
                else
                {
                    _coefficient = value;
                }                
            }
        }
        #endregion
    }
}
