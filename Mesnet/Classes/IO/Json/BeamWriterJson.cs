using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mesnet.Xaml.User_Controls;
using Newtonsoft.Json;

namespace Mesnet.Classes.IO.Json
{
    public class BeamWriterJson
    {
        public BeamWriterJson(JsonWriter writer, Beam beam)
        {
            _writer = writer;
            _beam = beam;
        }

        public void Write()
        {
            _writer.WritePropertyName("Beam");

            _writer.WriteStartObject();

            _writer.WritePropertyName("BeamProperties");

            _writer.WriteStartObject();

            _writer.WritePropertyName("length");
            _writer.WriteValue(_beam.Length.ToString());

            _writer.WritePropertyName("name");
            _writer.WriteValue(_beam.Name);

            _writer.WritePropertyName("id");
            _writer.WriteValue(_beam.Id.ToString());

            _writer.WritePropertyName("beamid");
            _writer.WriteValue(_beam.BeamId.ToString());

            _writer.WritePropertyName("izero");
            _writer.WriteValue(_beam.IZero.ToString());

            _writer.WritePropertyName("elasticity");
            _writer.WriteValue(_beam.ElasticityModulus.ToString());

            _writer.WritePropertyName("leftposition");
            _writer.WriteValue(_beam.LeftPos.ToString());

            _writer.WritePropertyName("topposition");
            _writer.WriteValue(_beam.TopPos.ToString());

            _writer.WritePropertyName("performstressanalysis");
            _writer.WriteValue(_beam.PerformStressAnalysis.ToString());

            if (_beam.PerformStressAnalysis)
            {
                _writer.WritePropertyName("maxallowablestress");
                _writer.WriteValue(_beam.MaxAllowableStress.ToString());
            }

            _writer.WritePropertyName("RotateTransform");

            _writer.WriteStartObject();

            _writer.WritePropertyName("centerx");
            _writer.WriteValue(_beam.rotateTransform.CenterX.ToString());

            _writer.WritePropertyName("centery");
            _writer.WriteValue(_beam.rotateTransform.CenterY.ToString());

            _writer.WritePropertyName("angle");
            _writer.WriteValue(_beam.rotateTransform.Angle.ToString());

            _writer.WriteEndObject();

            _writer.WritePropertyName("TransformGeometry");

            _writer.WriteStartObject();

            _writer.WritePropertyName("topleft");
            _writer.WriteValue(_beam.TGeometry.InnerTopLeft.X + ";" + _beam.TGeometry.InnerTopLeft.Y);

            _writer.WritePropertyName("topright");
            _writer.WriteValue(_beam.TGeometry.InnerTopRight.X + ";" + _beam.TGeometry.InnerTopRight.Y);

            _writer.WritePropertyName("bottomleft");
            _writer.WriteValue(_beam.TGeometry.InnerBottomLeft.X + ";" + _beam.TGeometry.InnerBottomLeft.Y);

            _writer.WritePropertyName("bottomright");
            _writer.WriteValue(_beam.TGeometry.InnerBottomRight.X + ";" + _beam.TGeometry.InnerBottomRight.Y);

            _writer.WriteEndObject();

            _writer.WriteEndObject();

            writeinertias();

            writeloads();

            if (_beam.PerformStressAnalysis)
            {
                writestresproperties();
            }

            writeconnections();

            _writer.WriteEndObject();
        }

        private void writeinertias()
        {
            _writer.WritePropertyName("Inertias");

            _writer.WriteStartObject();

            foreach (Math.Poly poly in _beam.Inertias)
            {
                _writer.WritePropertyName("Inertia");

                _writer.WriteStartObject();

                _writer.WritePropertyName("expression");
                _writer.WriteValue(poly.ToString());

                _writer.WritePropertyName("startpoint");
                _writer.WriteValue(poly.StartPoint.ToString());

                _writer.WritePropertyName("endpoint");
                _writer.WriteValue(poly.EndPoint.ToString());

                _writer.WriteEndObject();
            }

            _writer.WriteEndObject();
        }

