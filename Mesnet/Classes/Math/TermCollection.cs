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

namespace Mesnet.Classes.Math
{
    public class TermCollection:CollectionBase
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
        /// Sor Method: Sort Items of Collection in ASC or DESC Order.
        /// </summary>
        /// <param name="Order">SortOrder values : ASC or DESC</param>
        public void Sort(SortType Order)
        {
            TermCollection result = new TermCollection();
            if (Order == SortType.ASC)
            {
                while (this.Length > 0)
                {
                    Term MinTerm = this[0];
                    foreach (Term t in List)
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
                    Term MaxTerm = this[0];
                    foreach (Term t in List)
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
            foreach (Term t in result)
            {
                this.Add(t);
            }
        }

        /// <summary>
        /// This will Add the Term to the Same Power Term if it's already exists in the Collection.
        /// This mean we Plus the New Terms to the Currnet Polynomial
        /// </summary>
        /// <param name="value"></param>
        public void AddToEqualPower(Term value)
        {
            bool remove = false;
            var removeterm =new Term();
            foreach (Term t in List)
            {
                if (t.Power == value.Power)
                {
                    t.Coefficient += value.Coefficient;
                    if (t.Coefficient == 0)
                    {
                        removeterm = t;
                        remove = true;
                    }
                }                    
            }

            if (remove)
            {
                List.Remove(removeterm);
            }
        }

        /// <summary>
        /// Check if there is any Term by the given Power.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool HasTermByPower(double p)
        {
            foreach (Term t in List)
            {
                if (t.Power == p)
                    return true;
            }
            return false;
        }
        #endregion

        #region Collection Members:

        /// <summary>
        /// Index Collections Definition:
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Term this[int index]
        {
            get { return ((Term)List[index]); }
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
        public int Add(Term value)
        {
            if (value.Coefficient != 0)
            {
                if (this.HasTermByPower(value.Power))
                {
                    this.AddToEqualPower(value);
                    return -1;
                }
                else
                    return (List.Add(value));
            }
            else
            {
                if (value.Coefficient == 0 && value.Power == 0)
                {
                    return (List.Add(value));
                }
                else
                {
                    return -1;
                }
            }
        }
        

        public int IndexOf(Term value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Term value)
        {
            List.Insert(index, value);
        }

        public void Remove(Term value)
        {
            List.Remove(value);
        }

        public bool Contatins(Term value)
        {
            return (List.Contains(value));
        }

        #endregion

    }
}
