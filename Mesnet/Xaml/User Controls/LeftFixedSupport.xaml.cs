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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.Tools;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LeftFixedSupport.xaml
    /// </summary>
    public partial class LeftFixedSupport : UserControl
    {
        public LeftFixedSupport(Canvas canvas)
        {
            InitializeComponent();
            _canbedragged = true;
            canvas.Children.Add(this);
            _id = AddObject(this);
            SupportCount++;
            _supportid = SupportCount;
            _name = "Left Fixed Support " + SupportCount;
            MesnetDebug.WriteInformation(_name + " has been added to canvas");
            BindEvents();
        }

        public LeftFixedSupport()
        {
            InitializeComponent();
            _canbedragged = true;
            BindEvents();
        }

        private int _id;

        private int _supportid;

        private string _name;

        private double _angle;

        private bool _canbedragged;

        private bool _selected = false;

        public Member Member;

        private int _crossindex = -1;

        private void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            core.MouseDown += mw.LeftFixedSupportMouseDown;
            core.MouseUp += mw.LeftFixedSupportMouseUp;
        }

        public void Add(Canvas canvas, double leftpos, double toppos)
        {
            canvas.Children.Add(this);
            Canvas.SetLeft(this, leftpos);
            Canvas.SetTop(this, toppos);
            MesnetDebug.WriteInformation(_name + " has been added to canvas");
        }

        public void AddBeam(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - 7);

            Canvas.SetTop(this, beam.LeftPoint.Y - 13);

            Member = new Member(beam, Direction.Left);

            beam.LeftSide = this;

            MesnetDebug.WriteInformation("Left side of " + beam.Name + " has been added to " + _name);

            SetAngle(beam.Angle);
        }

        /// <summary>
        /// Updates the position of the support according to the beam that is bounded.
        /// </summary>
        /// <param name="beam">The reference beam.</param>
        public void UpdatePosition(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - 7);

            Canvas.SetTop(this, beam.LeftPoint.Y - 13);

            SetAngle(beam.Angle);
        }

        public void Select()
        {
            p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p3.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p4.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p5.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p6.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            p1.Fill = new SolidColorBrush(Colors.Black);
            p2.Fill = new SolidColorBrush(Colors.Black);
            p3.Fill = new SolidColorBrush(Colors.Black);
            p4.Fill = new SolidColorBrush(Colors.Black);
            p5.Fill = new SolidColorBrush(Colors.Black);
            p6.Fill = new SolidColorBrush(Colors.Black);
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

                MesnetDebug.WriteInformation(_name + ": Position has been set : " + left + " : " + right);
            }
            else
            {
                MesnetDebug.WriteWarning("The beam to be dragged can not be dragged");
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

                MesnetDebug.WriteInformation(_name + ": Position has been set : " + left + " : " + right);
            }
            else
            {
                MesnetDebug.WriteWarning("The beam to be dragged can not be dragged");
            }
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
            _angle = angle;
        }

        public void ResetSolution()
        {
            _crossindex = -1;
        }

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int SupportId
        {
            get { return _supportid; }
            set { _supportid = value; }
        }

        public string AName
        {
            get { return _name; }
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

        public double MomentDifference
        {
            get { return Member.Moment; }
        }

        public int CrossIndex
        {
            get { return _crossindex; }
            set { _crossindex = value; }
        }
        #endregion
    }
}
