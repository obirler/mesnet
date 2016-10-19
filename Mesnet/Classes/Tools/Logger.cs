using System;
using System.IO;

namespace Mesnet.Classes.Tools
{
    public static class Logger
    {
        private static StreamWriter stw;

        private static bool _isclosed = true;

        public static void InitializeLogger()
        {
            stw = new StreamWriter(@"log.txt");
            _isclosed = false;
        }

        public static void WriteLine(string message)
        {
            stw.WriteLine(message);
        }

        public static void NextLine()
        {
            stw.WriteLine("");
        }

        public static void SplitLine()
        {
            stw.WriteLine("-------------------------------------------------------------------------------------------------------");
        }

        public static void Write(string message)
        {
            stw.Write(message);
        }

        public static bool IsClosed()
        {
            return _isclosed;
        }

        public static void CloseLogger()
        {
            try
            {
                stw.Flush();
                stw.Close();
                _isclosed = true;
            }
            catch (Exception)
            {}
        }
    }
}
