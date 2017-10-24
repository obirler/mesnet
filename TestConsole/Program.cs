using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Mesnet.Classes.IO.Xml;
using Mesnet.Classes.Tools;
using System.Diagnostics;
using Mesnet.Classes.Math;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var testpoly = new Poly("x^3-6x^2+11x-6", 0, 4);
            var dict = testpoly.CalculateMagnitudeAndLocation();

            Console.WriteLine("Locations and magnitudes:");
            foreach (var pair in dict)
            {
                Console.WriteLine("Location : " + pair.Key + " Magnitude : " + pair.Value);
            }

            Console.ReadKey();
        }
    }
}
