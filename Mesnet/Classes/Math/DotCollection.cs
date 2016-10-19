using System;
using System.Collections;
using System.Collections.Generic;

namespace Mesnet.Classes.Math
{
    public class DotCollection:CollectionBase
    {
        public DotCollection()
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

