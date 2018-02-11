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
    /// Interaction logic for BasicSupport.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class BasicSupport : UserControl
    {
        public BasicSupport(Canvas canvas)
        {
            InitializeComponent();
            _canbedragged = true;
            Members = new List<Member>();
            SupportCount++;
            _supportid = SupportCount;
            _name = "Basic Support " + SupportCount;
            canvas.Children.Add(this);
            _id = AddObject(this);
            BindEvents();
        }

        public BasicSupport()
        {
            InitializeComponent();
            _canbedragged = true;
            Members = new List<Member>();
            BindEvents();
        }

        private int _id;

        private int _supportid;

        private string _name;

        private double _angle;

        private bool _canbedragged;

        private bool _selected = false;

        private double _totalstiffness;

        private int _crossindex = -1;

        public List<Member> Members;

        #region Methods

        private void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            core.MouseDown += mw.BasicSupportMouseDown;
            core.MouseUp += mw.BasicSupportMouseUp;
        }

        /// <summary>
        /// Adds basic support in the specified canvas. Used in xml read.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="leftpos">The leftpos.</param>
        /// <param name="toppos">The toppos.</param>
        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);

            Canvas.SetLeft(this, leftpos);

            Canvas.SetTop(this, toppos);
        }

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

                            //beam.IsBound = true;

                            break;

                        case Direction.Right:

                            Canvas.SetLeft(this, beam.RightPoint.X - 13);

                            Canvas.SetTop(this, beam.RightPoint.Y);

                            beam.RightSide = this;

                            //beam.IsBound = true;

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

                            beam.IsBound = true;

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            break;
                    }
                }
            }
            else
            {
                MyDebug.WriteWarning("the beam is already added!");
            }
        }

        public void RemoveBeam(Beam beam)
        {
            Member remove= new Member();
            foreach (var member in Members)
            {
                if (member.Beam.Equals(beam))
                {
                    remove = member;
                    break;
                }
            }
            Members.Remove(remove);
        }

        public void SetBeam(Beam beam, Direction direction)
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

                            beam.LeftSide = this;

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Left:

                            beam.LeftSide = this;

                            beam.IsBound = true;

                            break;

                        case Direction.Right:

                            beam.RightSide = this;

                            beam.IsBound = true;

                            break;
                    }
                }
            }
            else
            {
                MyDebug.WriteWarning("the beam is already added!");
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

        public void Select()
        {
            triangle.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            triangle.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
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

                MyDebug.WriteInformation("Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("The beam to be dragged can not be dragged");
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

                MyDebug.WriteWarning("Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("The beam to be dragged can not be dragged");
            }
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
            _angle = angle;
        }

        #region Cross

        /// <summary>
        /// Seperates moment in the support to the beams that are bounded and call Conduct function of each beam.
        /// </summary>
        /// <returns>True if the conduct moment is lower than the threshold otherwise false</returns>
        public bool Seperate()
        {
            #region code

            bool isstop = false;

            MyDebug.WriteInformation("Started");

            if (Members.Count == 1)
            {
                MyDebug.WriteInformation("This support is an end support, returning");
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
                        MyDebug.WriteInformation("Left Moment = " + beam.LeftEndMoment);
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
                        MyDebug.WriteInformation("Right Moment = " + beam.RightEndMoment);
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

        public void ResetSolution()
        {
            _totalstiffness = 0;
            _crossindex = -1;
        }

        #endregion

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
            get { return Objects.IndexOf(this); }
            set { _id = value; }
        }

        public int SupportId
        {
            get { return _supportid; }
            set { _supportid = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double LeftPos
        {
            get { return Canvas.GetLeft(this); }
        }

        public double TopPos
        {
            get { return Canvas.GetTop(this); }
        }

        public double Angle
        {
            get { return _angle; }
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
