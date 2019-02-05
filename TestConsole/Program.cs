using System;
using Mesnet.Classes.Math;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var poly = new Poly("x", 4.9, 5);

            var min = poly.Minimum();

            Console.WriteLine("function returned");
       
            Console.ReadKey();
        }

        private static void check()
        {
            return;
        }
    }
}
