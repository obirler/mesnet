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
using Mesnet.Xaml.User_Controls;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Mesnet.Classes.IO.Json
{
    public static class MesnetIOJson
    {
        public static string GetCurrentDrawingAsJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;

                writer.WriteStartObject();

                writer.WritePropertyName("Objects");

                writer.WriteStartObject();

                foreach (var item in Global.Objects)
                {
                    switch (Global.GetObjectType(item.Value))
                    {
                        case Global.ObjectType.Beam:

                            var beam = item.Value as Beam;

                            var beamwriter = new BeamWriterJson(writer, beam);

                            beamwriter.Write();

                            break;

                        case Global.ObjectType.BasicSupport:

                            var bs = item.Value as BasicSupport;

                            var bswriter = new BasicSupportWriterJson(writer, bs);

                            bswriter.Write();

                            break;

                        case Global.ObjectType.LeftFixedSupport:

                            var ls = item.Value as LeftFixedSupport;

                            var lswiter = new LeftFixedSupportWriterJson(writer, ls);

                            lswiter.Write();

                            break;

                        case Global.ObjectType.RightFixedSupport:

                            var rs = item.Value as RightFixedSupport;

                            var rswriter = new RightFixedSupportWriterJson(writer, rs);

                            rswriter.Write();

                            break;
                    }
                }

                writer.WriteEndObject();

                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public static string GetDebugLogsAsJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartObject();
                writer.WritePropertyName("Debug Logs");
                writer.WriteStartArray();                
                foreach (var log in Global.LogList)
                {
                    writer.WriteValue(log);
                }
                writer.WriteEnd();
                writer.WriteEndObject();
            }
            return sb.ToString();
        }

        public static string GetFileLogsAsJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartObject();
                writer.WritePropertyName("File Logs");
                writer.WriteStartArray();
                foreach (var log in Global.FileLogList)
                {
                    writer.WriteValue(log);
                }
                writer.WriteEnd();
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }
}
