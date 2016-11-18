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

namespace Mesnet.Classes.Tools
{
    public static class MyDebug
    {
        public static void WriteInformation(string tag, string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            Debug.WriteLine("+++++ Info => Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") + " => Line: " + callStack.GetFileLineNumber() + " : " + tag + ": " + info);
        }

        public static void WriteWarning(string tag, string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            Debug.WriteLine("!!!!! Warning => Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") + " => Line: " + callStack.GetFileLineNumber() + " : " + tag + ": " + info);
        }

        public static void WriteError(string tag, string info)
        {
            DateTime time = DateTime.Now;
            StackFrame callStack = new StackFrame(1, true);
            Debug.WriteLine("----- ERROR => Date: " + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") + " => Line: " + callStack.GetFileLineNumber() + tag + ": " + info);
        }
    }
}
