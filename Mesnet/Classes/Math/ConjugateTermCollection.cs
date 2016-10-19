using System;
using System.Collections;

namespace Mesnet.Classes.Math
{
    public class ConjugateTermCollection : CollectionBase
    {
        #region Custom Enum Definition:
        /// <summary>
        /// Array Sort Type
        /// Values: ASC , DESC
        /// </summary>
        public enum SortType
        {
            ASC = 0,
            DES = 1
        }

        #endregion

        #region Custom Methods:

        /// <summary>
        /// Sorts Items of Collection in ASC or DESC Order.
        /// </summary>
        /// <param name="Order">Sort Order values : ASC or DESC</param>
        public void Sort(SortType Order)
        {
            var result = new ConjugateTermCollection();
            if (Order == SortType.ASC)
            {
                while (this.Length > 0)
                {
                    ConjugateTerm MinTerm = this[0];
                    foreach (ConjugateTerm t in List)
                    {
                        if (t.Power < MinTerm.Power)
                        {
                            MinTerm = t;
                        }
                    }
                    result.Add(MinTerm);
                    this.Remove(MinTerm);
                }
            }
            else
            {
                while (this.Length > 0)
                {
                    ConjugateTerm MaxTerm = this[0];
                    foreach (ConjugateTerm t in List)
                    {
                        if (t.Power > MaxTerm.Power)
                        {
                            MaxTerm = t;
                        }
                    }
                    result.Add(MaxTerm);
                    this.Remove(MaxTerm);
                }
            }

            this.Clear();
            foreach (ConjugateTerm t in result)
            {
                this.Add(t);
            }
        }

        /// <summary>
        /// Adds the Term to the Same Power Term if it's already exists in the Collection.
        /// This mean we Plus the New Terms to the Current Polynomial
        /// </summary>
        /// <param name="value"></param>
        public void AddToEqualPower(ConjugateTerm value)
        {
            foreach (ConjugateTerm t in List)
            {
                if (value.Coefficient.Contains("("))
                {
                    if (t.Power == value.Power && GetInnerTerm(t) == GetInnerTerm(value))
                    {
                        var inner = GetInnerTerm(t);
                        string str1 = TrimEnd(t.Coefficient, "(" + GetInnerTerm(t) + ")");
                        double d1 = 0;
                        if (str1 == "-")
                        {
                            d1 = -1;
                        }
                        else if (str1 == "")
                        {
                            d1 = 1;
                        }
                        else if (str1 == "0")
                        {
                            d1 = 0;
                        }
                        else
                        {
                            d1 = double.Parse(str1);
                        }

                        string str2 = TrimEnd(value.Coefficient, "(" + GetInnerTerm(value) + ")");
                        double d2 = 0;
                        if (str2 == "-")
                        {
                            d2 = -1;
                        }
                        else if (str2 == "")
                        {
                            d2 = 1;
                        }
                        else if (str2 == "0")
                        {
                            d2 = 0;
                        }
                        else
                        {
                            d2 = double.Parse(str2);
                        }

                        var d3 = d1 + d2;
                        if (d3 == -1)
                        {
                            t.Coefficient = "-(" + inner + ")";
                        }
                        else if (d3 == 1)
                        {
                            t.Coefficient = "-(" + inner + ")";
                        }
                        else if (d3 == 0)
                        {
                            t.Coefficient = "0";
                        }
                        else
                        {
                            t.Coefficient = d3 + "(" + inner + ")";
                        }
                    }
                }
                else
                {
                    if (t.Power == value.Power)
                    {
                        var d1 = double.Parse(t.Coefficient.Replace(" ", ""));
                        var d2 = double.Parse(value.Coefficient.Replace(" ", ""));
                        var d3 = d1 + d2;

                        if (d3>0)
                        {
                            t.Coefficient = "+" + d3;
                        }
                        else
                        {
                            t.Coefficient = d3.ToString();
                        }
                    }                   
                }

            }
        }

        /// <summary>
        /// Checks if there is any Term by the given Power.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool HasSimilarTerm(ConjugateTerm term)
        {
            if (term.Coefficient.Contains("("))
            {
                foreach (ConjugateTerm t in List)
                {
                    if (t.Power == term.Power)
                    {
                        if (t.Coefficient.Contains("("))
                        {
                            if (GetInnerTerm(t) == GetInnerTerm(term))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (ConjugateTerm t in List)
                {
                    if (t.Power == term.Power)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

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

        #region Collection Members:

        /// <summary>
        /// Index Collections Definition:
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConjugateTerm this[int index]
        {
            get { return ((ConjugateTerm)List[index]); }
            set { List[index] = value; }
        }


        public int Length
        {
            get
            {
                return List.Count;
            }
        }


        /// <summary>
        /// Modified Add Method: 
        /// First check the Coefficient Value. 
        /// this Method checks if there is any Term by the Same Input Power.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(ConjugateTerm value)
        {
            if (value.Coefficient != "0")
            {
                if (this.HasSimilarTerm(value))
                {
                    this.AddToEqualPower(value);
                    return -1;
                }
                else
                    return (List.Add(value));
            }
            else
                return -1;
        }


        public int IndexOf(ConjugateTerm value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, ConjugateTerm value)
        {
            List.Insert(index, value);
        }

        public void Remove(ConjugateTerm value)
        {
            List.Remove(value);
        }

        public bool Contatins(ConjugateTerm value)
        {
            return (List.Contains(value));
        }

        #endregion
    }
}
