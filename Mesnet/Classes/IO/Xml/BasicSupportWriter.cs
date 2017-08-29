using Mesnet.Classes.Tools;

namespace Mesnet.Classes.IO.Xml
{
    public class BasicSupportWriter
    {
        public BasicSupportWriter(System.Xml.XmlWriter writer, Xaml.User_Controls.BasicSupport support)
        {
            _writer = writer;

            _support = support;
        }

        public void Write()
        {
            _writer.WriteStartElement("BasicSupport");

            _writer.WriteStartElement("SupportProperties");

            _writer.WriteElementString("id", _support.Id.ToString());

            _writer.WriteElementString("supportid", _support.SupportId.ToString());

            _writer.WriteElementString("name", _support.Name.ToString());

            _writer.WriteElementString("angle", _support.Angle.ToString());

            _writer.WriteElementString("leftposition", _support.LeftPos.ToString());

            _writer.WriteElementString("topposition", _support.TopPos.ToString());

            _writer.WriteEndElement();

            writemembers();

            _writer.WriteEndElement();
        }

        private void writemembers()
        {
            _writer.WriteStartElement("Members");

            foreach(Member member in _support.Members)
            {
                _writer.WriteStartElement("Member");

                _writer.WriteElementString("id", member.Beam.Id.ToString());

                _writer.WriteElementString("beamid", member.Beam.BeamId.ToString());

                _writer.WriteElementString("name", member.Beam.Name.ToString());

                switch(member.Direction)
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

            _writer.WriteEndElement();
        }

        System.Xml.XmlWriter _writer;

        Xaml.User_Controls.BasicSupport _support;
    }
}
