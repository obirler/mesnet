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
            var poly = new Poly("x^4");
            var newpoly = poly.Propagate(1);
            Console.WriteLine(newpoly.ToString());

            Console.ReadKey();
        }
    }
}
