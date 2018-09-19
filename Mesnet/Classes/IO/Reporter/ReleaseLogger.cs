using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Mesnet.Classes.Tools;

namespace Mesnet.Classes.IO.Reporter
{
    static class ReleaseLogger
    {
        static ReleaseLogger()
        {
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMaxThreads(5, minIOC);

            if (!File.Exists(_dbname))
            {
                MesnetDebug.WriteInformation("Releaselog database file does not exists, creating");
                SQLiteConnection.CreateFile(_dbname);
            }

            _connection = new SQLiteConnection("Data Source=" + _dbname + ";MesnetVersion=3;");
            _connection.Open();

            string sql = "create table if not exists logs (id INTEGER PRIMARY KEY, entry text)";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            lock (_connection)
            {
                command.ExecuteNonQuery();
            }
        }

        public static void Write(string entry)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(writelog), entry);
        }

        private static void writelog(object entry)
        {
            try
            {
                var log = entry as string;
                log.Replace("\'", "\"");

                string commandtext = String.Format("insert into logs (id, entry) values (NULL, '{0}')", log);

                using (var command = new SQLiteCommand(commandtext, _connection))
                {
                    lock (_connection)
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {}
        }

        private static string _dbname = "releaselog.db";

        private static SQLiteConnection _connection;
    }
}
