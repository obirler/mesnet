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
using System.Diagnostics;
using System.IO;
using Mesnet.Classes.IO.Reporter;

namespace Mesnet.Classes.Tools
{
    public static class MesnetDebug
    {
        public static void WriteInformation(string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            string entry = "+ Info => " + info + " at Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") +
                           " => Line: " + callStack.GetFileLineNumber() + " : from " +
                           Path.GetFileName(callStack.GetFileName()) + " " + callStack.GetMethod();
#if DEBUG
            Debug.WriteLine(entry);
            Global.LogList.Add(entry);
#else
            if (Config.LogInRelease)
            {
                ReleaseLogger.Write(entry);
            }
#endif
        }

        public static void WriteWarning(string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            string entry = "! Warning => " + info + " at Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") +
                           " => Line: " + callStack.GetFileLineNumber() + " : from " +
                           Path.GetFileName(callStack.GetFileName()) + " " + callStack.GetMethod();
#if DEBUG
            Debug.WriteLine(entry);
            Global.LogList.Add(entry);
#else
            if (Config.LogInRelease)
            {
                ReleaseLogger.Write(entry);
            }
#endif
        }

        public static void WriteError(string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            string entry = "- Error => " + info + " at Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") +
                           " => Line: " + callStack.GetFileLineNumber() + " : from " +
                           Path.GetFileName(callStack.GetFileName()) + " " + callStack.GetMethod();
#if DEBUG
            Debug.WriteLine(entry);
            Global.LogList.Add(entry);
#else
            if (Config.LogInRelease)
            {
                ReleaseLogger.Write(entry);
            }
#endif
        }
    }
}
