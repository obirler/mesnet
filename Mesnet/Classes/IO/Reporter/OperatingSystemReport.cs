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
