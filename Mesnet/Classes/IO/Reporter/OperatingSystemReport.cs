using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mesnet.Classes.IO.Xml;
using Mesnet.Classes.Tools;
using Newtonsoft.Json;

namespace Mesnet.Classes.IO.Reporter
{
    /// <summary>
    /// A report used for operation system information about client machine.
    /// </summary>
    /// <seealso cref="Mesnet.Classes.IO.Reporter.Report" />
    public class OperatingSystemReport : Report
    {
        public OperatingSystemReport()
        {
            _type = Global.ReportType.OperatingSystemReport;
            CreateContent();
        }

        public void CreateContent()
        {
            _sb = new StringBuilder();

            StringWriter sw = new StringWriter(_sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartObject();
                writer.WritePropertyName("ReportType");
                writer.WriteValue((int)_type);
                writer.WritePropertyName("Operating System Name");
                writer.WriteValue(OSInfo.OperatingSystemName());
                writer.WritePropertyName("Operating System MesnetVersion");
                writer.WriteValue(OSInfo.OperatingSystemVersion());
                writer.WritePropertyName("Total Available RAM");
                writer.WriteValue(OSInfo.TotalAvailableRAM());
                writer.WritePropertyName(".Net Versions");
                writer.WriteValue(OSInfo.DotNetVersions());
                writer.WritePropertyName("Proccessor Names");
                writer.WriteValue(OSInfo.ProcessorName());
                writer.WriteEndObject();
            }
        }
    }
}
