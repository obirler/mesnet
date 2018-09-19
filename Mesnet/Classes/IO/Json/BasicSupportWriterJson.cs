using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using Newtonsoft.Json;

namespace Mesnet.Classes.IO.Json
{
    public class BasicSupportWriterJson
    {
        public BasicSupportWriterJson(JsonWriter writer, BasicSupport support)
        {
            _writer = writer;

            _support = support;
        }

        public void Write()
        {
            _writer.WritePropertyName("BasicSupport");

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

            writemembers();

            _writer.WriteEndObject();
        }

        private void writemembers()
        {
            _writer.WritePropertyName("Members");

            _writer.WriteStartObject();


            foreach (Member member in _support.Members)
            {
                _writer.WritePropertyName("Member");

                _writer.WriteStartObject();

                _writer.WritePropertyName("id");
                _writer.WriteValue(member.Beam.Id.ToString());                

                _writer.WritePropertyName("beamid");
                _writer.WriteValue(member.Beam.BeamId.ToString());

                _writer.WritePropertyName("name");
                _writer.WriteValue(member.Beam.Name);

                switch (member.Direction)
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

            _writer.WriteEndObject();
        }

        JsonWriter _writer;

        private BasicSupport _support;
    }
}
