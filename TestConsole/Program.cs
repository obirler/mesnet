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
using Mesnet.Classes;
using Mesnet.Classes.Math;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.SetDecimalSeperator();
            while (true)
            {
                Console.Write("Enter polynomial expression : ");
                var exp = Console.ReadLine();
                try
                {
                    if (!Poly.ValidateExpression(exp))
                    {
                        Console.WriteLine("Invalid polynomial!");
                    }
                    else
                    {
                        var poly = new Poly(exp);
                        Console.Write(poly.ToString());
                        if (poly.IsLinear())
                        {
                            Console.WriteLine(" is linear");
                        }
                        else
                        {
                            Console.WriteLine(" is not linear");
                        }
                    }                  
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid polynomial!");
                }
            }
       
            Console.ReadKey();
        }
    }
}
