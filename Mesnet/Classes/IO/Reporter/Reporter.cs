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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Threading;
using Mesnet.Classes.IO;
using Mesnet.Classes.IO.Reporter;
using Mesnet.Classes.Tools;
using RestSharp;

namespace Mesnet.Classes.Reporter
{
    public static class Reporter
    {
        static Reporter()
        {
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMaxThreads(Config.MaxReporterThreadCount, minIOC);

            if (!File.Exists(Config.TelemetryDbName))
            {
                MesnetDebug.WriteInformation("Telemetry database file does not exists, creating");
                SQLiteConnection.CreateFile(Config.TelemetryDbName);
            }        

            _connection = new SQLiteConnection("Data Source=" + Config.TelemetryDbName + ";Version=3;");
            _connection.Open();

            string sql = "create table if not exists logs (id INTEGER PRIMARY KEY, username text, appname text, version text, type text, createdate text, content text)";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            lock (_connection)
            {
                command.ExecuteNonQuery();
            }

            _bwsend = new BackgroundWorker();
            _bwsend.DoWork += BwSendOnDoWork;
            _ids = new List<int>();
        }

        private static SQLiteConnection _connection;

        private static BackgroundWorker _bwsend;

        public static void Write(Entry entry)
        {
            var log = entry.Create();
            ThreadPool.QueueUserWorkItem(new WaitCallback(writelog), log);
        }

        public static void WriteSynchronously(Entry entry)
        {
            var log = entry.Create();
            writelog(log);
        }

        public static void TryToSend()
        {
            if (Config.CommunicateWithServer)
            {
                MesnetDebug.WriteInformation("TryToSend started to work");
                StartNewBw();
            }            
        }

