using System.Collections.Generic;

namespace Mesnet.Classes.IO.Xml
{
    public class BeamWriter
    {
        public BeamWriter(System.Xml.XmlWriter writer, Xaml.User_Controls.Beam beam)
        {
            _writer = writer;

            _beam = beam;                
        }

        public void Write()
        {                
            _writer.WriteStartElement("Beam");

            _writer.WriteStartElement("BeamProperties");

            _writer.WriteElementString("length", _beam.Length.ToString());

            _writer.WriteElementString("name", _beam.Name.ToString());

            _writer.WriteElementString("id", _beam.Id.ToString());

            _writer.WriteElementString("beamid", _beam.BeamId.ToString());

            _writer.WriteElementString("izero", _beam.IZero.ToString());

            _writer.WriteElementString("elasticity", _beam.ElasticityModulus.ToString());

            _writer.WriteElementString("leftposition", _beam.LeftPos.ToString());

            _writer.WriteElementString("topposition", _beam.TopPos.ToString());

            _writer.WriteElementString("performstressanalysis", _beam.PerformStressAnalysis.ToString());

            if (_beam.PerformStressAnalysis)
            {
                _writer.WriteElementString("maxallowablestress", _beam.MaxAllowableStress.ToString());
            }

            _writer.WriteStartElement("RotateTronsform");

            _writer.WriteElementString("centerx", _beam.rotateTransform.CenterX.ToString());

            _writer.WriteElementString("centery", _beam.rotateTransform.CenterY.ToString());

            _writer.WriteElementString("angle", _beam.rotateTransform.Angle.ToString());

            _writer.WriteEndElement();

            _writer.WriteEndElement();

            writeinertias();

            writeloads();

            if(_beam.PerformStressAnalysis)
            {
                writestresproperties();
            }

            writeconnections();
        
            _writer.WriteEndElement();
        }

        private void writeinertias()
        {
            _writer.WriteStartElement("Inertias");

            foreach(Math.Poly poly in _beam.Inertias)
            {
                _writer.WriteStartElement("Inertia");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void writeloads()
        {
            _writer.WriteStartElement("Loads");

            if(_beam.ConcentratedLoads.Count > 0)
            {
                _writer.WriteStartElement("ConcentratedLoads");
                foreach (KeyValuePair<double, double> pair in _beam.ConcentratedLoads)
                {
                    _writer.WriteStartElement("ConcentratedLoad");
                    _writer.WriteElementString("magnitude", pair.Value.ToString());
                    _writer.WriteElementString("location", pair.Key.ToString());
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }

            if(_beam.DistributedLoads.Count > 0)
            {
                _writer.WriteStartElement("DistributedLoads");
                foreach (Math.Poly poly in _beam.DistributedLoads)
                {
                    _writer.WriteStartElement("DistributedLoad");
                    _writer.WriteElementString("expression", poly.ToString());
                    _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                    _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }
          
            _writer.WriteEndElement();
        }

        private void writestresproperties()
        {
            _writer.WriteStartElement("EPolies");

            foreach (Math.Poly poly in _beam.E)
            {
                _writer.WriteStartElement("EPoly");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();

            _writer.WriteStartElement("DPolies");

            foreach (Math.Poly poly in _beam.D)
            {
                _writer.WriteStartElement("DPoly");
                _writer.WriteElementString("expression", poly.ToString());
                _writer.WriteElementString("startpoint", poly.StartPoint.ToString());
                _writer.WriteElementString("endpoint", poly.EndPoint.ToString());
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void writeconnections()
        {
            _writer.WriteStartElement("Connections");

            if (_beam.LeftSide != null)
            {
                _writer.WriteStartElement("LeftSide");

                switch (Global.GetObjectType(_beam.LeftSide))
                {
                    case Global.ObjectType.BasicSupport:
                        var bs = _beam.LeftSide as Xaml.User_Controls.BasicSupport;
                        _writer.WriteElementString("type", "BasicSupport");
                        _writer.WriteElementString("id", bs.Id.ToString());
                        _writer.WriteElementString("supportid", bs.SupportId.ToString());
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = _beam.LeftSide as Xaml.User_Controls.SlidingSupport;
                        _writer.WriteElementString("type", "SlidingSupport");
                        _writer.WriteElementString("id", ss.Id.ToString());
                        _writer.WriteElementString("supportid", ss.SupportId.ToString());
                        break;

                    case Global.ObjectType.LeftFixedSupport:
                        var ls = _beam.LeftSide as Xaml.User_Controls.LeftFixedSupport;
                        _writer.WriteElementString("type", "LeftFixedSupport");
                        _writer.WriteElementString("id", ls.Id.ToString());
                        _writer.WriteElementString("supportid", ls.SupportId.ToString());
                        break;
                }

                _writer.WriteEndElement();
            }

            if (_beam.RightSide != null)
            {
                _writer.WriteStartElement("RightSide");

                switch (Global.GetObjectType(_beam.RightSide))
                {
                    case Global.ObjectType.BasicSupport:
                        var bs = _beam.RightSide as Xaml.User_Controls.BasicSupport;
                        _writer.WriteElementString("type", "BasicSupport");
                        _writer.WriteElementString("id", bs.Id.ToString());
                        _writer.WriteElementString("supportid", bs.SupportId.ToString());
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = _beam.RightSide as Xaml.User_Controls.SlidingSupport;
                        _writer.WriteElementString("type", "SlidingSupport");
                        _writer.WriteElementString("id", ss.Id.ToString());
                        _writer.WriteElementString("supportid", ss.SupportId.ToString());
                        break;

                    case Global.ObjectType.RightFixedSupport:
                        var rs = _beam.RightSide as Xaml.User_Controls.RightFixedSupport;
                        _writer.WriteElementString("type", "RightFixedSupport");
                        _writer.WriteElementString("id", rs.Id.ToString());
                        _writer.WriteElementString("supportid", rs.SupportId.ToString());
                        break;
                }

                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        System.Xml.XmlWriter _writer;

        Xaml.User_Controls.Beam _beam;
    }   
}
