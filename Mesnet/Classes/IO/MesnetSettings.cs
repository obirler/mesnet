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
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace Mesnet.Classes.IO
{
    public static class MesnetSettings
    {
        //Static constructor that runs only once when one of the static methods of the class called at the first time
        static MesnetSettings()
        {
            if (!File.Exists(_dbname))
            {
                SQLiteConnection.CreateFile(_dbname);
            }
            _connection = new SQLiteConnection("Data Source=" + _dbname + ";MesnetVersion=3;");
            _connection.Open();
            string sql = "create table if not exists MesnetSettings (Id integer primary key, SettingKey text, SettingValue text)";
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            lock (_connection)
            {
                command.ExecuteNonQuery();
            }
        }

        private const string _dbname = "settings.db";

        private static SQLiteConnection _connection;

        public static void WriteSetting(string settingname, string settingvalue)
        {
            if (IsSettingExists(settingname))
            {
                Update(settingname, settingvalue);
            }
            else
            {
                Insert(settingname, settingvalue);
            }
        }

        public static string ReadSetting(string settingname)
        {
            if (IsSettingExists(settingname))
            {
                SQLiteDataReader rdr;
                SQLiteCommand contentCommand;
                String cmd = String.Format("select * from MesnetSettings where SettingKey ='{0}'", settingname);

                contentCommand = _connection.CreateCommand();
                contentCommand.CommandText = cmd;
                rdr = contentCommand.ExecuteReader();
                if (rdr.Read())
                {
                    return rdr.GetString(2);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static void Insert(string settingname, string settingvalue)
        {
            try
            {
                string commandtext = String.Format("insert into MesnetSettings (Id, SettingKey, SettingValue) values (NULL,'{0}','{1}')", settingname, settingvalue);

                using (SQLiteCommand comm = new SQLiteCommand(commandtext, _connection))
                {
                    lock (_connection)
                    {
                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Sql exception : " + ex.Message);
            }
        }

        private static void Update(string settingname, string settingvalue)
        {
            try
            {
                string commandtext = String.Format("update MesnetSettings set SettingValue = '{1}' where SettingKey = '{0}'", settingname, settingvalue);
                using (SQLiteCommand comm = new SQLiteCommand(commandtext, _connection))
                {
                    lock (_connection)
                    {
                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Sql exception : " + ex.Message);
            }
        }

        public static bool IsSettingExists(string settingkey)
        {
            SQLiteCommand command = new SQLiteCommand(_connection);
            command.CommandText =
                String.Format("select count(*) from MesnetSettings where SettingKey='{0}'", settingkey);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public static void Close()
        {
            _connection.Close();
        }
    }
}
