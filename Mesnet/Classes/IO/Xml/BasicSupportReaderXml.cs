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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.IO.Xml
{
    public class BasicSupportReaderXml
    {
        public BasicSupportReaderXml(System.Xml.Linq.XElement supportelement)
        {
            _supportelement = supportelement;
            _support = new SupportManifest();
        }

        public SupportManifest Read()
        {
            _support.Type = "BasicSupport";

            readproperties();

            readmembers();

            return _support;
        }

        private void readproperties()
        {
            var propelement = _supportelement.Elements().Where(x => x.Name == "SupportProperties").First();

            foreach (var item in propelement.Elements())
            {
                switch (item.Name.ToString())
                {
                    case "id":
                        _support.Id = Convert.ToInt32(item.Value);
                        break;
                    case "supportid":
                        _support.SupportId = Convert.ToInt32(item.Value);
                        break;
                    case "name":
                        _support.Name = item.Value;
                        break;
                    case "angle":
                        _support.Angle = Convert.ToDouble(item.Value);
                        break;
                    case "leftposition":
                        _support.LeftPosition = Convert.ToDouble(item.Value);
                        break;
                    case "topposition":
                        _support.TopPosition = Convert.ToDouble(item.Value);
                        break;
                }
            }
        }

        private void readmembers()
        {
            var memberselement = _supportelement.Elements().Where(x => x.Name == "Members").First();

            var members = new List<Member>();

            foreach (var item in memberselement.Elements())
            {
                var member = new Member();
                foreach (var memberitem in item.Elements())
                {
                    switch (memberitem.Name.ToString())
                    {
                        case "id":
                            member.Id = Convert.ToInt32(memberitem.Value);
                            break;
                        case "beamid":
                            member.BeamId = Convert.ToInt32(memberitem.Value);
                            break;
                        case "name":
                            member.Name = memberitem.Value;
                            break;
                        case "direction":

                            if(memberitem.Value== "Left")
                            {
                                member.Direction = Global.Direction.Left;
                            }
                            else if(memberitem.Value == "Right")
                            {
                                member.Direction = Global.Direction.Right;
                            }
                            break;
                    }
                }
                members.Add(member);
            }

            if(members.Count > 0)
            {
                _support.Members = members;
            }
        }

        System.Xml.Linq.XElement _supportelement;

        SupportManifest _support;
    }
}