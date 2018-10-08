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

using System.IO;
using System.Text;
using Mesnet.Classes.IO.Json;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Mesnet.Classes.IO.Reporter
{
    /// <summary>
    /// A report created when the application has crashed.
    /// It includes exception information, debug logs and drawing content.
    /// </summary>
    /// <seealso cref="Mesnet.Classes.IO.Reporter.Report" />
    public class CrashReport : Report
    {
        public CrashReport(object Exception, bool isterminating)
        {
            _type = Global.ReportType.CrashReport;
            _exception = Exception.ToString();
            _isterminating = isterminating;
            CreateContent();
        }

        public void CreateContent()
        {
            var drawingcontent = MesnetIOJson.GetCurrentDrawingAsJson();
            var logcotent = MesnetIOJson.GetDebugLogsAsJson();

            _sb = new StringBuilder();
            StringWriter sw = new StringWriter(_sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartObject();
                writer.WritePropertyName("ReportType");
                writer.WriteValue((int)_type);
                writer.WritePropertyName("ExceptionObject");              
                writer.WriteValue(_exception);
                writer.WritePropertyName("IsTerminating");
                writer.WriteValue(_isterminating.ToString());
                writer.WritePropertyName("Drawing File");
                writer.WriteValue(drawingcontent);
                writer.WritePropertyName("Debug Logs");
                writer.WriteValue(logcotent);
                writer.WriteEndObject();
            }
        }


        private string _exception;

        private bool _isterminating;

        private string _drawingcontent;
    }
}
