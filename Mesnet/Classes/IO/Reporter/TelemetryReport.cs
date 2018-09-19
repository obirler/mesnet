using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Mesnet.Classes.IO.Json;
using Mesnet.Classes.IO.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Mesnet.Classes.IO.Reporter
{
    /// <summary>
    /// A report send when specific numbers have been reached. 
    /// It is used for statics and telemetry purposes.
    /// </summary>
    /// <seealso cref="Mesnet.Classes.IO.Reporter.Report" />
    public class TelemetryReport : Report
    {
        public TelemetryReport()
        {
            _type = Global.ReportType.TelemetryReport;
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
                writer.WritePropertyName("Drawing File");
                writer.WriteValue(drawingcontent);
                writer.WritePropertyName("Debug Logs");
                writer.WriteValue(logcotent);
                writer.WriteEndObject();
            }
        }
    }
}