        private void writeloads()
        {
            _writer.WritePropertyName("Loads");

            _writer.WriteStartObject();

            if (_beam.ConcentratedLoads?.Count > 0)
            {
                _writer.WritePropertyName("ConcentratedLoads");

                _writer.WriteStartObject();

                foreach (KeyValuePair<double, double> pair in _beam.ConcentratedLoads)
                {
                    _writer.WritePropertyName("ConcentratedLoad");

                    _writer.WriteStartObject();

                    _writer.WritePropertyName("magnitude");
                    _writer.WriteValue(pair.Value.ToString());

                    _writer.WritePropertyName("location");
                    _writer.WriteValue(pair.Key.ToString());

                    _writer.WriteEndObject();
                }
                _writer.WriteEndObject();
            }

            if (_beam.DistributedLoads?.Count > 0)
            {
                _writer.WritePropertyName("DistributedLoads");

                _writer.WriteStartObject();

                foreach (Math.Poly poly in _beam.DistributedLoads)
                {
                    _writer.WritePropertyName("DistributedLoad");

                    _writer.WriteStartObject();

                    _writer.WritePropertyName("expression");
                    _writer.WriteValue(poly.ToString());

                    _writer.WritePropertyName("startpoint");
                    _writer.WriteValue(poly.StartPoint.ToString());

                    _writer.WritePropertyName("endpoint");
                    _writer.WriteValue(poly.EndPoint.ToString());

                    _writer.WriteEndObject();
                }

                _writer.WriteEndObject();
            }

            _writer.WriteEndObject();
        }

        private void writestresproperties()
        {
            _writer.WritePropertyName("EPolies");

            _writer.WriteStartObject();

            foreach (Math.Poly poly in _beam.E)
            {
                _writer.WritePropertyName("EPoly");

                _writer.WriteStartObject();

                _writer.WritePropertyName("expression");
                _writer.WriteValue(poly.ToString());

                _writer.WritePropertyName("startpoint");
                _writer.WriteValue(poly.StartPoint.ToString());

                _writer.WritePropertyName("endpoint");
                _writer.WriteValue(poly.EndPoint.ToString());

                _writer.WriteEndObject();
            }

            _writer.WriteEndObject();

            _writer.WritePropertyName("DPolies");

            _writer.WriteStartObject();

            foreach (Math.Poly poly in _beam.D)
            {
                _writer.WritePropertyName("DPoly");

                _writer.WriteStartObject();

                _writer.WritePropertyName("expression");
                _writer.WriteValue(poly.ToString());

                _writer.WritePropertyName("startpoint");
                _writer.WriteValue(poly.StartPoint.ToString());

                _writer.WritePropertyName("endpoint");
                _writer.WriteValue(poly.EndPoint.ToString());

                _writer.WriteEndObject();
            }

            _writer.WriteEndObject();
        }

        private void writeconnections()
        {
            _writer.WritePropertyName("Connections");

            _writer.WriteStartObject();

            if (_beam.LeftSide != null)
            {
                _writer.WritePropertyName("LeftSide");

                _writer.WriteStartObject();

                switch (Global.GetObjectType(_beam.LeftSide))
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = _beam.LeftSide as Xaml.User_Controls.BasicSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("BasicSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(bs.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(bs.SupportId.ToString());

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = _beam.LeftSide as Xaml.User_Controls.SlidingSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("SlidingSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(ss.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(ss.SupportId.ToString());

                        break;

                    case Global.ObjectType.LeftFixedSupport:

                        var ls = _beam.LeftSide as Xaml.User_Controls.LeftFixedSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("LeftFixedSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(ls.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(ls.SupportId.ToString());

                        break;
                }

                _writer.WriteEndObject();
            }

            if (_beam.RightSide != null)
            {
                _writer.WritePropertyName("RightSide");

                _writer.WriteStartObject();

                switch (Global.GetObjectType(_beam.RightSide))
                {
                    case Global.ObjectType.BasicSupport:

                        var bs = _beam.RightSide as Xaml.User_Controls.BasicSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("BasicSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(bs.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(bs.SupportId.ToString());

                        break;

                    case Global.ObjectType.SlidingSupport:

                        var ss = _beam.RightSide as Xaml.User_Controls.SlidingSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("SlidingSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(ss.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(ss.SupportId.ToString());

                        break;

                    case Global.ObjectType.RightFixedSupport:

                        var rs = _beam.RightSide as Xaml.User_Controls.RightFixedSupport;

                        _writer.WritePropertyName("type");
                        _writer.WriteValue("RightFixedSupport");

                        _writer.WritePropertyName("id");
                        _writer.WriteValue(rs.Id.ToString());

                        _writer.WritePropertyName("supportid");
                        _writer.WriteValue(rs.SupportId.ToString());

                        break;
                }

                _writer.WriteEndObject();
            }

            _writer.WriteEndObject();
        }

        JsonWriter _writer;

        Xaml.User_Controls.Beam _beam;
    }
}
