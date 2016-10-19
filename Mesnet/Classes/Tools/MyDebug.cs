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
