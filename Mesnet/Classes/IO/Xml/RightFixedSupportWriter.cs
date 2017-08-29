using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.IO.Xml
{
    public class RightFixedSupportWriter
    {
        public RightFixedSupportWriter(System.Xml.XmlWriter writer, Xaml.User_Controls.RightFixedSupport support)
        {
            _writer = writer;

            _support = support;
        }

        public void Write()
        {
            _writer.WriteStartElement("RightFixedSupport");

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

        Xaml.User_Controls.RightFixedSupport _support;
    }
}
