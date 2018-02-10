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
using Mesnet.Classes.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Mesnet.Classes.IO.Xml
{
    public class BeamReader
    {
        public BeamReader(System.Xml.Linq.XElement beamelement)
        {           
            _beamelement = beamelement;
            _beam = new BeamManifest();
        }

        public BeamManifest Read()
        {
            readproperties();

            readinertias();

            readloads();

            if(_beam.PerformStressAnalysis)
            {
                readepolies();

                readdpolies();
            }

            readconnections();

            return _beam;
        }

        private void readproperties()
        {
            var propelement = _beamelement.Elements().Where(x => x.Name == "BeamProperties").First();

            foreach (var item in propelement.Elements())
            {
                switch(item.Name.ToString())
                {
                    case "length":
                        _beam.Length = Convert.ToDouble(item.Value);
                        break;
                    case "name":
                        _beam.Name = item.Value;
                        break;
                    case "id":
                        _beam.Id = Convert.ToInt32(item.Value);
                        break;
                    case "beamid":
                        _beam.BeamId = Convert.ToInt32(item.Value);
                        break;
                    case "izero":
                        _beam.IZero = Convert.ToDouble(item.Value);
                        break;
                    case "elasticity":
                        _beam.Elasticity = Convert.ToDouble(item.Value);
                        break;
                    case "RotateTransform":
                        _beam.CenterX = Convert.ToDouble(item.Element("centerx").Value);
                        _beam.CenterY= Convert.ToDouble(item.Element("centery").Value);
                        _beam.Angle = Convert.ToDouble(item.Element("angle").Value);
                        break;
                    case "TransformGeometry":
                        var tl = item.Element("topleft").Value.Split(';');
                        _beam.TopLeft = new Point(Convert.ToDouble(tl[0]), Convert.ToDouble(tl[1]));
                        var tr = item.Element("topright").Value.Split(';');
                        _beam.TopRight = new Point(Convert.ToDouble(tr[0]), Convert.ToDouble(tr[1]));
                        var bl = item.Element("bottomleft").Value.Split(';');
                        _beam.BottomLeft = new Point(Convert.ToDouble(bl[0]), Convert.ToDouble(bl[1]));
                        var br = item.Element("bottomright").Value.Split(';');
                        _beam.BottomRight = new Point(Convert.ToDouble(br[0]), Convert.ToDouble(br[1]));
                        break;
                    case "leftposition":
                        _beam.LeftPosition = Convert.ToDouble(item.Value);
                        break;
                    case "topposition":
                        _beam.TopPosition = Convert.ToDouble(item.Value);
                        break;
                    case "performstressanalysis":
                        _beam.PerformStressAnalysis = Convert.ToBoolean(item.Value);
                        break;
                    case "maxallowablestress":
                        _beam.MaxAllowableStress = Convert.ToDouble(item.Value);
                        break;
                }
            }
        }

        private void readinertias()
        {
            var inertiaelement = _beamelement.Elements().Where(x => x.Name == "Inertias").First();

            var inertiappoly = new PiecewisePoly();

            foreach (var item in inertiaelement.Elements())
            {
                var poly = new Poly();

                foreach(var polyitem in item.Elements())
                {
                    switch(polyitem.Name.ToString())
                    {
                        case "expression":
                            poly.Parse(polyitem.Value);
                            break;
                        case "startpoint":
                            poly.StartPoint = Convert.ToDouble(polyitem.Value);
                            break;
                        case "endpoint":
                            poly.EndPoint = Convert.ToDouble(polyitem.Value);
                            break;
                    }
                }

                inertiappoly.Add(poly);
            }
            _beam.Inertias = inertiappoly;
        }

        private void readloads()
        {
            var loadelement = _beamelement.Elements().Where(x => x.Name == "Loads").First();        

            foreach (var item in loadelement.Elements())
            {
                switch (item.Name.ToString())
                {
                    case "DistributedLoads":

                        var distppoly = new PiecewisePoly();

                        foreach (var distitem in item.Elements())
                        {
                            var distpoly = new Poly();

                            foreach (var distpolyitem in distitem.Elements())
                            {
                                switch (distpolyitem.Name.ToString())
                                {
                                    case "expression":
                                        distpoly.Parse(distpolyitem.Value);
                                        break;
                                    case "startpoint":
                                        distpoly.StartPoint = Convert.ToDouble(distpolyitem.Value);
                                        break;
                                    case "endpoint":
                                        distpoly.EndPoint = Convert.ToDouble(distpolyitem.Value);
                                        break;
                                }
                            }
                            distppoly.Add(distpoly);
                        }

                        if(distppoly.Count > 0)
                        {
                            _beam.DistributedLoads = distppoly;
                        }

                        break;

                    case "ConcentratedLoads":

                        var concloads = new KeyValueCollection();

                        double magnitude = 0;

                        double location = 0;

                        foreach (var concitem in item.Elements())
                        {                          
                            foreach (var concpolyitem in concitem.Elements())
                            {
                                switch (concpolyitem.Name.ToString())
                                {
                                    case "location":
                                        location = Convert.ToDouble(concpolyitem.Value);
                                        break;
                                    case "magnitude":
                                        magnitude = Convert.ToDouble(concpolyitem.Value);
                                        break;
                                }
                            }
                            concloads.Add(location, magnitude);
                        }

                        if(concloads.Count > 0)
                        {
                            _beam.ConcentratedLoads = concloads;
                        }

                        break;
                }
            }
        }

        private void readepolies()
        {
            var epolyelement = _beamelement.Elements().Where(x => x.Name == "EPolies").First();

            var eppoly = new PiecewisePoly();

            foreach (var eitem in epolyelement.Elements())
            {
                var epoly = new Poly();

                foreach (var epolyitem in eitem.Elements())
                {
                    switch (epolyitem.Name.ToString())
                    {
                        case "expression":
                            epoly.Parse(epolyitem.Value);
                            break;
                        case "startpoint":
                            epoly.StartPoint = Convert.ToDouble(epolyitem.Value);
                            break;
                        case "endpoint":
                            epoly.EndPoint = Convert.ToDouble(epolyitem.Value);
                            break;
                    }
                }
                eppoly.Add(epoly);
            }

            if (eppoly.Count > 0)
            {
                _beam.EPolies = eppoly;
            }
        }

        private void readdpolies()
        {
            var dpolyelement = _beamelement.Elements().Where(x => x.Name == "DPolies").First();

            var dppoly = new PiecewisePoly();

            foreach (var ditem in dpolyelement.Elements())
            {
                var dpoly = new Poly();

                foreach (var dpolyitem in ditem.Elements())
                {
                    switch (dpolyitem.Name.ToString())
                    {
                        case "expression":
                            dpoly.Parse(dpolyitem.Value);
                            break;
                        case "startpoint":
                            dpoly.StartPoint = Convert.ToDouble(dpolyitem.Value);
                            break;
                        case "endpoint":
                            dpoly.EndPoint = Convert.ToDouble(dpolyitem.Value);
                            break;
                    }
                }
                dppoly.Add(dpoly);
            }

            if (dppoly.Count > 0)
            {
                _beam.DPolies = dppoly;
            }
        }

        private void readconnections()
        {
            var conectionelement = _beamelement.Elements().Where(x => x.Name == "Connections").First();

            var connections = new Connections();

            connections.LeftSide = null;

            connections.RightSide = null;

            foreach (var sideitem in conectionelement.Elements())
            {
                switch (sideitem.Name.ToString())
                {
                    case "LeftSide":

                        var left = new LeftSide();

                        foreach (var leftitem in sideitem.Elements())
                        {
                            switch (leftitem.Name.ToString())
                            {
                                case "type":
                                    left.Type = leftitem.Value;
                                    break;
                                case "id":
                                    left.Id = Convert.ToInt32(leftitem.Value);
                                    break;
                                case "supportid":
                                    left.SupportId = Convert.ToInt32(leftitem.Value);
                                    break;
                            }
                        }
                        connections.LeftSide = left;

                        break;
                    case "RightSide":

                        var right = new RightSide();

                        foreach (var rightitem in sideitem.Elements())
                        {
                            switch (rightitem.Name.ToString())
                            {
                                case "type":
                                    right.Type = rightitem.Value;
                                    break;
                                case "id":
                                    right.Id = Convert.ToInt32(rightitem.Value);
                                    break;
                                case "supportid":
                                    right.SupportId = Convert.ToInt32(rightitem.Value);
                                    break;
                            }
                        }
                        connections.RightSide = right;
                        break;
                }
            }

            _beam.Connections = connections;
        }

        System.Xml.Linq.XElement _beamelement;

        BeamManifest _beam;
    }
}