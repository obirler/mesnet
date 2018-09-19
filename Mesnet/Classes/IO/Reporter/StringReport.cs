using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mesnet.Classes.IO.Reporter
{
    /// <summary>
    /// A report used for only string messages.
    /// </summary>
    /// <seealso cref="Mesnet.Classes.IO.Reporter.Report" />
    public class StringReport : Report
    {
        public StringReport(string contentmessage)
        {
            _type = Global.ReportType.StringReport;
            _contentmessage = contentmessage;
            CreateContent();
        }

        private string _contentmessage;

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
                writer.WritePropertyName("StringMessage");
                writer.WriteValue(_contentmessage);
                writer.WriteEndObject();
            }
        }
    }
}