        public static void RegisterUser()
        {
            if (Config.CommunicateWithServer)
            {
                var bw = new BackgroundWorker();
                bw.DoWork += delegate
                {
                    try
                    {
                        if (InternetAvailability.IsInternetAvailable())
                        {
                            MesnetDebug.WriteInformation("Internet connection is available");
                            string id = MesnetSettings.ReadSetting("userid");
                            DateTime time = DateTime.Now.ToUniversalTime();
                            string createdate = time.ToString("dd/MM/yyyy HH:mm:ss");
                            try
                            {
                                System.Net.ServicePointManager.Expect100Continue = false;

                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "multipart/form-data");
                                request.AddParameter("username", Config.UserName);
                                request.AddParameter("appname", Config.AppName);
                                request.AddParameter("version", Config.VersionNumber);
                                request.AddParameter("createdate", createdate);
                                request.AddParameter("userid", id);

                                var client = new RestClient(Config.UserRegisterUrl);

                                var response = client.Execute(request);

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    MesnetDebug.WriteInformation("Restsharp user register was successful");
                                    MesnetSettings.WriteSetting("userregistered", "true");
                                }
                                else
                                {
                                    MesnetDebug.WriteError("Restsharp user register was unsuccessful, error message : " + response.ErrorMessage);
                                    MesnetSettings.WriteSetting("userregistered", "false");
                                    MesnetSettings.WriteSetting("userregistered", "false");
                                }
                            }
                            catch (Exception e)
                            {
                                MesnetDebug.WriteError("Error in register user : " + e.Message);
                                MesnetSettings.WriteSetting("userregistered", "false");
                            }
                        }
                        else
                        {
                            MesnetDebug.WriteInformation("Internet connection is not available");
                            MesnetSettings.WriteSetting("userregistered", "false");
                        }
                    }
                    catch (Exception e)
                    {
                        MesnetDebug.WriteError(e.Message);
                        MesnetSettings.WriteSetting("userregistered", "false");
                    }
                };
                bw.RunWorkerAsync();
            }          
        }

        private static void writelog(object a)
        {
            try
            {
                var log = a as Log;
                string commandtext = String.Format("insert into logs (id, username, appname, version, type, createdate, content) values (NULL, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", log.UserName, log.AppName, log.Version, log.Type, log.CreatedDate, log.Content);

                using (var command = new SQLiteCommand(commandtext, _connection))
                {
                    lock (_connection)
                    {
                        command.ExecuteNonQuery();
                        MesnetDebug.WriteInformation("Log has been written");
                    }
                }
            }
            catch (Exception e)
            {
                MesnetDebug.WriteError("Log couldnt be written : " + e.Message);
            }            
        }

        private static List<int> _ids;

        private static void BwSendOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            if (Config.CommunicateWithServer)
            {
                try
                {
                    if (InternetAvailability.IsInternetAvailable())
                    {
                        if (!File.Exists(Config.TelemetryDbName))
                        {
                            MesnetDebug.WriteWarning("Database file does not exists");
                            return;
                        }
                        MesnetDebug.WriteInformation("Internet connection is available");
                        SQLiteDataReader rdr;
                        SQLiteCommand contentCommand;
                        String cmd = "select id from logs";

                        lock (_connection)
                        {
                            contentCommand = _connection.CreateCommand();
                            contentCommand.CommandText = cmd;
                            rdr = contentCommand.ExecuteReader();
                        }

                        if (rdr.Read())
                        {
                            for (int i = 0; i < rdr.StepCount; i++)
                            {
                                _ids.Add((int)rdr.GetInt64(i));
                            }
                            StartNewBw();
                        }
                        else
                        {
                            MesnetDebug.WriteInformation("No data available in database");
                        }
                    }
                    else
                    {
                        MesnetDebug.WriteInformation("Internet connection is not available");
                    }
                }
                catch (Exception e)
                {
                    MesnetDebug.WriteError(e.Message);
                }
            }           
        }

        private static void StartNewBw()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Send;
            bw.RunWorkerCompleted += SendCompleted;
            bw.RunWorkerAsync();
        }     

        private static void Send(object senderbw, DoWorkEventArgs ebw)
        {
            ebw.Result = -1;
            SQLiteDataReader rdr;
            SQLiteCommand contentCommand;
            String cmd = "select min(id) from logs";
            lock (_connection)
            {
                contentCommand = _connection.CreateCommand();
                contentCommand.CommandText = cmd;
                rdr = contentCommand.ExecuteReader();
            }

            int id = Int32.MinValue;

            if (rdr.Read())
            {
                try
                {
                    id = (int)rdr.GetInt64(0);
                    MesnetDebug.WriteInformation("BW started for posting for Database id " + id);
                }
                catch (Exception e)
                {
                    MesnetDebug.WriteError("BW started cant get Database id. Exception = " + e.Message);
                    ebw.Result = -7;
                    return;
                }               
            }
            else
            {
                MesnetDebug.WriteWarning("No data available in database");
                ebw.Result = -8;
                return;
            }

            cmd = "select * from logs where id=" + id;

            lock (_connection)
            {
                contentCommand = _connection.CreateCommand();
                contentCommand.CommandText = cmd;
                rdr = contentCommand.ExecuteReader();
            }

            var log = new Log();

            try
            {
                if (rdr.Read())
                {
                    try
                    {
                        log.UserName = rdr.GetString(1);
                        log.AppName = rdr.GetString(2);
                        log.Version = rdr.GetString(3);
                        log.Type = rdr.GetString(4);
                        log.CreatedDate = rdr.GetString(5);
                        log.Content = rdr.GetString(6);

                        try
                        {
                            System.Net.ServicePointManager.Expect100Continue = false;

                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "multipart/form-data");
                            request.AddParameter("username", log.UserName);
                            request.AddParameter("appname", log.AppName);
                            request.AddParameter("version", log.Version);
                            request.AddParameter("type", log.Type);
                            request.AddParameter("createdate", log.CreatedDate);
                            request.AddParameter("content", log.Content);

                            var client = new RestClient(Config.LogUrl);
                            var response = client.Execute(request);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                MesnetDebug.WriteInformation("Restsharp post was successful");
                                try
                                {
                                    string command = String.Format("delete from logs where id={0}", id);
                                    using (SQLiteCommand comm = new SQLiteCommand(command, _connection))
                                    {
                                        lock (_connection)
                                        {
                                            comm.ExecuteNonQuery();
                                        }
                                    }
                                    MesnetDebug.WriteInformation("Log delete was successful");
                                    ebw.Result = 0;
                                }
                                catch (SqlException ex)
                                {
                                    MesnetDebug.WriteError("Delete unsuccessful : " + ex.Message);
                                    ebw.Result = -2;
                                }
                            }
                            else
                            {
                                MesnetDebug.WriteError("Restsharp post was unsuccesful, error message : " + response.ErrorMessage);
                                ebw.Result = -3;
                            }
                        }
                        catch (Exception e)
                        {
                            MesnetDebug.WriteError("Error in post : " + e.Message);
                            ebw.Result = -4;
                        }
                    }
                    catch (Exception e)
                    {
                        MesnetDebug.WriteError("Error in reading database : " + e.Message);
                        ebw.Result = -5;
                    }
                }
                else
                {
                    MesnetDebug.WriteWarning("No data available in database");
                    ebw.Result = -6;
                }
            }
            catch (Exception e)
            {
                MesnetDebug.WriteError("Couldnt read database : " + e.Message);
                ebw.Result = -7;
            }                   
        }

        private static void SendCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            int result = (int)runWorkerCompletedEventArgs.Result;
            MesnetDebug.WriteInformation("Send Bw completed result = " + result);

            if (InternetAvailability.IsInternetAvailable())
            {
                switch ((int)runWorkerCompletedEventArgs.Result)
                {
                    case 0:
                        MesnetDebug.WriteInformation("Send Bw completed");
                        StartNewBw();
                        break;
                }
            }          
        }
        
        public static void Close()
        {
            _connection.Close();
        }      
    }
}
