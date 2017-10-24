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
using OrgMath = System.Math;
namespace Mesnet.Classes.Math
{
    public class Term
    {
        #region Constructors:

        /// <summary>
        /// Simple Constructor which Create a new Instanse of Term Class
        /// With 2 parameters
        ///  
        /// </summary>
        /// <param name="power"></param>
        /// <param name="coefficient"></param>
        public Term(double power,double coefficient)
        {
            this.Power = power;
            this.Coefficient = coefficient;
        }

        /// <summary>
        /// Constructor Overload which Create a new Instance of Term Class
        /// With a simple string and try to read the Power and Coefficient
        /// by identifing the input string
        /// </summary>
        /// <param name="TermExpression"></param>
        public Term(string TermExpression)
        {
            if (TermExpression.Length > 0)
            {
                if (TermExpression.IndexOf("x^") > -1)
                {
                    string CoefficientString = TermExpression.Substring(0, TermExpression.IndexOf("x^"));
                    int IndexofX = TermExpression.IndexOf("x^");
                    string PowerString = TermExpression.Substring(IndexofX + 2, (TermExpression.Length -1) - (IndexofX + 1));
                    if (CoefficientString == "-")
                        this.Coefficient = -1;
                    else if (CoefficientString == "+" | CoefficientString == "")
                        this.Coefficient = 1;
                    else
                        this.Coefficient = double.Parse(CoefficientString);
                    
                    this.Power = double.Parse(PowerString);
                }
                else if (TermExpression.IndexOf("x") > -1)
                {
                    this.Power = 1;
                    string CoefficientString = TermExpression.Substring(0, TermExpression.IndexOf("x"));
                    if (CoefficientString == "-")
                        this.Coefficient = -1;
                    else if (CoefficientString == "+" | CoefficientString == "")
                        this.Coefficient = 1;
                    else
                        this.Coefficient = double.Parse(CoefficientString);
                }
                else
                {
                    this.Power = 0;
                    this.Coefficient = double.Parse(TermExpression);
                }
            }
            else
            {
                this.Power = 0;
                this.Coefficient = 0;
            }
        }

        public Term()
        {
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
            if (Coefficient != 0)
            {
                if (this.Coefficient > 0)
                    Result += "+";
                else
                    Result += "-";

                if (this.Power == 0)
                    Result += (this.Coefficient < 0 ? this.Coefficient * -1 : this.Coefficient).ToString();
                else if (this.Power == 1)
                {
                    if (Coefficient == 1)
                    {
                        Result += "x";
                    }
                    else if (Coefficient == -1)
                    {
                        Result += "x";
                    }
                    else
                    {
                        Result += string.Format("{0}x",
                            (this.Coefficient < 0 ? this.Coefficient*-1 : this.Coefficient).ToString());
                    }
                }
                else
                {
                    if (Coefficient == 1)
                    {
                        Result += "x^" + this.Power.ToString();
                    }
                    else if (Coefficient == -1)
                    {
                        Result += "x^" + this.Power.ToString();
                    }
                    else
                    {
                        Result += string.Format("{0}x^{1}", (this.Coefficient < 0 ? this.Coefficient * -1 : this.Coefficient).ToString(), this.Power.ToString());
                    }
                    
                }                    
            }
            else
            {
                return "0";
            }
            return Result;
        }

        /// <summary>
        /// It is the same as ToString method but it rounds the numbers before turning them into string.
        /// </summary>
        /// <param name="digit">The digit.</param>
        /// <returns></returns>
        public string GetString(int digit)
        {
            string Result = string.Empty;
            if (Coefficient != 0)
            {
                if (this.Coefficient > 0)
                    Result += "+";
                else
                    Result += "-";

                if (this.Power == 0)
                    Result += (this.Coefficient < 0 ? OrgMath.Round(this.Coefficient * -1, digit) : OrgMath.Round(this.Coefficient, digit)).ToString();
                else if (this.Power == 1)
                {
                    if (Coefficient == 1)
                    {
                        Result += "x";
                    }
                    else if (Coefficient == -1)
                    {
                        Result += "x";
                    }
                    else
                    {
                        Result += string.Format("{0}x",
                            (this.Coefficient < 0 ? OrgMath.Round(this.Coefficient * -1, digit) : OrgMath.Round(this.Coefficient, digit)).ToString());
                    }
                }
                else
                {
                    if (Coefficient == 1)
                    {
                        Result += "x^" + this.Power.ToString();
                    }
                    else if (Coefficient == -1)
                    {
                        Result += "x^" + this.Power.ToString();
                    }
                    else
                    {
                        Result += string.Format("{0}x^{1}", (this.Coefficient < 0 ? OrgMath.Round(this.Coefficient * -1, digit) * -1 : OrgMath.Round(this.Coefficient, digit)).ToString(), this.Power.ToString());                      
                    }

                }

                if (Result[0] == '-')
                {
                    if (Result[1] == '-')
                    {
                        Result = Result.Remove(0, 1); 
                    }
                }

                if (Result[0] == '+')
                {
                    if (Result[1] == '+')
                    {
                        Result = Result.Remove(0, 1);
                    }
                }
            }
            else
            {
                return "0";
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
        private double _coefficient;

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
                    _Power = value * -1;
                else
                    _Power = value;
                
            }
        }

        /// <summary>
        /// Coefficient Property
        /// </summary>
        public double Coefficient
        {
            get
            {
                return _coefficient;
            }
            set
            {
                _coefficient = value;
            }
        }
        #endregion
    }
}
