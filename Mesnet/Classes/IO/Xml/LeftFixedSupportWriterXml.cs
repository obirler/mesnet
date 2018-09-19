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

namespace Mesnet.Classes.IO.Xml
{
    public class LeftFixedSupportWriterXml
    {
        public LeftFixedSupportWriterXml(System.Xml.XmlWriter writer, Xaml.User_Controls.LeftFixedSupport support)
        {
            _writer = writer;

            _support = support;
        }

        public void Write()
        {
            _writer.WriteStartElement("LeftFixedSupport");

            _writer.WriteStartElement("SupportProperties");

            _writer.WriteElementString("id", _support.Id.ToString());

            _writer.WriteElementString("supportid", _support.SupportId.ToString());

            _writer.WriteElementString("name", _support.Name.ToString());

            _writer.WriteElementString("angle", _support.Angle.ToString());

            _writer.WriteElementString("leftposition", _support.LeftPos.ToString());

            _writer.WriteElementString("topposition", _support.TopPos.ToString());

            _writer.WriteEndElement();

            writemember();

            _writer.WriteEndElement();
        }

        private void writemember()
        {
            _writer.WriteStartElement("Member");

            _writer.WriteElementString("id", _support.Member.Beam.Id.ToString());

            _writer.WriteElementString("beamid", _support.Member.Beam.BeamId.ToString());

            _writer.WriteElementString("name", _support.Member.Beam.Name.ToString());

            switch (_support.Member.Direction)
            {
                case Global.Direction.Left:

                    _writer.WriteElementString("direction", "Left");

                    break;

                case Global.Direction.Right:

                    _writer.WriteElementString("direction", "Right");

                    break;
            }

            _writer.WriteEndElement();
        }

        System.Xml.XmlWriter _writer;

        Xaml.User_Controls.LeftFixedSupport _support;
    }
}
