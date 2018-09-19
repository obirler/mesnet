using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Mesnet.Classes.IO.Json;
using Mesnet.Classes.IO.Xml;
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
