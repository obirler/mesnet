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

namespace Mesnet.Classes.Math
{
    public class KeyValueCollection:CollectionBase
    {
        public KeyValueCollection()
        {
        }

        public void Add(double xpos, double ypos)
        {
            var pair = new KeyValuePair<double, double>(xpos, ypos);
            List.Add(pair);         
        }

        public bool ContainsKey(double xpos)
        {
            for (int i = 0; i < List.Count; i++)
            {
                KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                if (item.Key == xpos)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsValue(double ypos)
        {
            for (int i = 0; i < List.Count; i++)
            {
                KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                if (item.Value == ypos)
                {
                    return true;
                }
            }
            return false;
        }

        public KeyValuePair<double, double> this[int index]
        {
            get { return (KeyValuePair<double, double>)List[index]; }
            set { List[index] = value; }
        }

        public double Calculate(double x)
        {
            if (x < this[0].Key || x > this[this.Count - 1].Key)
            {
                return 0;
            }
            else if (this.ContainsKey(x))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (x == this[i].Key)
                    {
                        return this[i].Value;
                    }
                }
                return 0;
            }
            else
            {
                for (int i = 1; i < this.Count; i++)
                {
                    if (x > this[i - 1].Key && x < this[i].Key)
                    {
                        //Linear interpolation
                        return this[i - 1].Value +
                               (x - this[i - 1].Key) * (this[i].Value - this[i - 1].Value) /
                               (this[i].Key - this[i - 1].Key);
                    }
                }
            }
            return 0;
        }

        public double YMax
        {
            get
            {
                double max = Double.MinValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>) List[i];
                    if (item.Value > max)
                    {
                        max = item.Value;
                    }
                }
                return max;
            }
        }

        public double YMaxAbs
        {
            get
            {
                double max = Double.MinValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                    if (System.Math.Abs(item.Value) > max)
                    {
                        max = System.Math.Abs(item.Value);
                    }
                }
                return max;
            }
        }

        public double YMin
        {
            get
            {
                double max = Double.MaxValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                    if (item.Value < max)
                    {
                        max = item.Value;
                    }
                }
                return max;
            }
        }

        public double YMinAbs
        {
            get
            {
                double max = Double.MaxValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                    if (System.Math.Abs(item.Value) < max)
                    {
                        max = System.Math.Abs(item.Value);
                    }
                }
                return max;
            }
        }

        public double YMaxPosition
        {
            get
            {
                double pos = 0;
                double max = Double.MinValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                    if (item.Value > max)
                    {
                        max = item.Value;
                        pos = item.Key;
                    }
                }
                return pos;
            }
        }

        public double YMinPosition
        {
            get
            {
                double pos = 0;
                double max = Double.MaxValue;
                for (int i = 0; i < List.Count; i++)
                {
                    KeyValuePair<double, double> item = (KeyValuePair<double, double>)List[i];
                    if (item.Value < max)
                    {
                        max = item.Value;
                        pos = item.Key;
                    }
                }
                return pos;
            }
        }
    }
}

