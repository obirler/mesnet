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
using System.Collections.Generic;

namespace Mesnet.Classes.Math
{
    public class SimpsonIntegrator
    {
        public SimpsonIntegrator(double deltax)
        {
            datas = new List<double>();
            _sum = 0;
            _h = deltax;
        }

        private double _h;

        private double _sum;

        private List<double> datas;

        private double _result;

        public void AddData(double data)
        {
            datas.Add(data);
        }

        public void Calculate()
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (i == 0)
                {
                    _sum += datas[i];
                }
                else if (i == datas.Count - 1)
                {
                    _sum += datas[i];
                }
                else if (i % 2 == 0)
                {
                    _sum += 2 * datas[i];
                }
                else if (i % 2 == 1)
                {
                    _sum += 4 * datas[i];
                }
            }
            _result = _h/3*_sum;
        }

        public double Result
        {
            get { return _result; }
        }
    }
}
