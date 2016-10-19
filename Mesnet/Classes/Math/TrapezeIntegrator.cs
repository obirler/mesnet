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
