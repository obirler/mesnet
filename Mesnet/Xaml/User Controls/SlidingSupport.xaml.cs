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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.Tools;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for SlidingSupport.xaml
    /// </summary>
    public partial class SlidingSupport : UserControl
    {
        public SlidingSupport(Canvas canvas)
        {
            InitializeComponent();
            _canbedragged = true;
            Members = new List<Member>();
            SupportCount++;
            _supportid = SupportCount;
            _name = "Sliding Support " + SupportCount;
            canvas.Children.Add(this);
            _id = AddObject(this);
        }

        private int _id;

        private int _supportid;

        private string _name;

        private bool _selected;

        private double _totalstiffness;

        private bool _canbedragged;

        private int _crossindex;

        public List<Member> Members;

        public void AddBeam(Beam beam, Direction direction)
        {
            var member = new Member(beam, direction);
            if (!Members.Contains(member))
            {
                Members.Add(member);

                if (Members.Count == 1)
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - 13);

                            Canvas.SetTop(this, beam.LeftPoint.Y);

                            beam.LeftSide = this;

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - 13);

                            Canvas.SetTop(this, beam.RightPoint.Y);

                            beam.RightSide = this;

                            break;
                    }

                    SetAngle(beam.Angle);
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            beam.LeftSide = this;

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            break;
                    }
                }
            }
            else
            {
                MyDebug.WriteWarning("SlidingSupport : Addbeam", "the beam is already added!");
            }
        }

        /// <summary>
        /// Updates the position of the support according to the beam that is bounded.
        /// </summary>
        /// <param name="beam">The reference beam.</param>
        public void UpdatePosition(Beam beam)
        {
            foreach (Member member in Members)
            {
                if (member.Beam == beam)
                {
                    switch (member.Direction)
                    {
                        case Direction.Left:

                            Canvas.SetLeft(this, beam.LeftPoint.X - 13);

                            Canvas.SetTop(this, beam.LeftPoint.Y);

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - 13);

                            Canvas.SetTop(this, beam.RightPoint.Y);

                            break;
                    }
                    SetAngle(beam.Angle);
                }
            }
        }

        /// <summary>
        /// Updates the position of the support when the bounded beam's position has changes.
        /// </summary>
        public void UpdatePosition()
        {
            if (Members.Count == 1)
            {
                switch (Members[0].Direction)
                {
                    case Direction.Left:

                        Canvas.SetLeft(this, Members[0].Beam.LeftPoint.X - 13);

                        Canvas.SetTop(this, Members[0].Beam.LeftPoint.Y);

                        break;

                    case Direction.Right:

                        Canvas.SetLeft(this, Members[0].Beam.RightPoint.X - 13);

                        Canvas.SetTop(this, Members[0].Beam.RightPoint.Y);

                        break;
                }

                SetAngle(Members[0].Beam.Angle);
            }
        }

        public void Select()
        {
            _selected = true;
            p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            e1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            e2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
        }

        public void UnSelect()
        {
            _selected = false;
            p1.Fill = new SolidColorBrush(Colors.Black);
            e1.Fill = new SolidColorBrush(Colors.Black);
            e2.Fill = new SolidColorBrush(Colors.Black);
        }

        public bool IsSelected()
        {
            return _selected;
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetPosition(double x, double y)
        {
            if (_canbedragged)
            {
                var left = x - Width / 2;
                var right = y - Height / 2;

                Canvas.SetLeft(this, left);

                Canvas.SetTop(this, right);

                MyDebug.WriteInformation("SlidingSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("SlidingSupport", "The beam to be dragged can not be dragged");
            }
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetPosition(Point point)
        {
            if (_canbedragged)
            {
                var left = point.X - Width / 2;
                var right = point.Y - Height / 2;

                Canvas.SetLeft(this, left);

                Canvas.SetTop(this, right);

                MyDebug.WriteWarning("SlidingSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("SlidingSupport", "The beam to be dragged can not be dragged");
            }
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
        }

        #region Cross

        public bool Seperate()
        {
            #region code

            bool isstop = false;

            MyDebug.WriteInformation(this.Name + " : Seperate", "Started");

            if (Members.Count == 1)
            {
                MyDebug.WriteInformation(this.Name + " : Seperate", "This support is an end support, returning");
                return true;
            }

            double momenttoadd = -MomentDifference;

            foreach (Member member in Members)
            {
                var beam = member.Beam;
                var direction = member.Direction;
                double coeff = 0;
                double beammoment = 0;

                switch (direction)
                {
                    case Direction.Left:

                        //The right side of the beam is connected to this support
                        coeff = beam.StiffnessA / _totalstiffness;
                        beammoment = coeff * momenttoadd;
                        beam.LeftEndMoment += beammoment;
                        MyDebug.WriteInformation(this.Name + " : Seperate", "Left Moment = " + beam.LeftEndMoment);
                        Logger.WriteLine(this.Name + " : " + beammoment + " will be conducted to " + beam.Name);
                        beam.Conduct(Direction.Left, beammoment);
                        if (Math.Abs(beammoment * beam.CarryOverAB) < CrossLoopTreshold)
                        {
                            isstop = true;
                        }
                        else
                        {
                            isstop = false;
                        }

                        break;

                    case Direction.Right:

                        //The right side of the beam is connected to this support
                        coeff = beam.StiffnessB / _totalstiffness;
                        beammoment = coeff * momenttoadd;
                        beam.RightEndMoment += beammoment;
                        MyDebug.WriteInformation(this.Name + " : Seperate", "Right Moment = " + beam.RightEndMoment);
                        Logger.WriteLine(this.Name + " : " + beammoment + " will be conducted to " + beam.Name);
                        beam.Conduct(Direction.Right, beammoment);
                        if (Math.Abs(beammoment * beam.CarryOverBA) < 0.00001)
                        {
                            isstop = true;
                        }
                        else
                        {
                            isstop = false;
                        }

                        break;
                }
                Logger.NextLine();
            }

            return isstop;

            #endregion
        }

        public void CalculateTotalStiffness()
        {
            _totalstiffness = 0;
            foreach (Member member in Members)
            {
                var beam = member.Beam;
                var direction = member.Direction;

                switch (direction)
                {
                    case Direction.Left:

                        _totalstiffness += beam.StiffnessA;

                        break;

                    case Direction.Right:

                        _totalstiffness += beam.StiffnessB;

                        break;
                }
            }

            if (Members.Count > 1)
            {
                foreach (Member member in Members)
                {
                    switch (member.Direction)
                    {
                        case Direction.Left:

                            Logger.WriteLine(this.Name + " : " + member.Beam.Name + " stiffness = " + member.Beam.StiffnessA / _totalstiffness);

                            break;

                        case Direction.Right:

                            Logger.WriteLine(this.Name + " : " + member.Beam.Name + " stiffness = " + member.Beam.StiffnessB / _totalstiffness);

                            break;
                    }

                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the beam can be dragged in the canvas.
        /// </summary>
        /// <value>
        /// <c>true</c> if the beam can be dragged; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeDragged
        {
            get
            {
                return _canbedragged;
            }
            set
            {
                _canbedragged = value;
            }
        }

        public int Id
        {
            get { return _id; }
        }

        public int SupportId
        {
            get { return _supportid; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double MomentDifference
        {
            get
            {
                double sum = 0;

                foreach (Member member in Members)
                {
                    sum += member.Moment;
                }

                return sum;
            }
        }

        public double TotalStiffness
        {
            get { return _totalstiffness; }
        }

        public int CrossIndex
        {
            get { return _crossindex; }
            set { _crossindex = value; }
        }
        #endregion
    }
}
