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
using System.Data.SQLite;
using System.IO;
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

            if (!File.Exists(Config.ReleaseLogDbName))
            {
                MesnetDebug.WriteInformation("Releaselog database file does not exists, creating");
                SQLiteConnection.CreateFile(Config.ReleaseLogDbName);
            }

            _connection = new SQLiteConnection("Data Source=" + Config.ReleaseLogDbName + ";MesnetVersion=3;");
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

        private static SQLiteConnection _connection;
    }
}
