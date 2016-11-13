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
    public partial class BasicSupport : UserControl
    {
        public BasicSupport(Canvas canvas)
        {
            InitializeComponent();
            _canbedragged = true;
            Members = new List<Member>();
            SupportCount++;
            _name = "Basic Support " + SupportCount;
            canvas.Children.Add(this);
            _id = AddObject(this);
        }

        private int _id;

        private string _name;

        private bool _canbedragged;

        private bool _selected = false;

        private double _totalstiffness;

        private int _crossindex;

        public List<Member> Members;

        #region Methods

        public void Add(Canvas canvas, double x, double y)
        {
            canvas.Children.Add(this);
            _id = AddObject(this);

            Canvas.SetLeft(this, x);

            Canvas.SetTop(this, y);
        }

        public void Add(Canvas canvas, Point point)
        {
            canvas.Children.Add(this);
            _id = AddObject(this);
            var x = point.X;
            var y = point.Y;

            Canvas.SetLeft(this, x - 13);

            Canvas.SetTop(this, y);
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
                MyDebug.WriteWarning("SlidingSupport : Addbeam", "the beam is already added!");
            }
        }

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
            }
        }

        public void Select()
        {
            p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            p1.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        private bool IsSelected()
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

                MyDebug.WriteInformation("BasicSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("BasicSupport", "The beam to be dragged can not be dragged");
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

                MyDebug.WriteWarning("BasicSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("BasicSupport", "The beam to be dragged can not be dragged");
            }
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
        }

        /*
        /// <summary>
        /// Refreshes the member list.
        /// </summary>
        private void ReloadMembersCollection()
        {
            foreach (var member in Members)
            {
                var beam = member.Key.Key;
                var direction = member.Key.Value;

                switch (direction)
                {
                    case Direction.Left:

                        Members[new KeyValuePair<Beam, Direction>(beam, direction)] = beam.LeftEndMoment;

                        break;

                    case Direction.Right:

                        Members[new KeyValuePair<Beam, Direction>(beam, direction)] = beam.RightEndMoment;

                        break;
                }
            }
        }
        */

        #region Cross

        private void AddCrossMoment(double moment)
        {

        }

        /// <summary>
        /// Seperates moment in the support to the beams that are bounded and call Conduct function of each beam.
        /// </summary>
        /// <returns>True if the conduct moment is lower than the threshold otherwise false</returns>
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
                        if (Math.Abs(beammoment * beam.CarryOverAB) < 0.00001)
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

            //ReloadMembersCollection();

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
