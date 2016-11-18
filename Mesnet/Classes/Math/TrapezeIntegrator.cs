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
    public static class TrapezeIntegrator
    {
        public static List<Global.Func> Integrate(List<Global.Func> function, double precision= 0.001)
        {
            var integration = new List<Global.Func>();

            Global.Func value;
            value.id = 0;
            value.xposition = 0;
            value.yposition = 0;
            integration.Add(value);

            for (int i = 1; i < function.Count; i++)
            {
                value.id = function[i].id;
                value.xposition = function[i].xposition;
                value.yposition = value.yposition + (function[i - 1].yposition + function[i].yposition) /2*precision;
                integration.Add(value);
            }
            return integration;
        }
    }
}
