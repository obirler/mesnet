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

using System;
using System.Linq;

namespace Mesnet.Classes.IO.Manifest
{
    public class RightFixedSupportReader
    {
        public RightFixedSupportReader(System.Xml.Linq.XElement supportelement)
        {
            _supportelement = supportelement;
            _support = new RightFixedSupportManifest();
        }

        public RightFixedSupportManifest Read()
        {
            readproperties();

            readmember();

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

        private void readmember()
        {
            var memberelement = _supportelement.Elements().Where(x => x.Name == "Member").First();

            var member = new Member();

            foreach (var memberitem in memberelement.Elements())
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

                        if (memberitem.Value == "Left")
                        {
                            member.Direction = Global.Direction.Left;
                        }
                        else if (memberitem.Value == "Right")
                        {
                            member.Direction = Global.Direction.Right;
                        }
                        break;
                }
            }

            _support.Member = member;
        }

        System.Xml.Linq.XElement _supportelement;

        RightFixedSupportManifest _support;
    }
}
