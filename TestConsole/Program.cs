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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            for(int i=0; i<10000;i++)
            {
                Mesnet.Classes.IO.DatabaseLogger.Write(i.ToString(), "information");
            }
            //Mesnet.Classes.IO.DatabaseLogger.Close();
            //sw.Stop();
            //Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString()+ " ms");
            Console.WriteLine("Log has been written");
            Console.ReadKey();
        }
    }
}
