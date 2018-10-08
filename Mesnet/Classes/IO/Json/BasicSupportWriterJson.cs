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
