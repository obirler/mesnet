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

using System.Reflection;
using System.Security.Principal;
using Mesnet.Classes;

namespace Mesnet
{
    public static class Config
    {
        public static string AppName = "Mesnet";

        public static string VersionNumber =
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string UserName = WindowsIdentity.GetCurrent().Name;

        public static string ServerUrl = "http://mesnet.pythonanywhere.com";

        //public static string ServerUrl = "http://127.0.0.1:80";

        public static string LogUrl = ServerUrl + "/log";

        public static string UserRegisterUrl = ServerUrl + "/user";

        public static string NewVersionKey = "newversion";

        public static string NewVersionUrl = "newversionurl";

        public static bool CommunicateWithServer = true;

        public static double Scale = 1;

        public static double SimpsonStep = 0.0001;

        public static double CrossLoopTreshold = 0.00001;

        public static Global.LanguageType Language = Global.LanguageType.English;

        public static Global.CalculationType Calculation = Global.CalculationType.MultiThreaded;

        //Write debug logs to database so that it can be analyzed later
        public static bool LogInRelease = false;

        public static string ReleaseLogDbName = "releaselog.db";

        public static string TelemetryDbName = "telemetry.db";

        public static string SettingsDbName = "settings.db";

        public static string CrossLogFileName = "log.txt";

        public static int MaxReporterThreadCount = 2;

        public static int MaxDatabaseLoggerThreadCount = 3;
    }

}
