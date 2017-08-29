﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mesnet.Classes.IO
{
    public class DatabaseLogger
    {
        static DatabaseLogger()
        {
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMaxThreads(3, minIOC);

            SQLiteConnection.CreateFile(_dbname);

            _connection = new SQLiteConnection("Data Source=" + _dbname + ";Version=3;");
            _connection.Open();
           
            string sql = "create table if not exists userlog (id INTEGER PRIMARY KEY, logtye text, logmessage text, time text, info text)";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            lock (_connection)
            {                                             
                command.ExecuteNonQuery();
            }
        }
 
        private const string _dbname = "dblog.sqlite";

        private static SQLiteConnection _connection;

        public static void Write(string message, string type)
        {
            StackFrame frame = new StackFrame(1, true);
            var info = new ThreadInfo(frame, message, type);
            ThreadPool.QueueUserWorkItem(new WaitCallback(dowork), info);
        }

        private static void dowork(object a)
        {
            DateTime time = DateTime.Now;
            var threadinfo = a as ThreadInfo;
            StackFrame callStack = threadinfo.StackFrame;
            string info = "Line " + callStack.GetFileLineNumber() + " from " + Path.GetFileName(callStack.GetFileName()) + " " + callStack.GetMethod();
            string command = "insert into userlog (id,logmessage,time,info) values (NULL,'" + threadinfo.Message + "','" + time.ToString("dd/MM/yyyy , hh:mm:ss:FFFF") + "','" + info + "')";

            SQLiteCommand comm = new SQLiteCommand(command, _connection);
            lock(_connection)
            {               
                comm.ExecuteNonQuery();
            }
        }

        public static void Close()
        {
            _connection.Close();
        }

        public class ThreadInfo
        {
            public ThreadInfo(StackFrame frame, string message, string type)
            {
                StackFrame = frame;
                Message = message;
                Type = type;
            }

            public StackFrame StackFrame { get; set; }

            public string Message { get; set; }

            public string Type { get; set; }
        }
    }
}
