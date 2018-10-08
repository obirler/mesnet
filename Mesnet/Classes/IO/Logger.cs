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
            stw = new StreamWriter(Config.CrossLogFileName);
            _isclosed = false;
        }

        public static void WriteLine(string message)
        {
            stw.WriteLine(message);
            Global.FileLogList.Add(message);
        }

        public static void NextLine()
        {
            stw.WriteLine("");
            Global.FileLogList.Add("");
        }

        public static void SplitLine()
        {
            stw.WriteLine("-------------------------------------------------------------------------------------------------------");
            Global.FileLogList.Add("-------------------------------------------------------------------------------------------------------");
        }

        public static void Write(string message)
        {
            stw.Write(message);
            Global.FileLogList.Add(message);
        }

        public static bool IsClosed()
        {
            return _isclosed;
        }

        public static void CloseLogger()
        {
            try
            {
                if (!_isclosed)
                {
                    stw.Flush();
                    stw.Close();           
                    _isclosed = true;
                }                
            }
            catch (Exception)
            {}
        }
    }
}
