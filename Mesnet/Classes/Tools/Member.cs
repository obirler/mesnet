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
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Classes.Tools
{
    public class Member
    {
        public Member()
        {
            
        }

        public Member(Beam beam, Direction direction)
        {
            _beam = beam;
            _direction = direction;
        }

        private Beam _beam;

        private Direction _direction;

        public Beam Beam
        {
            get { return _beam; }
            set { _beam = value; }
        }

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public double Moment
        {
            get
            {
                double moment = 0;
                switch (_direction)
                {
                    case Direction.Left:

                        moment = _beam.LeftEndMoment;

                        break;

                    case Direction.Right:

                        moment = _beam.RightEndMoment;

                        break;
                }

                return moment;
            }
        }
    }
}
