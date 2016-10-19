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
