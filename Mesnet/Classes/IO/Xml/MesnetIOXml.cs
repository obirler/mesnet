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

using Mesnet.Classes.IO.Manifest;
using Mesnet.Xaml.User_Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using static Mesnet.Classes.Global;

namespace Mesnet.Classes.IO.Xml
{
    public class MesnetIOXml
    {
        public MesnetIOXml()
        {           
        }

        private MainWindow _mw;

        public void WriteXml(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Objects");

                foreach (var item in Objects)
                {
                    switch (GetObjectType(item.Value))
                    {
                        case ObjectType.Beam:

                            var beam = item.Value as Beam;

                            var beamwriter = new BeamWriterXml(writer, beam);

                            beamwriter.Write();

                            break;

                        case ObjectType.BasicSupport:

                            var bs = item.Value as BasicSupport;

                            var bswriter = new BasicSupportWriterXml(writer, bs);

                            bswriter.Write();

                            break;

                        case ObjectType.LeftFixedSupport:

                            var ls = item.Value as LeftFixedSupport;

                            var lswiter = new LeftFixedSupportWriterXml(writer, ls);

                            lswiter.Write();

                            break;

                        case ObjectType.RightFixedSupport:

                            var rs = item.Value as RightFixedSupport;

                            var rswriter = new RightFixedSupportWriterXml(writer, rs);

                            rswriter.Write();

                            break;
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public bool ReadXml(Canvas canvas, string path, MainWindow mw)
        {
            _mw = mw;
            _canvas = canvas;
            manifestlist = new List<object>();

            XDocument doc;
            try
            {
                doc = XDocument.Load(path);
            }
            catch (Exception)
            {
                MessageBox.Show(GetString("invalidfile"));
                _mw.Notify();
                return false;
            }
            
            foreach (XElement element in doc.Element("Objects").Elements())
            {
                switch (element.Name.ToString())
                {
                    case "Beam":
                        var beamreader = new BeamReaderXml(element);
                        manifestlist.Add(beamreader.Read());
                        break;

                    case "BasicSupport":
                        var bsreader = new BasicSupportReaderXml(element);
                        manifestlist.Add(bsreader.Read());
                        break;

                    case "SlidingSupport":
                        var ssreader = new BasicSupportReaderXml(element);
                        manifestlist.Add(ssreader.Read());
                        break;

                    case "LeftFixedSupport":
                        var lsreader = new LeftFixedSupportReaderXml(element);
                        manifestlist.Add(lsreader.Read());
                        break;

                    case "RightFixedSupport":
                        var rsreader = new RightFixedSupportReader(element);
                        manifestlist.Add(rsreader.Read());
                        break;
                }
            }
            setmaxvalues();
            
            Global.Objects.Clear();
            addtocanvas();
            connectobjects();
            setvariables();
            _mw.UpToolBar().UpdateLoadDiagrams();
            return true;
        }

        public string GetCurrentXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Objects");

                foreach (var item in Objects)
                {
                    switch (GetObjectType(item.Value))
                    {
                        case ObjectType.Beam:

                            var beam = item.Value as Beam;

                            var beamwriter = new BeamWriterXml(writer, beam);

                            beamwriter.Write();

                            break;

                        case ObjectType.BasicSupport:

                            var bs = item.Value as BasicSupport;

                            var bswriter = new BasicSupportWriterXml(writer, bs);

                            bswriter.Write();

                            break;

                        case ObjectType.LeftFixedSupport:

                            var ls = item.Value as LeftFixedSupport;

                            var lswiter = new LeftFixedSupportWriterXml(writer, ls);

                            lswiter.Write();

                            break;

                        case ObjectType.RightFixedSupport:

                            var rs = item.Value as RightFixedSupport;

                            var rswriter = new RightFixedSupportWriterXml(writer, rs);

                            rswriter.Write();

                            break;
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return builder.ToString();
        }

        public string GetLogXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {                
                writer.WriteStartDocument();
                writer.WriteStartElement("Logs");              
                for (int i = 0; i < LogList.Count; i++)
                {
                    writer.WriteElementString("Log", LogList[i]);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return builder.ToString();
        }

        public string GetFileLogXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("FileLogs");
                for (int i = 0; i < FileLogList.Count; i++)
                {
                    writer.WriteElementString("FileLogs", FileLogList[i]);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return builder.ToString();
        }

        private void addtocanvas()
        {
            foreach(object item in manifestlist)
            {
                if (item is BeamManifest beammanifest)
                {
                    addbeam(beammanifest);
                }
                else if(item is SupportManifest bsupportmanifest)
                {
                    addsupport(bsupportmanifest);
                }
                else if(item is LeftFixedSupportManifest lssupportmanifest)
                {
                    addleftfixedsupport(lssupportmanifest);
                }
                else if (item is RightFixedSupportManifest rssupportmanifest)
                {
                    addrightfixedsupport(rssupportmanifest);
                }
            }
            _canvas.UpdateLayout();
        }

        private void addbeam(BeamManifest beammanifest)
        {
            var beam = new Beam();
            beam.AddFromXml(_canvas, beammanifest.Length);
            beam.Id = beammanifest.Id;
            beam.Name = beammanifest.Name;
            beam.BeamId = beammanifest.BeamId;
            beam.SetTopLeft(beammanifest.TopPosition, beammanifest.LeftPosition);           
            beam.RotateTransform.CenterX = beammanifest.CenterX;
            beam.RotateTransform.CenterY = beammanifest.CenterY;
            beam.RotateTransform.Angle = beammanifest.Angle;
            beam.Angle = beam.RotateTransform.Angle;
            beam.SetTransformGeometry(beammanifest.TopLeft, beammanifest.TopRight, beammanifest.BottomRight, beammanifest.BottomLeft, _canvas);
            beam.IZero = beammanifest.IZero;
            beam.AddElasticity(beammanifest.Elasticity);
            beam.PerformStressAnalysis = beammanifest.PerformStressAnalysis;
            if(beam.PerformStressAnalysis)
            {
                beam.MaxAllowableStress = beammanifest.MaxAllowableStress;
            }
            beam.AddInertia(beammanifest.Inertias);
            if(beammanifest.DistributedLoads != null)
            {
                if(beammanifest.DistributedLoads.Count > 0)
                {
                    beam.AddLoad(beammanifest.DistributedLoads);
                }               
            }
            if (beammanifest.ConcentratedLoads != null)
            {
                if (beammanifest.ConcentratedLoads.Count > 0)
                {
                    beam.AddLoad(beammanifest.ConcentratedLoads);
                }                
            }
            if(beammanifest.EPolies != null)
            {
                if(beammanifest.EPolies.Count >0)
                {
                    beam.AddE(beammanifest.EPolies);
                }
            }
            if (beammanifest.DPolies != null)
            {
                if (beammanifest.DPolies.Count > 0)
                {
                    beam.AddD(beammanifest.DPolies);
                }
            }
            Objects.Add(beam.Id, beam);
        }

        private void addsupport(SupportManifest supportmanifest)
        {
            switch(supportmanifest.Type)
            {
                case "BasicSupport":

                    var bs = new BasicSupport();
                    bs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
                    bs.Id = supportmanifest.Id;
                    bs.SupportId = supportmanifest.SupportId;
                    bs.Name = supportmanifest.Name;
                    bs.SetAngle(supportmanifest.Angle);
                    Objects.Add(bs.Id, bs);

                    break;

                case "SlidingSupport":

                    var ss = new SlidingSupport();
                    ss.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
                    ss.Id = supportmanifest.Id;
                    ss.SupportId = supportmanifest.SupportId;
                    ss.Name = supportmanifest.Name;
                    ss.SetAngle(supportmanifest.Angle);
                    Objects.Add(ss.Id, ss);

                    break;
            }
        }

        private void addleftfixedsupport(LeftFixedSupportManifest supportmanifest)
        {
            var ls = new LeftFixedSupport();
            ls.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            ls.Id = supportmanifest.Id;
            ls.SupportId = supportmanifest.SupportId;
            ls.Name = supportmanifest.Name;
            ls.SetAngle(supportmanifest.Angle);
            Objects.Add(ls.Id, ls);
        }

        private void addrightfixedsupport(RightFixedSupportManifest supportmanifest)
        {
            var rs = new RightFixedSupport();
            rs.Add(_canvas, supportmanifest.LeftPosition, supportmanifest.TopPosition);
            rs.Id = supportmanifest.Id;
            rs.SupportId = supportmanifest.SupportId;
            rs.Name = supportmanifest.Name;
            rs.SetAngle(supportmanifest.Angle);
            Objects.Add(rs.Id, rs);
        }

        private void connectobjects()
        {
            foreach(var item in Objects)
            {
                switch (GetObjectType(item.Value))
                {
                    case ObjectType.Beam:
                        connectbeam(item.Value as Beam);
                        break;

                    case ObjectType.BasicSupport:
                        connectbasicsupport(item.Value as BasicSupport);
                        break;

                    case ObjectType.SlidingSupport:
                        connectslidingsupport(item.Value as SlidingSupport);
                        break;

                    case ObjectType.LeftFixedSupport:
                        connectleftfixedsupport(item.Value as LeftFixedSupport);
                        break;

                    case ObjectType.RightFixedSupport:
                        connectrightfixedsupport(item.Value as RightFixedSupport);
                        break;
                }
            }
        }

        private void connectbeam(Beam beam)
        {
            foreach (object manifestitem in manifestlist)
            {
                switch (manifestitem.GetType().Name)
                {
                    case "BeamManifest":
                        var beammanifest = manifestitem as BeamManifest;
                        if (beam.Id == beammanifest.Id)
                        {
                            if (beammanifest.Connections.LeftSide != null)
                            {
                                beam.LeftSide = GetObject(beammanifest.Connections.LeftSide.Id);
                            }
                            else
                            {
                                beam.LeftSide = null;
                            }

                            if (beammanifest.Connections.RightSide != null)
                            {
                                beam.RightSide = GetObject(beammanifest.Connections.RightSide.Id);
                            }
                            else
                            {
                                beam.RightSide = null;
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectbasicsupport(BasicSupport support)
        {
            SupportManifest manifest;
            foreach (object manifestitem in manifestlist)
            {
                switch (manifestitem.GetType().Name)
                {
                    case "SupportManifest":
                        manifest = manifestitem as SupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            foreach(Member item in manifest.Members)
                            {
                                var member = new Mesnet.Classes.Tools.Member();
                                member.Beam = GetObject(item.Id) as Beam;
                                member.Direction = item.Direction;
                                support.Members.Add(member);
                            }
                            
                            if (support.Members.Count > 1)
                            {
                                foreach (var member in support.Members)
                                {
                                   member.Beam.IsBound = true;
                                }
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectslidingsupport(SlidingSupport support)
        {
            SupportManifest manifest;
            foreach (object manifestitem in manifestlist)
            {
                switch (manifestitem.GetType().Name)
                {
                    case "SupportManifest":
                        manifest = manifestitem as SupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            foreach (Member item in manifest.Members)
                            {
                                var member = new Mesnet.Classes.Tools.Member();
                                member.Beam = GetObject(item.Id) as Beam;
                                member.Direction = item.Direction;
                                support.Members.Add(member);
                            }
                            if (support.Members.Count > 1)
                            {
                                foreach (var member in support.Members)
                                {
                                    member.Beam.IsBound = true;
                                }
                            }
                            return;
                        }
                        break;
                }
            }
        }

        private void connectleftfixedsupport(LeftFixedSupport support)
        {
            LeftFixedSupportManifest manifest;
            foreach (object manifestitem in manifestlist)
            {
                switch (manifestitem.GetType().Name)
                {
                    case "LeftFixedSupportManifest":
                        manifest = manifestitem as LeftFixedSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            var member = new Mesnet.Classes.Tools.Member();
                            member.Beam = GetObject(manifest.Member.Id) as Beam;
                            member.Direction = manifest.Member.Direction;
                            support.Member = member;
                            return;
                        }
                        break;
                }
            }
        }

        private void connectrightfixedsupport(RightFixedSupport support)
        {
            RightFixedSupportManifest manifest;
            foreach (object manifestitem in manifestlist)
            {
                switch (manifestitem.GetType().Name)
                {
                    case "RightFixedSupportManifest":
                        manifest = manifestitem as RightFixedSupportManifest;
                        if (support.Id == manifest.Id)
                        {
                            var member = new Mesnet.Classes.Tools.Member();
                            member.Beam = GetObject(manifest.Member.Id) as Beam;
                            member.Direction = manifest.Member.Direction;
                            support.Member = member;
                            return;
                        }
                        break;
                }
            }
        }

        private void setvariables()
        {
            BeamCount = 0;
            SupportCount = 0;
            foreach (var item in Objects)
            {
                switch (GetObjectType(item.Value))
                {
                    case ObjectType.Beam:
                        BeamCount++;
                        break;

                    case ObjectType.BasicSupport:
                        SupportCount++;
                        break;

                    case ObjectType.SlidingSupport:
                        SupportCount++;
                        break;

                    case ObjectType.LeftFixedSupport:
                        SupportCount++;
                        break;

                    case ObjectType.RightFixedSupport:
                        SupportCount++;
                        break;
                }
            }
        }

        private void setmaxvalues()
        {
            MaxInertia = Double.MinValue;
            MaxMoment = Double.MinValue;
            MaxConcLoad = Double.MinValue;
            MaxDistLoad = Double.MinValue;

            for (int i = 0; i < manifestlist.Count; i++)
            {
                switch (manifestlist[i].GetType().Name)
                {
                    case "BeamManifest":

                        var beammanifest = manifestlist[i] as BeamManifest;
                        if (beammanifest.Inertias.MaxAbs > MaxInertia)
                        {
                            MaxInertia = beammanifest.Inertias.MaxAbs;
                        }
                        if (beammanifest.DistributedLoads?.Count > 0)
                        {
                            if (beammanifest.DistributedLoads.MaxAbs > MaxDistLoad)
                            {
                                MaxDistLoad = beammanifest.DistributedLoads.MaxAbs;
                            }
                        }
                        if (beammanifest.ConcentratedLoads?.Count > 0)
                        {
                            if (beammanifest.ConcentratedLoads.YMaxAbs > MaxConcLoad)
                            {
                                MaxConcLoad = beammanifest.ConcentratedLoads.YMaxAbs;
                            }
                        }

                        break;
                }
            }
        }

        private Canvas _canvas;

        private List<object> manifestlist;       
    }
}
