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

namespace Mesnet.Classes.IO.Reporter
{
    public class Entry
    {
        public Entry(string type, Report report)
        {
            _type = type;
            _report = report;
        }

        private string _type;

        private Report _report;

        public Log Create()
        {
            var log = new Log();
            log.AppName = Config.AppName;
            log.UserName = Config.UserName;
            string correctedcontent = _report.GetContent.Replace("'", "''");
            log.Content = correctedcontent;
            log.Type = _type;
            DateTime time = DateTime.Now.ToUniversalTime();
            string createdate = time.ToString("dd/MM/yyyy HH:mm:ss");
            log.CreatedDate = createdate;
            log.Version = Config.VersionNumber;
            return log;
        }
    }
}
