using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mesnet.Xaml.User_Controls;
using Newtonsoft.Json;

namespace Mesnet.Classes.IO.Json
{
    public class LeftFixedSupportWriterJson
    {
        public LeftFixedSupportWriterJson(JsonWriter writer, LeftFixedSupport support)
        {
            _writer = writer;

            _support = support;
        }

        public void Write()
        {
            _writer.WritePropertyName("LeftFixedSupport");

            _writer.WriteStartObject();

            _writer.WritePropertyName("SupportProperties");

            _writer.WriteStartObject();

            _writer.WritePropertyName("id");
            _writer.WriteValue(_support.Id.ToString());

            _writer.WritePropertyName("supportid");
            _writer.WriteValue(_support.SupportId.ToString());

            _writer.WritePropertyName("name");
            _writer.WriteValue(_support.Name);

            _writer.WritePropertyName("angle");
            _writer.WriteValue(_support.Angle.ToString());

            _writer.WritePropertyName("leftposition");
            _writer.WriteValue(_support.LeftPos.ToString());

            _writer.WritePropertyName("topposition");
            _writer.WriteValue(_support.TopPos.ToString());

            _writer.WriteEndObject();

            writemember();

            _writer.WriteEndObject();
        }

        private void writemember()
        {
            _writer.WritePropertyName("Member");

            _writer.WriteStartObject();

            _writer.WritePropertyName("id");
            _writer.WriteValue(_support.Member.Beam.Id.ToString());

            _writer.WritePropertyName("beamid");
            _writer.WriteValue(_support.Member.Beam.BeamId.ToString());

            _writer.WritePropertyName("name");
            _writer.WriteValue(_support.Member.Beam.Name);

            switch (_support.Member.Direction)
            {
                case Global.Direction.Left:

                    _writer.WritePropertyName("direction");
                    _writer.WriteValue("Left");

                    break;

                case Global.Direction.Right:

                    _writer.WritePropertyName("direction");
                    _writer.WriteValue("Right");

                    break;
            }

            _writer.WriteEndObject();
        }

        private JsonWriter _writer;

        private LeftFixedSupport _support;
    }
}
