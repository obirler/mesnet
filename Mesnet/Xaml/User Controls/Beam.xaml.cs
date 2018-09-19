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
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Mesnet.Classes;
using Mesnet.Classes.Math;
using Mesnet.Classes.Tools;
using MoreLinq;
using static Mesnet.Classes.Global;
using static Mesnet.Classes.Math.Algebra;
using Geometry = Mesnet.Classes.Math.Geometry;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for NewBeam.xaml
    /// </summary>
    public partial class Beam : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Beam"/> class without length. It is used in xml reading purposes.
        /// </summary>
        public Beam()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Beam"/> with specified length.
        /// </summary>
        /// <param name="length">The length in meters.</param>
        /// 
        public Beam(double length)
        {
            InitializeComponent();

            InitializeVariables(length);
        }

        public Beam(Canvas canvas, double length)
        {
            _canvas = canvas;

            InitializeComponent();

            InitializeVariables(length);

            AddTopLeft(_canvas, 10000, 10000);
        }

        private void InitializeVariables(double length)
        {
            _length = length;
            contentgrid.Width = length * 100;
            Width = contentgrid.Width;
            contentgrid.Height = 14;
            Height = contentgrid.Height;
            MesnetDebug.WriteInformation("A has been created : length = " + _length + " m, Width = " + Width + " Height = " + Height);

            RightSide = null;

            LeftSide = null;

            BindEvents();
        }

        private void BindEvents()
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            core.MouseDown += mw.BeamCoreMouseDown;
            core.MouseUp += mw.BeamCoreMouseUp;
            core.MouseMove += mw.BeamCoreMouseMove;
            startcircle.MouseDown += mw.StartCircleMouseDown;
            endcircle.MouseDown += mw.EndCircleMouseDown;
        }

        #region internal variables

        private int _id;

        private int _beamid;

        private bool selected;

        private double _length;

        private string _name = null;

        private double _izero;

        private double _elasticity;

        private bool _canbedragged = true;

        private double _leftpos;

        private double _toppos;

        KeyValueCollection _concentratedloads;

        private PiecewisePoly _distributedloads;

        private PiecewisePoly _loads;

        private PiecewisePoly _inertiappoly;

        private PiecewisePoly _zeroforceconc;

        private PiecewisePoly _zeroforcedist;

        /// <summary>
        /// The force when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeroforce;

        /// <summary>
        /// The moment when there is no fixed support for right side of the clapeyron equation.
        /// </summary>
        private PiecewisePoly _zeromoment;

        private PiecewisePoly _fixedendmoment;

        private PiecewisePoly _fixedendforce;

        private KeyValueCollection _stress;

        private PiecewisePoly _e;

        private PiecewisePoly _d;

        public object LeftSide;

        public object RightSide;

        public double LeftDistributionFactor;

        public double RightDistributionFactor;

        public Direction circledirection;

        private Canvas _canvas;

        private Point corepoint;

        private ConcentratedLoad _concload;

        private DistributedLoad _distload;

        private Force _force;

        private Moment _moment;

        private Force _feforce;

        private Moment _femoment;

        private Inertia _inertia;

        private Deflection _deflectiondigram;

        private Stress _stressdiagram;

        private bool _directionshown = false;

        /// <summary>
        /// The left support force of the beam.
        /// </summary>
        private double _leftsupportforcedist;

        private double _leftsupportforceconc;

        /// <summary>
        /// The right support force of the beam.
        /// </summary>
        private double _rightsupportforcedist;

        private double _rightsupportforceconc;

        /// <summary>
        /// The resultant force that is the sum of the all acting force on beam.
        /// </summary>
        private double _resultantforce;

        /// <summary>
        /// The acting point of the resultant force.
        /// </summary>
        private double _resultantforcedistance;

        /// <summary>
        /// The left-end moment of the beam when the load is acting according to cross sign system.
        /// </summary>
        private double _ma;

        /// <summary>
        /// The left-end moment of the beam when the load is acting according to mohr sign system.
        /// </summary>
        private double _maclapeyron;

        /// <summary>
        /// The right-end moment of the beam when the load is acting according to cross sign system.
        /// </summary>
        private double _mb;

        /// <summary>
        /// The right-end moment of the beam when the load is acting according to mohr sign system.
        /// </summary>
        private double _mbclapeyron;

        private double _alfaa;

        private double _alfab;

        private double _beta;

        private double _ca;

        private double _cb;

        private double _fia;

        private double _fib;

        private double _gamaba;

        private double _gamaab;

        private double _ka;

        private double _kb;

        private double _angle;

        private double _maxmoment;

        private double _maxabsmoment;

        private double _minmoment;

        private double _maxforce;

        private double _maxabsforce;

        private double _minforce;

        private double _maxinertia;

        private double _maxstress;

        private double _maxabsstress;

        private double _maxconcload;

        private double _maxabsconcload;

        private double _maxdistload;

        private double _maxabsdistload;

        private TransformGeometry _tgeometry;

        private bool _leftcircleseleted;

        private bool _rightcircleselected;

        private bool _indexpassed;

        private bool _isbound = false;

        private bool _stressanalysis = false;

        private List<Func> _deflection;

        private Func _maxdeflection;

        private double _maxallowablestress;

        private bool _analyticalsolution = false;

        private double _stiffnessa;

        private double _stiffnessb;

        private double _newstiffnessa;

        private double _newstiffnessb;

        #endregion

        #region methods

        public void Add(Canvas canvas)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                _id = AddObject(this);
                BeamCount++;
                _beamid = BeamCount;
                _name = "Beam " + BeamCount;
                Canvas.SetZIndex(this, 1);
                MesnetDebug.WriteInformation(_name + " has been added to canvas : length = " + _length + " m, Width = " + Width + " Height = " + Height);
            }
            else
            {
                MesnetDebug.WriteWarning(_name + " : This beam has already been added to canvas!");
            }
        }

        /// <summary>
        /// A special add function that is used to add beam that is read from xml file.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void AddFromXml(Canvas canvas, double length)
        {
            _canvas = canvas;
            InitializeVariables(length);
            canvas.Children.Add(this);
            Canvas.SetLeft(this, _leftpos);
            Canvas.SetTop(this, _toppos);
            MesnetDebug.WriteInformation(_name + " has been added from xml to canvas : length = " + _length + " m, Width = " + Width + " Height = " + Height);
        }

        public void AddTopLeft(Canvas canvas, double x, double y)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                _id = AddObject(this);

                Canvas.SetLeft(this, x);

                if (Height > 0)
                {
                    Canvas.SetTop(this, y);
                }
                else
                {
                    Canvas.SetTop(this, y - 7);
                }

                BeamCount++;
                _beamid = BeamCount;
                _name = "Beam " + BeamCount;

                MesnetDebug.WriteInformation(_name + " has been added to canvas : length = " + _length + " m, Width = " + Width + " Height = " + Height);

                SetTransformGeometry(canvas);
                SetAngleCenter(0);
            }
            else
            {
                MesnetDebug.WriteWarning(_name + " : This beam has already been added to canvas!");
            }
        }

        public void AddCenter(Canvas canvas, double x, double y)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                _id = AddObject(this);

                Canvas.SetLeft(this, x - Width / 2);

                if (Height > 0)
                {
                    Canvas.SetTop(this, y - Height / 2);
                }
                else
                {
                    Canvas.SetTop(this, y - 7);
                }

                BeamCount++;
                _beamid = BeamCount;
                _name = "Beam " + BeamCount;
                MesnetDebug.WriteInformation(_name + " has been added to canvas : length = " + _length + " m, Width = " + Width + " Height = " + Height);

                SetTransformGeometry(canvas);
            }
            else
            {
                MesnetDebug.WriteWarning(_name + " This beam has already been added to canvas!");
            }
        }

        /// <summary>
        /// Sets the length of the Beam. It is used in xml reading purposes.
        /// </summary>
        /// <param name="length">The length of the beam.</param>
        public void SetLength(double length)
        {
            InitializeVariables(length);
        }

        /// <summary>
        /// Changes the length of the existing Beam.
        /// </summary>
        /// <param name="length">The desired length of the beam.</param>
        public void ChangeLength(double length)
        {
            double oldlength = _length;
            _length = length;
            contentgrid.Width = _length * 100;
            Width = contentgrid.Width;
            _tgeometry.ChangeWidth(Width);

            //If the length really is changed then the loads on the beam become meaningless (at least distributed loads), so remove them.
            if (Math.Abs(oldlength - length) > 0.00001)
            {
                RemoveConcentratedLoad();
                DestroyConcLoadDiagram();
                RemoveDistributedLoad();
                DestroyDistLoadDiagram();
            }
        }

        public void Remove()
        {
            Objects.Remove(_id);
            _canvas.Children.Remove(this);
        }

        /// <summary>
        /// Connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>        
        public void Connect(Direction direction1, Beam oldbeam, Direction direction2)
        {
            if (_isbound && oldbeam.IsBound)
            {
                throw new InvalidOperationException("Both beam has bound");
            }
            switch (direction1)
            {
                case Direction.Left:

                    switch (direction2)
                    {
                        #region Left-Left

                        case Direction.Left:

                            if (LeftSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftconnect(oldbeam);

                            break;

                        #endregion

                        #region Left-Right

                        case Direction.Right:

                            if (LeftSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the right side of lodbeam.
                            leftrightconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;

                case Direction.Right:

                    switch (direction2)
                    {
                        #region Right-Left

                        case Direction.Left:

                            if (RightSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftconnect(oldbeam);

                            break;

                        #endregion

                        #region Right-Right

                        case Direction.Right:

                            if (RightSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Right side of this beam will be connected to the right side of oldbeam.                             
                            rightrightconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;
            }

            _isbound = true;
            oldbeam.IsBound = true;
        }

        private void leftleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                if (GetObjectType(oldbeam.LeftSide) != ObjectType.LeftFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Left, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(oldbeam.LeftSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = oldbeam.LeftSide as SlidingSupport;
                            ss.AddBeam(this, Direction.Left);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = oldbeam.LeftSide as BasicSupport;
                            bs.AddBeam(this, Direction.Left);

                            break;

                        case ObjectType.RightFixedSupport:

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (LeftSide != null)
            {
                if (GetObjectType(LeftSide) != ObjectType.LeftFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        SetPosition(Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Left, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(LeftSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = LeftSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Direction.Left);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = LeftSide as BasicSupport;
                            bs.AddBeam(oldbeam, Direction.Left);

                            break;

                        case ObjectType.RightFixedSupport:

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void leftrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                if (GetObjectType(oldbeam.RightSide) != ObjectType.RightFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Right, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(oldbeam.RightSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = oldbeam.RightSide as SlidingSupport;
                            ss.AddBeam(this, Direction.Left);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = oldbeam.RightSide as BasicSupport;
                            bs.AddBeam(this, Direction.Left);

                            break;

                        case ObjectType.LeftFixedSupport:

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (LeftSide != null)
            {
                if (GetObjectType(LeftSide) != ObjectType.LeftFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Right, LeftPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Left, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(LeftSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = LeftSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Direction.Right);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = LeftSide as BasicSupport;
                            bs.AddBeam(oldbeam, Direction.Right);

                            break;

                        case ObjectType.RightFixedSupport:

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightleftconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                if (GetObjectType(LeftSide) != ObjectType.LeftFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Left, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(LeftSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = oldbeam.LeftSide as SlidingSupport;
                            ss.AddBeam(this, Direction.Right);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = oldbeam.LeftSide as BasicSupport;
                            bs.AddBeam(this, Direction.Right);

                            break;

                        case ObjectType.RightFixedSupport:

                            throw new InvalidOperationException(
                                "RightFixedSupport has been bounded to the left side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (RightSide != null)
            {
                if (GetObjectType(RightSide) != ObjectType.RightFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Left, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.LeftPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = RightSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Direction.Left);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = RightSide as BasicSupport;
                            bs.AddBeam(oldbeam, Direction.Left);

                            break;

                        case ObjectType.LeftFixedSupport:

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        private void rightrightconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                if (GetObjectType(RightSide) != ObjectType.RightFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Right, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = oldbeam.RightSide as SlidingSupport;
                            ss.AddBeam(this, Direction.Right);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = oldbeam.RightSide as BasicSupport;
                            bs.AddBeam(this, Direction.Right);

                            break;

                        case ObjectType.LeftFixedSupport:

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else if (RightSide != null)
            {
                if (GetObjectType(RightSide) != ObjectType.RightFixedSupport)
                {
                    if (oldbeam.IsBound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }
                    else if (this._isbound)
                    {
                        //We will move the old beam
                        oldbeam.SetPosition(Direction.Right, RightPoint);
                        oldbeam.MoveSupports();
                    }
                    else if (!oldbeam.IsBound && !this._isbound)
                    {
                        //We will move this beam
                        SetPosition(Direction.Right, oldbeam.RightPoint);
                        MoveSupports();
                    }

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.SlidingSupport:

                            var ss = RightSide as SlidingSupport;
                            ss.AddBeam(oldbeam, Direction.Right);

                            break;

                        case ObjectType.BasicSupport:

                            var bs = RightSide as BasicSupport;
                            bs.AddBeam(oldbeam, Direction.Right);

                            break;

                        case ObjectType.LeftFixedSupport:

                            throw new InvalidOperationException(
                                "LeftFixedSupport has been bounded to the right side of the beam");

                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "The side that has a fixed support can not be connected.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "In order to add beam to a beam, the beam that is supposed to connected must have a support.");
            }
        }

        /// <summary>
        /// Circular connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>
        /// <exception cref="InvalidOperationException">
        /// In order to create circular beam system both beam to be connected need to be bound
        /// or
        /// In order to create circular beam system one of the beam to be connected need to have support on connection side
        /// or
        /// In order to create circular beam system one of the beam to be connected need to have support on connection side
        /// or
        /// Both beam has supports on the assembly points
        /// or
        /// Both beam has supports on the assembly points
        /// or
        /// Both beam has supports on the assembly points
        /// </exception>
        public void CircularConnect(Direction direction1, Beam oldbeam, Direction direction2)
        {
            if (!_isbound || !oldbeam.IsBound)
            {
                throw new InvalidOperationException("In order to create circular beam system both beam to be connected need to be bound");
            }

            switch (direction1)
            {
                case Direction.Left:

                    switch (direction2)
                    {
                        #region Left-Left

                        case Direction.Left:

                            if (LeftSide == null && oldbeam.LeftSide == null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }
                            else if (LeftSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("In order to create circular beam system one of the beam to be connected need to have support on connection side");
                            }

                            //Left side of this beam will be connected to the left side of oldbeam.
                            leftleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Left-Right

                        case Direction.Right:

                            if (LeftSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Left side of this beam will be connected to the right side of lodbeam.
                            leftrightcircularconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;

                case Direction.Right:

                    switch (direction2)
                    {
                        #region Right-Left

                        case Direction.Left:

                            if (RightSide != null && oldbeam.LeftSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }
                            //Right side of this beam will be connected to the left side of oldbeam.
                            rightleftcircularconnect(oldbeam);

                            break;

                        #endregion

                        #region Right-Right

                        case Direction.Right:

                            if (RightSide != null && oldbeam.RightSide != null)
                            {
                                throw new InvalidOperationException("Both beam has supports on the assembly points");
                            }

                            //Right side of this beam will be connected to the right side of oldbeam.                             
                            rightrightcircularconnect(oldbeam);

                            break;

                            #endregion
                    }

                    break;
            }
        }

        private void leftleftcircularconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                switch (GetObjectType(LeftSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = oldbeam.LeftSide as SlidingSupport;
                        ss.AddBeam(this, Direction.Left);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = oldbeam.LeftSide as BasicSupport;
                        bs.AddBeam(this, Direction.Left);

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
            else if (LeftSide != null)
            {
                switch (GetObjectType(LeftSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = LeftSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Direction.Left);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = LeftSide as BasicSupport;
                        bs.AddBeam(oldbeam, Direction.Left);

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
        }

        private void leftrightcircularconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                switch (GetObjectType(RightSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = oldbeam.RightSide as SlidingSupport;
                        ss.AddBeam(this, Direction.Left);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = oldbeam.RightSide as BasicSupport;
                        bs.AddBeam(this, Direction.Left);

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
            else if (LeftSide != null)
            {
                switch (GetObjectType(LeftSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = LeftSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Direction.Right);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = LeftSide as BasicSupport;
                        bs.AddBeam(oldbeam, Direction.Right);

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
        }

        private void rightrightcircularconnect(Beam oldbeam)
        {
            if (oldbeam.RightSide != null)
            {
                switch (GetObjectType(RightSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = oldbeam.RightSide as SlidingSupport;
                        ss.AddBeam(this, Direction.Right);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = oldbeam.RightSide as BasicSupport;
                        bs.AddBeam(this, Direction.Right);

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
            else if (RightSide != null)
            {
                switch (GetObjectType(RightSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = RightSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Direction.Right);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = RightSide as BasicSupport;
                        bs.AddBeam(oldbeam, Direction.Right);

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
        }

        private void rightleftcircularconnect(Beam oldbeam)
        {
            if (oldbeam.LeftSide != null)
            {
                switch (GetObjectType(LeftSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = oldbeam.LeftSide as SlidingSupport;
                        ss.AddBeam(this, Direction.Right);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = oldbeam.LeftSide as BasicSupport;
                        bs.AddBeam(this, Direction.Right);

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "RightFixedSupport has been bounded to the left side of the beam");

                        break;
                }
            }
            else if (RightSide != null)
            {
                switch (GetObjectType(RightSide))
                {
                    case ObjectType.SlidingSupport:

                        var ss = RightSide as SlidingSupport;
                        ss.AddBeam(oldbeam, Direction.Left);

                        break;

                    case ObjectType.BasicSupport:

                        var bs = RightSide as BasicSupport;
                        bs.AddBeam(oldbeam, Direction.Left);

                        break;

                    case ObjectType.RightFixedSupport:

                        throw new InvalidOperationException(
                            "The side that has a fixed support can not be connected.");

                        break;

                    case ObjectType.LeftFixedSupport:

                        throw new InvalidOperationException(
                            "LeftFixedSupport has been bounded to the right side of the beam");

                        break;
                }
            }
        }

        private void leftreconnect()
        {
            Beam leftbeam = null;
            var direction = Direction.None;

            switch (GetObjectType(LeftSide))
            {
                case ObjectType.BasicSupport:

                    var bs = LeftSide as BasicSupport;
                    if (bs.Members.Count > 1)
                    {
                        foreach (Member member in bs.Members)
                        {
                            if (!member.Beam.Equals(this))
                            {
                                leftbeam = member.Beam;
                                direction = member.Direction;
                                break;
                            }
                        }
                    }

                    break;

                case ObjectType.SlidingSupport:

                    var ss = LeftSide as SlidingSupport;
                    if (ss.Members.Count > 1)
                    {
                        foreach (Member member in ss.Members)
                        {
                            if (!member.Beam.Equals(this))
                            {
                                leftbeam = member.Beam;
                                direction = member.Direction;
                                break;
                            }
                        }
                    }

                    break;
            }

            if (leftbeam != null)
            {
                switch (direction)
                {
                    case Direction.Left:
                        SetPosition(Direction.Left, leftbeam.LeftPoint);
                        break;

                    case Direction.Right:
                        SetPosition(Direction.Left, leftbeam.RightPoint);
                        break;
                }
                MoveSupports();
            }
        }

        private void rightreconnect()
        {
            Beam rightbeam = null;
            var direction = Direction.None;

            switch (GetObjectType(RightSide))
            {
                case ObjectType.BasicSupport:

                    var bs = RightSide as BasicSupport;
                    if (bs.Members.Count > 1)
                    {
                        foreach (Member member in bs.Members)
                        {
                            if (!member.Beam.Equals(this))
                            {
                                rightbeam = member.Beam;
                                direction = member.Direction;
                                break;
                            }
                        }
                    }

                    break;

                case ObjectType.SlidingSupport:

                    var ss = RightSide as SlidingSupport;
                    if (ss.Members.Count > 1)
                    {
                        foreach (Member member in ss.Members)
                        {
                            if (!member.Beam.Equals(this))
                            {
                                rightbeam = member.Beam;
                                direction = member.Direction;
                                break;
                            }
                        }
                    }

                    break;
            }

            if (rightbeam != null)
            {
                switch (direction)
                {
                    case Direction.Left:
                        SetPosition(Direction.Right, rightbeam.LeftPoint);
                        break;

                    case Direction.Right:
                        SetPosition(Direction.Right, rightbeam.RightPoint);
                        break;
                }
                MoveSupports();
            }
        }

        /// <summary>
        /// Adds the distributed load to beam with specified direction.
        /// </summary>
        /// <param name="loadppoly">The desired distributed load piecewise polynomial.</param>
        public void AddLoad(PiecewisePoly loadppoly)
        {
            _distributedloads = loadppoly;
            _maxdistload = _distributedloads.Max;
            _maxabsdistload = _distributedloads.MaxAbs;
        }

        /// <summary>
        /// Adds the concentrated load to beam with specified direction.
        /// </summary>
        /// <param name="load">The desired list of concentrated load key value pair.</param>
        public void AddLoad(KeyValueCollection loadpairs)
        {
            _concentratedloads = loadpairs;
            _maxconcload = _concentratedloads.YMax;
            _maxabsconcload = _concentratedloads.YMaxAbs;
        }

        public void RemoveDistributedLoad()
        {
            DistributedLoad distload = null;
            foreach (var load in upcanvas.Children)
            {
                switch (load.GetType().Name)
                {
                    case "DistributedLoad":

                        distload = load as DistributedLoad;

                        break;
                }
            }

            if (distload != null)
            {
                distload.RemoveLabels();
                upcanvas.Children.Remove(distload);
                _distributedloads = null;
                _maxdistload = Double.MinValue;
                _distload = null;
            }
        }

        public void RemoveConcentratedLoad()
        {
            ConcentratedLoad concload = null;
            foreach (var load in upcanvas.Children)
            {
                switch (load.GetType().Name)
                {
                    case "ConcentratedLoad":

                        concload = load as ConcentratedLoad;

                        break;
                }
            }

            if (concload != null)
            {
                concload.RemoveLabels();
                upcanvas.Children.Remove(concload);
                _concentratedloads = null;
                _concload = null;
                _maxconcload = double.MinValue;
            }
        }

        public void ShowDistLoadDiagram(int c)
        {
            if (_distload != null)
            {
                _distload.Show();
            }
            else if (_distributedloads?.Count > 0)
            {
                var load = new DistributedLoad(_distributedloads, this, c);
                upcanvas.Children.Add(load);
                Canvas.SetBottom(load, 0);
                Canvas.SetLeft(load, 0);
                _distload = load;
            }
        }

        public void HideDistLoadDiagram()
        {
            if (_distload != null)
            {
                _distload.Hide();
            }
        }

        public void DestroyDistLoadDiagram()
        {
            if (_distload != null)
            {
                _distload.RemoveLabels();
                upcanvas.Children.Remove(_distload);
                _distload = null;
            }
        }

        public void ShowConcLoadDiagram(int c)
        {
            if (_concload != null)
            {
                _concload.Show();
            }
            else if (_concentratedloads?.Count > 0)
            {
                var concentratedload = new ConcentratedLoad(_concentratedloads, this, c);
                upcanvas.Children.Add(concentratedload);
                Canvas.SetBottom(concentratedload, 0);
                Canvas.SetLeft(concentratedload, 0);
                _concload = concentratedload;
            }
        }

        public void HideConcLoadDiagram()
        {
            if (_concload != null)
            {
                _concload.Hide();
            }
        }

        public void DestroyConcLoadDiagram()
        {
            if (_concload != null)
            {
                _concload.RemoveLabels();
                upcanvas.Children.Remove(_concload);
                _concload = null;
            }
        }

        public void ShowFixedEndForceDiagram(int c)
        {
            if (_feforce != null)
            {
                _feforce.Show();
            }
            else
            {
                var force = new Force(_fixedendforce, this, c);
                upcanvas.Children.Add(force);
                Canvas.SetBottom(force, 0);
                Canvas.SetLeft(force, 0);
                _feforce = force;
            }
        }

        public void HideFixedEndForceDiagram()
        {
            if (_feforce != null)
            {
                _feforce.Hide();
            }
        }

        public void DestroyFixedEndForceDiagram()
        {
            if (_feforce != null)
            {
                _feforce.RemoveLabels();
                upcanvas.Children.Remove(_feforce);
                _feforce = null;
            }
        }

        public void ShowFixedEndMomentDiagram(int c)
        {
            if (_femoment != null)
            {
                _femoment.Show();
            }
            else if (_fixedendmoment?.Count > 0)
            {
                var moment = new Moment(_fixedendmoment, this, c);
                upcanvas.Children.Add(moment);
                Canvas.SetBottom(moment, 0);
                Canvas.SetLeft(moment, 0);
                _femoment = moment;
            }
        }

        public void HideFixedEndMomentDiagram()
        {
            if (_femoment != null)
            {
                _femoment.Hide();
            }
        }

        public void DestroyFixedEndMomentDiagram()
        {
            if (_femoment != null)
            {
                _femoment.RemoveLabels();
                upcanvas.Children.Remove(_femoment);
                _femoment = null;
            }
        }

        public void ShowInertiaDiagram(int c)
        {
            if (_inertia != null)
            {
                _inertia.Show();
            }
            else
            {
                var inertia = new Inertia(_inertiappoly, this, c);
                upcanvas.Children.Add(inertia);
                Canvas.SetBottom(inertia, 0);
                Canvas.SetLeft(inertia, 0);
                _inertia = inertia;
            }
        }

        public void HideInertiaDiagram()
        {
            if (_inertia != null)
            {
                _inertia.Hide();
            }
        }

        public void DestroyInertiaDiagram()
        {
            if (_inertia != null)
            {
                _inertia.RemoveLabels();
                upcanvas.Children.Remove(_inertia);
                _inertia = null;
            }
        }

        public void ShowDeflectionDiagram()
        {
            if (_deflectiondigram != null)
            {
                _deflectiondigram.Show();
            }
            else if (_deflection?.Count > 0)
            {
                var deflection = new Deflection(_deflection, this);
                upcanvas.Children.Add(deflection);
                Canvas.SetBottom(deflection, 0);
                Canvas.SetLeft(deflection, 0);
                _deflectiondigram = deflection;
            }
        }

        public void HideDeflectionDiagram()
        {
            if (_deflectiondigram != null)
            {
                _deflectiondigram.Hide();
            }
        }

        public void ShowStressDiagram(int c)
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.Show();
            }
            else if (_stress?.Count > 0)
            {
                var stress = new Stress(_stress, this, c);
                upcanvas.Children.Add(stress);
                Canvas.SetBottom(stress, 0);
                Canvas.SetLeft(stress, 0);
                _stressdiagram = stress;
            }
        }

        public void HideStressDiagram()
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.Hide();
            }
        }

        public void DestroyStressDiagram()
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.RemoveLabels();
                upcanvas.Children.Remove(_stressdiagram);
                _stressdiagram = null;
            }
        }

        public void ShowDirectionArrow()
        {
            directionarrow.Visibility = Visibility.Visible;
            _directionshown = true;
        }

        public void HideDirectionArrow()
        {
            directionarrow.Visibility = Visibility.Collapsed;
            _directionshown = false;
        }

        /// <summary>
        /// Adds inertia moment function.
        /// </summary>
        /// <param name="inertiappoly">The inertia Piecewise Polynomial.</param>
        public void AddInertia(PiecewisePoly inertiappoly)
        {
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.Min;
            _maxinertia = _inertiappoly.Max;
            WritePPolytoConsole(_name + " inertia added", inertiappoly);
        }

        public void ChangeInertia(PiecewisePoly inertiappoly)
        {
            DestroyInertiaDiagram();
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.Min;
            _maxinertia = _inertiappoly.Max;
            WritePPolytoConsole(_name + " inertia changed", inertiappoly);
        }

        /// <summary>
        /// Adds the modulus of elasticity.
        /// </summary>
        /// <param name="elasticitymodulus">The modulus of elasticity.</param>
        public void AddElasticity(double elasticitymodulus)
        {
            _elasticity = elasticitymodulus;
        }

        /// <summary>
        /// Adds the e. E stans for neutral axis distance.
        /// </summary>
        /// <param name="eppoly">The eppoly.</param>
        public void AddE(PiecewisePoly eppoly)
        {
            _e = eppoly;
        }

        /// <summary>
        /// Adds the d. D stands for the height of the beam on load direction
        /// </summary>
        /// <param name="dppoly">The dppoly.</param>
        public void AddD(PiecewisePoly dppoly)
        {
            _d = dppoly;
        }

        /// <summary>
        /// Determines whether the beam was selected.
        /// </summary>
        /// <returns>True if the beam was selected. False if the beam was not selected.</returns>
        public bool IsSelected()
        {
            return selected;
        }

        /// <summary>
        /// It is executed when the beam was selected. It changes the beam color and shows circles.
        /// </summary>
        public void Select()
        {
            BringToFront(_canvas, this);
            core.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            if (LeftSide != null)
            {
                if (LeftSide.GetType().Name == "LeftFixedSupport")
                {
                    startcircle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    startcircle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                startcircle.Visibility = Visibility.Visible;
            }

            if (RightSide != null)
            {
                if (RightSide.GetType().Name == "RightFixedSupport")
                {
                    endcircle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    endcircle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                endcircle.Visibility = Visibility.Visible;
            }

            selected = true;
        }

        public void SelectLeftCircle()
        {
            endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            startcircle.Stroke = new SolidColorBrush(Colors.Yellow);
            circledirection = Direction.Left;
            _leftcircleseleted = true;
        }

        public void SelectRightCircle()
        {
            startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            endcircle.Stroke = new SolidColorBrush(Colors.Yellow);
            circledirection = Direction.Right;
            _rightcircleselected = true;
        }

        public void UnSelectCircle()
        {
            startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            circledirection = Direction.None;
            _leftcircleseleted = false;
            _rightcircleselected = false;
        }

        /// <summary>
        /// Executed when the beam was unselected. It changes the beam color and hides circles.
        /// </summary>
        public void UnSelect()
        {
            if (selected)
            {
                core.Fill = new SolidColorBrush(Colors.Black);
                startcircle.Visibility = Visibility.Collapsed;
                startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
                endcircle.Visibility = Visibility.Collapsed;
                endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
                circledirection = Direction.None;
                selected = false;
                UnSelectCircle();
                _tgeometry.HideCorners();
                MesnetDebug.WriteInformation(_name + " Beam unselected : left = " + Canvas.GetLeft(this) + " top = " + Canvas.GetTop(this));
            }
        }

        /// <summary>
        /// Sets the position of the beam based on the center point of the beam. The origin is the top-left corner.
        /// </summary>
        /// <param name="x">The x (horizontal) component of desired point.</param>
        /// <param name="y">The y (vertical) component of desired point.</param>
        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(this, x - Width / 2);

            if (Height > 0)
            {
                Canvas.SetTop(this, y - Height / 2);
            }
            else
            {
                Canvas.SetTop(this, y - 7);
            }
        }

        /// <summary>
        /// Sets the position of desired circle point of the beam.
        /// </summary>
        /// <param name="direction">The direction of the circle.</param>
        /// <param name="x">The x (horizontal) component of desired point.</param>
        /// <param name="y">The y (vertical) component of desired point.</param>
        public void SetPosition(Direction direction, double x, double y)
        {
            Vector delta = new Vector();

            switch (direction)
            {
                case Direction.Left:

                    delta.X = x - this.LeftPoint.X;
                    delta.Y = y - this.LeftPoint.Y;

                    Move(delta);

                    break;

                case Direction.Right:

                    delta.X = x - this.RightPoint.X;
                    delta.Y = y - this.RightPoint.Y;

                    Move(delta);

                    break;
            }
        }

        public void SetPosition(Direction direction, Point point)
        {
            SetPosition(direction, point.X, point.Y);
        }

        /// <summary>
        /// Sets the position of the beam based on top-left point of it.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetAbsolutePosition(double x, double y)
        {
            if (_canbedragged)
            {
                Canvas.SetLeft(this, x);

                if (Height > 0)
                {
                    Canvas.SetTop(this, y);
                }
                else
                {
                    Canvas.SetTop(this, y - 7);
                }
            }
            else
            {
                MesnetDebug.WriteWarning(_name + " The beam to be dragged can not be dragged");
            }
        }

        /// <summary>
        /// Sets the position of the beam based on top-right point of it.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetAbsolutePosition(Point point)
        {
            if (_canbedragged)
            {
                Canvas.SetLeft(this, point.X);

                if (Height > 0)
                {
                    Canvas.SetTop(this, point.Y);
                }
                else
                {
                    Canvas.SetTop(this, point.Y - 7);
                }
            }
            else
            {
                MesnetDebug.WriteWarning(_name + " : The beam to be dragged can not be dragged");
            }
        }

        public void SetTopLeft(double top, double left)
        {
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
            SetTransformGeometry(_canvas);
        }

        public void SetTransformGeometry(Canvas canvas)
        {
            _tgeometry = new TransformGeometry(this, canvas);
        }

        public void SetTransformGeometry(Point tl, Point tr, Point br, Point bl, Canvas canvas)
        {
            _tgeometry = new TransformGeometry(tl, tr, br, bl, _canvas);
        }

        /// <summary>
        /// Changes the position of the beam by the given amount.
        /// </summary>
        /// <param name="delta">The change vector.</param>
        public void Move(Vector delta)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + delta.X);
            Canvas.SetTop(this, Canvas.GetTop(this) + delta.Y);
            _tgeometry.Move(delta);
        }

        /// <summary>
        /// Moves the supports of this beam to the point where the beam is placed.
        /// </summary>
        public void MoveSupports()
        {
            if (LeftSide != null)
            {
                switch (LeftSide.GetType().Name)
                {
                    case "LeftFixedSupport":

                        var ls = LeftSide as LeftFixedSupport;
                        ls.UpdatePosition(this);

                        break;

                    case "SlidingSupport":

                        var ss = LeftSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case "BasicSupport":

                        var bs = LeftSide as BasicSupport;
                        bs.UpdatePosition(this);

                        break;
                }
            }

            if (RightSide != null)
            {
                switch (RightSide.GetType().Name)
                {
                    case "RightFixedSupport":

                        var rs = RightSide as RightFixedSupport;
                        rs.UpdatePosition(this);

                        break;

                    case "SlidingSupport":

                        var ss = RightSide as SlidingSupport;
                        ss.UpdatePosition(this);

                        break;

                    case "BasicSupport":

                        var bs = RightSide as BasicSupport;
                        bs.UpdatePosition(this);

                        break;
                }
            }
        }

        public void AnimatedMove(Point newpoint)
        {
            double newX = newpoint.X;
            double newY = newpoint.Y;
            var top = Canvas.GetTop(this);
            var left = Canvas.GetLeft(this);
            TranslateTransform trans = new TranslateTransform();
            this.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(1));
            DoubleAnimation anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(1));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        /// <summary>
        /// Rotates the beam about its center point.
        /// </summary>
        /// <param name="angle">The desired angle.</param>
        public void SetAngleCenter(double angle)
        {
            double oldangle = rotateTransform.Angle;
            rotateTransform.CenterX = Width / 2;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = angle;
            _angle = angle;

            _tgeometry.RotateAboutCenter(angle - oldangle);
        }

        /// <summary>
        /// Rotates the beam about its left point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleLeft(double angle)
        {
            SetAngleCenter(angle);
            leftreconnect();
        }

        /// <summary>
        /// Rotates the beam about its right point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleRight(double angle)
        {
            SetAngleCenter(angle);
            rightreconnect();
        }

        /// <summary>
        /// Determines whether the specified point is inside of the rectangle geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is inside; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInside(Point point)
        {
            return _tgeometry.IsInsideOuter(point);
        }

        /// <summary>
        /// Shows the corners of the outer rectangle in canvas. It is used in debug purposes.
        /// </summary>
        /// <param name="radius">The radius of the corner circle.</param>
        public void ShowCorners(double radius)
        {
            _tgeometry.ShowCorners(radius);
        }

        /// <summary>
        /// Shows the corners of the outer rectangle in canvas with predefined values of 5 and 7. 
        /// It is used in debug purposes.
        /// </summary>
        public void ShowCorners()
        {
            ShowCorners(5, 7);
        }

        /// <summary>
        /// Shows the corners rectangle in canvas. It is used in debug purposes.
        /// </summary>
        /// <param name="radiusinner">The inner transform geometry circle radius.</param>
        /// <param name="radiusouter">The outer transform geometry circle radius.</param>
        public void ShowCorners(double radiusinner, double radiusouter)
        {
            //_tgeometry.ShowCorners(radiusinner, radiusouter);
        }

        public void HideCorners()
        {
            _tgeometry.HideCorners();
        }

        /// <summary>
        /// Brings to ui element to front by increasing z index in canvas.
        /// </summary>
        /// <param name="pParent">Parent canvas.</param>
        /// <param name="pToMove">Ui Element to bring front.</param>
        private void BringToFront(Canvas pParent, UserControl pToMove)
        {
            try
            {
                int currentIndex = Canvas.GetZIndex(pToMove);
                int zIndex = 0;
                int maxZ = 0;
                UserControl child;
                for (int i = 0; i < pParent.Children.Count; i++)
                {
                    if (pParent.Children[i] is UserControl &&
                        pParent.Children[i] != pToMove)
                    {
                        child = pParent.Children[i] as UserControl;
                        zIndex = Canvas.GetZIndex(child);
                        maxZ = Math.Max(maxZ, zIndex);
                        if (zIndex > currentIndex)
                        {
                            Canvas.SetZIndex(child, zIndex - 1);
                        }
                    }
                }
                Canvas.SetZIndex(pToMove, maxZ);
            }
            catch (Exception ex)
            {
            }
        }

        #region SoM

        /// <summary>
        /// Finds the left and right support forces under the distributed load conditions.
        /// </summary>
        private void finddistributedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_distributedloads?.Count > 0)
            {
                var forcelist = new List<KeyValuePair<double, double>>();

                foreach (Poly load in _distributedloads)
                {
                    var forces = load.CalculateMagnitudeAndLocation();
                    forcelist.AddRange(forces);
                }

                //Moment from left support point
                double leftmoment = 0;
                foreach (var force in forcelist)
                {
                    leftmoment += force.Key * force.Value;
                }
                _rightsupportforcedist = leftmoment / _length;

                //Moment from right support point
                double rightmoment = 0;
                foreach (var force in forcelist)
                {
                    rightmoment += (_length - force.Key) * force.Value;
                }
                _leftsupportforcedist = rightmoment / _length;
            }
            else
            {
                _leftsupportforcedist = 0;
                _rightsupportforcedist = 0;
            }

            MesnetDebug.WriteInformation(_name + " : resultantforce = " + resultantforce + " resultantforcedistance = " + resultantforcedistance);

            MesnetDebug.WriteInformation(_name + " : leftsupportforcedist = " + _leftsupportforcedist + " rightsupportforcedist = " + _rightsupportforcedist);

        }

        private void findconcentratedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_concentratedloads?.Count > 0)
            {
                //Moment from left support point
                double leftmoment = 0;
                foreach (KeyValuePair<double, double> force in _concentratedloads)
                {
                    leftmoment += force.Key * force.Value;
                }
                _rightsupportforceconc = leftmoment / _length;

                //Moment from right support point
                double rightmoment = 0;
                foreach (KeyValuePair<double, double> force in _concentratedloads)
                {
                    rightmoment += (_length - force.Key) * force.Value;
                }
                _leftsupportforceconc = rightmoment / _length;
            }
            else
            {
                _leftsupportforceconc = 0;
                _rightsupportforceconc = 0;
            }

            MesnetDebug.WriteInformation(_name + " : resultantforcedistance = " + resultantforcedistance);

            MesnetDebug.WriteInformation(_name + " : leftsupportforceconc = " + _leftsupportforceconc + " rightsupportforceconc = " + _rightsupportforceconc);
        }

        #region Zero Condition

        private void findconcentratedzeroforce()
        {
            _zeroforceconc = new PiecewisePoly();

            if (_concentratedloads?.Count > 0)
            {
                double leftforce = _leftsupportforceconc;

                if (_concentratedloads[0].Key > 0)
                {
                    var poly1 = new Poly(leftforce.ToString());
                    poly1.StartPoint = 0;
                    poly1.EndPoint = _concentratedloads[0].Key;
                    _zeroforceconc.Add(poly1);
                }

                for (int i = 0; i < _concentratedloads.Count; i++)
                {
                    leftforce = leftforce - _concentratedloads[i].Value;

                    var poly = new Poly(leftforce.ToString());

                    poly.StartPoint = _concentratedloads[i].Key;
                    if (i + 1 < _concentratedloads.Count)
                    {
                        poly.EndPoint = _concentratedloads[i + 1].Key;
                    }
                    else
                    {
                        poly.EndPoint = _length;
                    }

                    _zeroforceconc.Add(poly);
                }
                WritePPolytoConsole(_name + " : _zeroforceconc", _zeroforceconc);
            }
        }

        /// <summary>
        /// Finds the zero force polynomial which is the force polynomial when there is no fixed support in the end of the beam.
        /// </summary>
        private void finddistributedzeroforce()
        {
            _zeroforcedist = new PiecewisePoly();

            if (_distributedloads?.Count > 0)
            {
                if (_distributedloads[0].StartPoint != 0)
                {
                    var ply = new Poly(_leftsupportforcedist.ToString());
                    ply.StartPoint = 0;
                    ply.EndPoint = _distributedloads[0].StartPoint;
                    _zeroforcedist.Add(ply);
                }

                foreach (Poly load in _distributedloads)
                {
                    var index = _distributedloads.IndexOf(load);

                    double weightsbefore = findforcebefore(index);

                    if (index > 0)
                    {
                        if (_distributedloads[index - 1].EndPoint != _distributedloads[index].StartPoint)
                        {
                            var ply = new Poly(weightsbefore.ToString());
                            ply.StartPoint = _distributedloads[index - 1].EndPoint;
                            ply.EndPoint = _distributedloads[index].StartPoint;
                            _zeroforcedist.Add(ply);
                        }
                    }

                    var poly = new Poly();

                    var integration = load.Integrate();
                    var zerovalue = load.Integrate().Calculate(load.StartPoint);
                    if (zerovalue != 0)
                    {
                        if (weightsbefore != 0)
                        {
                            poly = new Poly(weightsbefore.ToString()) - integration + new Poly(zerovalue.ToString());
                        }
                        else
                        {
                            poly = -1 * integration + new Poly(zerovalue.ToString());
                        }
                    }
                    else
                    {
                        if (weightsbefore != 0)
                        {
                            poly = new Poly(weightsbefore.ToString()) - integration;
                        }
                        else
                        {
                            poly = -1 * integration;
                        }
                    }
                    poly.StartPoint = load.StartPoint;
                    poly.EndPoint = load.EndPoint;
                    _zeroforcedist.Add(poly);
                }
                _zeroforcedist.Sort();

                if (_distributedloads.Last().EndPoint != _length)
                {
                    var weights = findforcebefore(_distributedloads.Count);
                    var ply = new Poly(weights.ToString());
                    ply.StartPoint = _distributedloads.Last().EndPoint;
                    ply.EndPoint = _length;
                    _zeroforcedist.Add(ply);
                }

                WritePPolytoConsole(_name + " : _zeroforcedist", _zeroforcedist);
            }
        }

        /// <summary>
        /// Calculates the zero moment, the monet when the beam is bounded with basic supports on both sides.
        /// </summary>
        private void findzeromoment()
        {
            _zeromoment = new PiecewisePoly();

            foreach (Poly force in _zeroforce)
            {
                var index = _zeroforce.IndexOf(force);
                var poly = new Poly();
                var integration = force.Integrate();
                var momentsbefore = findmomentbefore(index);
                var zerovalue = force.Integrate().Calculate(force.StartPoint);
                var constant = momentsbefore - zerovalue;

                MesnetDebug.WriteInformation(_name + " : integration = " + integration.ToString() + " momentsbefore = " + momentsbefore + " zeroforcevalue = " + zerovalue + " startpoint = " + force.StartPoint + " endpoint = " + force.EndPoint);

                if (constant != 0)
                {
                    poly = integration + new Poly(constant.ToString());
                }
                else
                {
                    poly = integration;
                }

                poly.StartPoint = force.StartPoint;
                poly.EndPoint = force.EndPoint;
                _zeromoment.Add(poly);
                _zeromoment.Sort();
            }

            WritePPolytoConsole(_name + " : Fixed End Force", _zeromoment);
        }

        /// <summary>
        /// Calculates weights forces before the force poly whose index is given.
        /// </summary>
        /// <param name="index">The load polynomial index.</param>
        /// <returns></returns>
        private double findforcebefore(int index)
        {
            double weights = _leftsupportforcedist;

            int indx = 0;

            while (indx < index)
            {
                var weight = _distributedloads[indx].DefiniteIntegral(_distributedloads[indx].StartPoint, _distributedloads[indx].EndPoint);
                weights -= weight;

                indx++;
            }

            return weights;
        }

        /// <summary>
        /// Calculates moments before the moment poly whose index is given.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private double findmomentbefore(int index)
        {
            double moments = 0;
            int indx = 0;

            while (indx < index)
            {
                var area = _zeroforce[indx].DefiniteIntegral(_zeroforce[indx].StartPoint, _zeroforce[indx].EndPoint);
                moments += area;
                indx++;
            }

            return moments;
        }

        #endregion

        #region clapeyron

        /// <summary>
        /// Finds end moments in case of both end have fixed support.
        /// </summary>
        private void ffsolver()
        {
            if (_zeromoment.Count > 0)
            {
                double ma1 = 0;
                double ma2 = 0;
                double mb1 = 0;
                double mb2 = 0;
                double r1 = 0;
                double r2 = 0;

                ///////////////////////////////////////////////////////////
                /////////////////Left Equation Solve///////////////////////
                //////////////////////////////////////////////////////////

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    //When the inertia distribution is constant dont waste time and cpu with simpson numerical integration, integrate it analytically.
                    //Since izero equals inertia the expression can be simplified
                    MesnetDebug.WriteInformation(_name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MesnetDebug.WriteInformation(_name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : mb2 = " + mb2);

                    var mox = _zeromoment * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MesnetDebug.WriteInformation(_name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MesnetDebug.WriteInformation(_name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MesnetDebug.WriteInformation(_name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson6.AddData(_zeromoment.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MesnetDebug.WriteInformation(_name + " : r2 = " + r2);
                }

                double[,] coefficients =
                {
                    {ma1, mb1},
                    {ma2, mb2},
                };

                double[] results =
                {
                    r1, r2
                };

                //////////////////////////////////////////////////////////

                var moments = LinearEquationSolver(coefficients, results);

                //_ma = Math.Round(moments[0], 4);

                //_mb = Math.Round(moments[1], 4);

                _ma = -moments[0];

                _mb = -moments[1];
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetDebug.WriteInformation(_name + " : ma = " + _ma);
            MesnetDebug.WriteInformation(_name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have fixed support when there is only one beam.
        /// </summary>
        private void ffsolverclapeyron()
        {
            if (_zeromoment.Count > 0)
            {
                double ma1 = 0;
                double ma2 = 0;
                double mb1 = 0;
                double mb2 = 0;
                double r1 = 0;
                double r2 = 0;

                ///////////////////////////////////////////////////////////
                /////////////////Left Equation Solve///////////////////////
                //////////////////////////////////////////////////////////

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    //When the inertia distribution is constant dont waste time and cpu with simpson numerical integration, integrate it analytically.
                    //Since izero equals inertia the expression can be simplified
                    MesnetDebug.WriteInformation(_name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    mb1 = _length / 2 - ma1;
                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    ma2 = _length / 6;
                    MesnetDebug.WriteInformation(_name + " : ma2 = " + ma2);

                    mb2 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : mb2 = " + mb2);

                    var mox = _zeromoment * xppoly;
                    r2 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r2 = " + r2);
                }
                else
                {
                    //When the inertia distribution is not constant, there is no choice but to use numerical integration 
                    //since the integration can not be solved analytically using polinomials in this program.
                    MesnetDebug.WriteInformation(_name + " : Numerical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////            

                    var simpson2 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson2.Calculate();

                    var value1 = 1 / _length * simpson2.Result;

                    mb1 = value1 - ma1;

                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) *
                                         x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    ////////////////////////////////////////////////////////////
                    /////////////////Right Equation Solve///////////////////////
                    ////////////////////////////////////////////////////////////

                    var simpson4 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson4.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson4.Calculate();

                    var value2 = 1 / Math.Pow(_length, 2) * simpson4.Result;

                    var simpson5 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    ma2 = 1 / _length * simpson5.Result - value2;

                    MesnetDebug.WriteInformation(_name + " : ma2 = " + ma2);

                    ///////////////////////////////////////////////////////////

                    mb2 = value2;

                    MesnetDebug.WriteInformation(_name + " : mb2 = " + mb2);

                    ///////////////////////////////////////////////////////////

                    var simpson6 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson6.AddData(_zeromoment.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) *
                                         xppoly.Calculate(i));
                    }

                    simpson6.Calculate();

                    r2 = -1 / _length * simpson6.Result;

                    MesnetDebug.WriteInformation(_name + " : r2 = " + r2);
                }

                double[,] coefficients =
                {
                    {ma1, mb1},
                    {ma2, mb2},
                };

                double[] results =
                {
                    r1, r2
                };

                //////////////////////////////////////////////////////////

                var moments = LinearEquationSolver(coefficients, results);

                //_ma = Math.Round(moments[0], 4);

                //_mb = Math.Round(moments[1], 4);

                _ma = moments[0];

                _mb = moments[1];
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetDebug.WriteInformation(_name + " : ma = " + _ma);
            MesnetDebug.WriteInformation(_name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the left end has fixed support and the right and basic or sliding support.
        /// </summary>
        private void fbsolver()
        {
            if (_zeromoment.Count > 0)
            {
                double ma1;
                double r1;

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                if (_analyticalsolution)
                {
                    MesnetDebug.WriteInformation(_name + " : Analytical solution started");

                    ma1 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    var moxp = _zeromoment.Propagate(_length) * xppoly;
                    r1 = -1 / _length * moxp.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
                else
                {
                    MesnetDebug.WriteInformation(_name + " : Analytical solution started");

                    var conjugateinertia = _inertiappoly.Conjugate(_length);

                    var simpson1 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    ma1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

                    MesnetDebug.WriteInformation(_name + " : ma1 = " + ma1);

                    //////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(SimpsonStep);

                    var conjugatemoment = _zeromoment.Conjugate(_length);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;
                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    _ma = r1 / ma1;
                    _mb = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }
            MesnetDebug.WriteInformation(_name + " : ma = " + _ma);
            MesnetDebug.WriteInformation(_name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the right end has fixed support and the left and basic or sliding support.
        /// </summary>
        private void bfsolver()
        {
            if (_zeromoment.Count > 0)
            {
                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                var x = new Poly("x");
                x.StartPoint = 0;
                x.EndPoint = _length;

                var xppoly = new PiecewisePoly();
                xppoly.Add(x);

                double mb1;
                double r1;

                if (_analyticalsolution)
                {
                    MesnetDebug.WriteInformation(_name + " : Analytical solution started");
                    mb1 = _length / 3;
                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    var mox = _zeromoment * xppoly;
                    r1 = -1 / _length * mox.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
                else
                {
                    MesnetDebug.WriteInformation(_name + " : Numerical solution started");
                    var simpson1 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson1.AddData(_izero / _inertiappoly.Calculate(i) * xsquare.Calculate(i));
                    }

                    simpson1.Calculate();

                    mb1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

                    MesnetDebug.WriteInformation(_name + " : mb1 = " + mb1);

                    ///////////////////////////////////////////////////////////

                    var simpson3 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson3.AddData(_izero / _inertiappoly.Calculate(i) * _zeromoment.Calculate(i) * x.Calculate(i));
                    }

                    simpson3.Calculate();

                    r1 = -1 / _length * simpson3.Result;

                    MesnetDebug.WriteInformation(_name + " : r1 = " + r1);

                    _mb = r1 / mb1;
                    _ma = 0;
                }
            }
            else
            {
                _ma = 0;
                _mb = 0;
            }

            MesnetDebug.WriteInformation(_name + " : ma = " + _ma);
            MesnetDebug.WriteInformation(_name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of both end have basic or sliding support.
        /// </summary>
        private void bbsolver()
        {
            _mb = 0;
            _ma = 0;

            MesnetDebug.WriteInformation(_name + " : ma = " + _ma);
            MesnetDebug.WriteInformation(_name + " : mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments according to end moments that are found by solvers.
        /// </summary>
        private void findfixedendmomentcross()
        {
            var polylist = new List<Poly>();

            var constant = (_mb - _ma) / _length;

            if (Math.Abs(constant) < 0.00000001)
            {
                constant = 0.0;
            }

            foreach (Poly moment in _zeromoment)
            {
                var poly = new Poly();
                var poly1 = new Poly(_ma.ToString(), moment.StartPoint, moment.EndPoint);
                var poly2 = new Poly("x", moment.StartPoint, moment.EndPoint);
                if (!constant.Equals(0.0))
                {
                    var poly3 = new Poly(constant.ToString(), moment.StartPoint, moment.EndPoint);
                    poly = moment - poly1 - poly2 * poly3;
                }
                else
                {
                    poly = moment - poly1;
                }
                poly.StartPoint = moment.StartPoint;
                poly.EndPoint = moment.EndPoint;
                polylist.Add(poly);
            }
            _fixedendmoment = new PiecewisePoly(polylist);

            WritePPolytoConsole(_name + " : Fixed End Moment", _fixedendmoment);
        }

        private void findfixedendmomentclapeyron()
        {
            var polylist = new List<Poly>();

            var constant = (_mb - _ma) / _length;

            if (Math.Abs(constant) < 0.00000001)
            {
                constant = 0.0;
            }

            foreach (Poly moment in _zeromoment)
            {
                var resultpoly = new Poly();
                resultpoly.StartPoint = moment.StartPoint;
                resultpoly.EndPoint = moment.EndPoint;
                var mapoly = new Poly(_ma.ToString(), moment.StartPoint, moment.EndPoint);
                var xpoly = new Poly("x", moment.StartPoint, moment.EndPoint);

                if (!constant.Equals(0.0))
                {
                    var cpoly = new Poly(constant.ToString(), moment.StartPoint, moment.EndPoint);
                    resultpoly = moment + mapoly + xpoly * cpoly;
                }
                else
                {
                    resultpoly = moment + mapoly;
                }
                resultpoly.StartPoint = moment.StartPoint;
                resultpoly.EndPoint = moment.EndPoint;
                polylist.Add(resultpoly);
            }
            _fixedendmoment = new PiecewisePoly(polylist);

            WritePPolytoConsole(_name + " : Fixed End Moment", _fixedendmoment);
        }

        /// <summary>
        /// Calculates the deflection of the beam on selected point. The deflection toward beam's red arrow direction is accepted as positive.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public double Deflection(double x)
        {
            var simpson1 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                var mom = _fixedendmoment.Calculate(i);
                var iner = _inertiappoly.Calculate(i);

                simpson1.AddData(mom * (_length - i) / iner);
            }

            simpson1.Calculate();

            var int1 = simpson1.Result;

            var simpson2 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= x; i = i + 0.0001)
            {
                var mom = _fixedendmoment.Calculate(i);
                var iner = _inertiappoly.Calculate(i);

                simpson2.AddData(mom * (x - i) / iner);
            }

            simpson2.Calculate();

            var int2 = simpson2.Result;

            double deflection = x / _length * int1 - int2;

            return deflection;
        }

        public void ClapeyronCalculate()
        {
            MesnetDebug.WriteInformation(_name + " : ClapeyronCalculate has started to work");
            findconcentratedsupportforces();
            finddistributedsupportforces();
            findconcentratedzeroforce();
            finddistributedzeroforce();
            _zeroforce = _zeroforceconc + _zeroforcedist;
            WritePPolytoConsole(_name + " : Zero Force", _zeroforce);
            findzeromoment();
            canbesolvedanalytically();
            clapeyronsupportcase();
            findfixedendmomentclapeyron();

            MesnetDebug.WriteInformation(_name + " Left End Moment = " + _ma);
            Logger.WriteLine(_name + " Left End Moment = " + _ma);
            MesnetDebug.WriteInformation(_name + " Right End Moment = " + _mb);
            Logger.WriteLine(_name + " Right End Moment = " + _mb);
        }

        /// <summary>
        /// Calculates moments when there is only one beam presented.
        /// </summary>
        private void clapeyronsupportcase()
        {
            #region cross support cases

            switch (GetObjectType(LeftSide))
            {
                case ObjectType.LeftFixedSupport:

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.RightFixedSupport:

                            MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                            ffsolverclapeyron();

                            break;

                        case ObjectType.BasicSupport:

                            MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                            fbsolver();

                            break;

                        case ObjectType.SlidingSupport:

                            MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                            fbsolver();

                            break;
                    }

                    break;

                case ObjectType.BasicSupport:

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.RightFixedSupport:

                            MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case ObjectType.BasicSupport:

                            MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case ObjectType.SlidingSupport:

                            MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                            bbsolver();

                            break;
                    }

                    break;

                case ObjectType.SlidingSupport:

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.RightFixedSupport:

                            MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                            bfsolver();

                            break;

                        case ObjectType.BasicSupport:

                            MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                            bbsolver();

                            break;

                        case ObjectType.SlidingSupport:

                            MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                            bbsolver();

                            break;
                    }

                    break;
            }

            #endregion
        }

        /// <summary>
        /// Checks if the beam can be calculated analytically, without numerical integration.
        /// If the beam has constant and only one inertia polynomial, and if the beam has 
        /// integer-powered zero moment poly, the analytical solution can be done which is a way
        /// faster than numerical solution 
        /// </summary>
        private void canbesolvedanalytically()
        {
            //Check inertia ppoly has only one poly
            if (_inertiappoly.Count > 1)
            {
                _analyticalsolution = false;
                MesnetDebug.WriteInformation(_name + " : Analytical solution is not possible");
            }

            //Check if inertia ppoly is constant or not dependant on x
            if (_inertiappoly.Degree() > 0)
            {
                _analyticalsolution = false;
                MesnetDebug.WriteInformation(_name + " : Analytical solution is not possible");
            }

            //Check if zero moment ppoly has any term with non-integer power
            if (_zeromoment.Count > 0)
            {
                foreach (Poly poly in _zeromoment)
                {
                    foreach (Term term in poly.Terms)
                    {
                        if (term.Power % 1 != 0)
                        {
                            _analyticalsolution = false;
                            MesnetDebug.WriteInformation(_name + " : Analytical solution is not possible");
                        }
                    }
                }
            }
            _analyticalsolution = true;
            MesnetDebug.WriteInformation(_name + " : Analytical solution is possible");
        }

        #endregion

        #region cross

        /// <summary>
        /// Conducts cross-balancing moment from the given direction to the other direction of the beam.
        /// </summary>
        /// <param name="direction">The first direction.</param>
        public void Conduct(Direction direction, double moment)
        {
            #region code

            switch (direction)
            {
                case Direction.Left:

                    double conductmoment;

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.BasicSupport:

                            BasicSupport bs = RightSide as BasicSupport;

                            if (bs.Members.Count > 1)
                            {
                                conductmoment = moment * CarryOverAB;

                                _mb = _mb + conductmoment;

                                Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because right side of this beam is bounded to a free basic support(" + bs.Name + ")");
                            }

                            break;

                        case ObjectType.SlidingSupport:

                            SlidingSupport ss = RightSide as SlidingSupport;

                            if (ss.Members.Count > 1)
                            {
                                conductmoment = moment * CarryOverAB;

                                _mb = _mb + conductmoment;

                                Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because right side of this beam is bounded to a free sliding support(" + ss.Name + ")");
                            }

                            break;

                        case ObjectType.RightFixedSupport:

                            RightFixedSupport rs = RightSide as RightFixedSupport;

                            conductmoment = moment * CarryOverAB;

                            _mb = _mb + conductmoment;

                            Logger.WriteLine(this.Name + " : Left to right conduct moment = " + conductmoment);

                            Logger.WriteLine(this.Name + " : " + rs.Name + " is a fixed support. So there will be no seperation in this support");

                            break;
                    }

                    break;

                case Direction.Right:

                    double conductmoment1;

                    switch (GetObjectType(LeftSide))
                    {
                        case ObjectType.BasicSupport:

                            BasicSupport bs = LeftSide as BasicSupport;

                            if (bs.Members.Count > 1)
                            {
                                conductmoment1 = moment * CarryOverBA;

                                _ma = _ma + conductmoment1;

                                Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because left side of this beam is bounded to a free basic support(" + bs.Name + ")");
                            }

                            break;

                        case ObjectType.SlidingSupport:

                            SlidingSupport ss = LeftSide as SlidingSupport;

                            if (ss.Members.Count > 1)
                            {
                                conductmoment1 = moment * CarryOverBA;

                                _ma = _ma + conductmoment1;

                                Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);
                            }
                            else
                            {
                                Logger.WriteLine(this.Name + " : Moment not conducted because left side of this beam is bounded to a free sliding support (" + ss.Name + ")");
                            }

                            break;

                        case ObjectType.LeftFixedSupport:

                            LeftFixedSupport ls = LeftSide as LeftFixedSupport;

                            conductmoment1 = moment * CarryOverBA;

                            _ma = _ma + conductmoment1;

                            Logger.WriteLine(this.Name + " : Right to left conduct moment = " + conductmoment1);

                            Logger.WriteLine(this.Name + " : " + ls.Name + " is a fixed support. So there will be no seperation in this support");

                            break;
                    }

                    break;
            }

            #endregion
        }

        /// <summary>
        /// Calculates alfa, beta, gama, k and fi coefficients.
        /// </summary>
        private void findcrosscoefficients()
        {
            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            var lxpoly = new Poly(_length.ToString() + "-x");
            lxpoly.StartPoint = 0;
            lxpoly.EndPoint = _length;

            var xppoly = new PiecewisePoly();
            xppoly.Add(x);

            if (_analyticalsolution)
            {
                MesnetDebug.WriteInformation(_name + " : Analytical solution started");
                _alfaa = 1.0 / 3;
                MesnetDebug.WriteInformation(_name + " : alfaa = " + _alfaa);
                Logger.WriteLine(this.Name + " : alfaa = " + _alfaa);

                _alfab = 1.0 / 3;
                MesnetDebug.WriteInformation(_name + " : alfab = " + _alfab);
                Logger.WriteLine(this.Name + " : alfab = " + _alfab);

                _beta = 1.0 / 6;
                MesnetDebug.WriteInformation(_name + " : beta = " + _beta);
                Logger.WriteLine(_name + " : beta = " + _beta);
            }
            else
            {
                MesnetDebug.WriteInformation(_name + " : Numerical solution started");
                var simpson1 = new SimpsonIntegrator(SimpsonStep);

                for (double i = 0; i <= _length; i = i + SimpsonStep)
                {
                    simpson1.AddData(Math.Pow(lxpoly.Calculate(i), 2) / _inertiappoly.Calculate(i));
                }

                simpson1.Calculate();

                _alfaa = _izero / Math.Pow(_length, 3) * simpson1.Result;

                MesnetDebug.WriteInformation(_name + " : alfaa = " + _alfaa);

                Logger.WriteLine(this.Name + " : alfaa = " + _alfaa);

                var simpson2 = new SimpsonIntegrator(SimpsonStep);

                var xsquare = new Poly("x^2");
                xsquare.StartPoint = 0;
                xsquare.EndPoint = _length;

                for (double i = 0; i <= _length; i = i + SimpsonStep)
                {
                    simpson2.AddData(xsquare.Calculate(i) / _inertiappoly.Calculate(i));
                }

                simpson2.Calculate();

                _alfab = _izero / Math.Pow(_length, 3) * simpson2.Result;

                MesnetDebug.WriteInformation(_name + " : alfab = " + _alfab);

                Logger.WriteLine(_name + " : alfab = " + _alfab);

                var simpson3 = new SimpsonIntegrator(SimpsonStep);

                for (double i = 0; i <= _length; i = i + SimpsonStep)
                {
                    simpson3.AddData((lxpoly.Calculate(i) * x.Calculate(i)) / _inertiappoly.Calculate(i));
                }

                simpson3.Calculate();

                _beta = _izero / Math.Pow(_length, 3) * simpson3.Result;

                MesnetDebug.WriteInformation(_name + " : beta = " + _beta);

                Logger.WriteLine(_name + " : beta = " + _beta);
            }

            if (_zeromoment.Count > 0)
            {
                var mox = _zeromoment * xppoly;

                if (_analyticalsolution)
                {
                    _ka = 6.0 * _izero / Math.Pow(_length, 2) * mox.DefiniteIntegral(0, _length);
                    MesnetDebug.WriteInformation(_name + " : ka = " + _ka);
                    Logger.WriteLine(_name + " : ka = " + _ka);

                    var lxppoly = new PiecewisePoly();
                    lxppoly.Add(lxpoly);

                    var mlx = _zeromoment * lxppoly;

                    _kb = 6.0 * _izero / Math.Pow(_length, 2) * mlx.DefiniteIntegral(0, _length);
                    Logger.WriteLine(_name + " : kb = " + _kb);
                    MesnetDebug.WriteInformation(_name + " : kb = " + _kb);
                }
                else
                {
                    var simpson4 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson4.AddData(mox.Calculate(i) / _inertiappoly.Calculate(i));
                    }

                    simpson4.Calculate();

                    _ka = 6 * _izero / Math.Pow(_length, 2) * simpson4.Result;

                    MesnetDebug.WriteInformation(_name + " : ka = " + _ka);

                    Logger.WriteLine(_name + " : ka = " + _ka);

                    var simpson5 = new SimpsonIntegrator(SimpsonStep);

                    for (double i = 0; i <= _length; i = i + SimpsonStep)
                    {
                        simpson5.AddData((_zeromoment.Calculate(i) * lxpoly.Calculate(i)) / _inertiappoly.Calculate(i));
                    }

                    simpson5.Calculate();

                    _kb = 6 * _izero / Math.Pow(_length, 2) * simpson5.Result;

                    Logger.WriteLine(_name + " : kb = " + _kb);

                    MesnetDebug.WriteInformation(_name + " : kb = " + _kb);
                }
            }
            else
            {
                _ka = 0;
                _kb = 0;
            }

            _fia = _length * (_kb / 6 + _mb * _beta + _ma * _alfaa) / (_elasticity * _izero);
            MesnetDebug.WriteInformation(_name + " : fia = " + _fia);

            _fib = -_length * (_ka / 6 + _ma * _beta + _mb * _alfab) / (_elasticity * _izero);
            MesnetDebug.WriteInformation(_name + " : fib = " + _fib);

            _gamaba = _beta / _alfaa;
            MesnetDebug.WriteInformation(_name + " : gamaba = " + _gamaba);

            _gamaab = _beta / _alfab;
            MesnetDebug.WriteInformation(_name + " : gamaab = " + _gamaab);

            #region stiffnesses with support cases

            switch (GetObjectType(LeftSide))
            {
                case ObjectType.LeftFixedSupport:

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.RightFixedSupport:

                            MesnetDebug.WriteInformation(_name + " : stiffness case 1");

                            _stiffnessa = 0;

                            _stiffnessb = 0;

                            break;

                        case ObjectType.BasicSupport:

                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MesnetDebug.WriteInformation(_name + " : stiffness case 2");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MesnetDebug.WriteInformation(_name + " : stiffness case 3");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;

                        case ObjectType.SlidingSupport:

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MesnetDebug.WriteInformation(_name + " : stiffness case 4");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MesnetDebug.WriteInformation(_name + " : stiffness case 5");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;
                    }

                    break;

                case ObjectType.BasicSupport:

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : stiffness case 6");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 7");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 8");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 9");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : stiffness case 10");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 11");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 12");
                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 13");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 14");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;

                case ObjectType.SlidingSupport:

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : stiffness case 15");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 16");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 17");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : tiffness case 18");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 19");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : stiffness case 20");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 21");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 22");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 23");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : stiffness case 24");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;
            }

            _stiffnessa = Math.Round(_stiffnessa, 5);

            _stiffnessb = Math.Round(_stiffnessb, 5);

            MesnetDebug.WriteInformation(_name + " : StiffnessA = " + _stiffnessa);

            Logger.WriteLine(_name + " : StiffnessA = " + _stiffnessa);

            MesnetDebug.WriteInformation(_name + " : StiffnessB = " + _stiffnessb);

            Logger.WriteLine(_name + " : StiffnessB = " + _stiffnessb);
            #endregion
        }

        /// <summary>
        /// Chooses the solver to be executed according to supports in the way of Cross Method.
        /// </summary>
        private void crosssupportcases()
        {
            #region cross support cases

            switch (GetObjectType(LeftSide))
            {
                case ObjectType.LeftFixedSupport:

                    switch (GetObjectType(RightSide))
                    {
                        case ObjectType.RightFixedSupport:

                            MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                            ffsolver();

                            break;

                        case ObjectType.BasicSupport:

                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                                fbsolver();
                            }

                            break;

                        case ObjectType.SlidingSupport:

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                                fbsolver();
                            }

                            break;
                    }

                    break;

                case ObjectType.BasicSupport:

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                ffsolver();

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                bfsolver();

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;

                case ObjectType.SlidingSupport:

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                ffsolver();

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (GetObjectType(RightSide))
                        {
                            case ObjectType.RightFixedSupport:

                                MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                bfsolver();

                                break;

                            case ObjectType.BasicSupport:

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MesnetDebug.WriteInformation(_name + " : bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MesnetDebug.WriteInformation(_name + " : bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;
            }

            #endregion
        }

        /// <summary>
        /// Main function that prepares parameters and conducts Cross Solution for the beam.
        /// </summary>
        public void CrossCalculate()
        {
            MesnetDebug.WriteInformation(_name + " : CrossCalculate has started to work");

            findconcentratedsupportforces();

            finddistributedsupportforces();

            findconcentratedzeroforce();

            finddistributedzeroforce();

            _zeroforce = _zeroforceconc + _zeroforcedist;

            WritePPolytoConsole(_name + " : _zeroforce", _zeroforce);

            findzeromoment();

            canbesolvedanalytically();

            crosssupportcases();

            findfixedendmomentcross();

            findcrosscoefficients();

            _maclapeyron = _ma;

            MesnetDebug.WriteInformation(_name + " : Clapeyron Ma = " + _maclapeyron);

            Logger.WriteLine(_name + " : Clapeyron Ma = " + _maclapeyron);

            _mbclapeyron = _mb;

            MesnetDebug.WriteInformation(_name + " : Clapeyron Mb = " + _mbclapeyron);

            Logger.WriteLine(_name + " : Clapeyron Mb = " + _mbclapeyron);

            //Normal to Cross sign conversion

            if (Deflection(0.001) < 0)
            {
                _ma = Negative(_ma);
            }
            else
            {
                _ma = Positive(_ma);
            }

            if (Deflection(_length - 0.001) < 0)
            {
                _mb = Positive(_mb);
            }
            else
            {
                _mb = Negative(_mb);
            }

            MesnetDebug.WriteInformation(_name + " : Ma = " + _ma);

            Logger.WriteLine(_name + " : Cross Ma = " + _ma);

            Logger.WriteLine(_name + " : Cross Mb = " + _mb);

            Logger.NextLine();

            MesnetDebug.WriteInformation(_name + " : Mb = " + _mb);

            MesnetDebug.WriteInformation(_name + " : CrossCalculate has finished to work");
        }

        public void ResetSolution()
        {
            _ma = 0;
            _mb = 0;
            _fixedendforce = null;
            _fixedendmoment = null;
            _zeroforce = null;
            _zeroforceconc = null;
            _zeroforcedist = null;
            _zeromoment = null;
            _maxforce = Double.MinValue;
            _maxabsmoment = Double.MinValue;
            _minmoment = Double.MaxValue;
            _maxforce = Double.MinValue;
            _maxabsforce = Double.MinValue;
            _minforce = Double.MinValue;
            _maxstress = Double.MinValue;
            _maxabsstress = Double.MinValue;
            DestroyFixedEndMomentDiagram();
            DestroyFixedEndForceDiagram();
            DestroyStressDiagram();
        }

        #endregion

        #region post-cross

        /// <summary>
        /// Calculates final moment, force and stress distributions after Cross loop according to the results
        /// </summary>
        public void PostCrossUpdate()
        {
            updatemoments();

            updateforces();

            if (_stressanalysis)
            {
                updatestresses();
            }

            //updatedeflection();
        }

        public void PostClapeyronUpdate()
        {
            _maxmoment = _fixedendmoment.Max;

            _maxabsmoment = _fixedendmoment.MaxAbs;

            _minmoment = _fixedendmoment.Min;

            _fixedendforce = _fixedendmoment.Derivate();

            WritePPolytoConsole(_name + " : Fixed End Force", _fixedendforce);

            _maxforce = _fixedendforce.Max;

            _maxabsforce = _fixedendforce.MaxAbs;

            _minforce = _fixedendforce.Min;

            if (_stressanalysis)
            {
                updatestresses();
            }
            //updatedeflection();
        }

        /// <summary>
        /// Updates the fixed end moment after cross loop.
        /// </summary>
        private void updatemoments()
        {
            var polylist = new List<Poly>();

            double constant = 0;

            if (_zeromoment.Length > 0)
            {
                //Cross to normal convention sign conversion
                if (Deflection(0.001) < 0)
                {
                    if (_ma > 0)
                    {
                        _ma = Negative(_ma);
                    }
                    else
                    {
                        _ma = Positive(_ma);
                    }
                }
                else
                {
                    if (_ma > 0)
                    {
                        _ma = Negative(_ma);
                    }
                    else
                    {
                        _ma = Positive(_ma);
                    }
                }

                if (Deflection(_length - 0.001) < 0)
                {
                    if (_mb > 0)
                    {
                        _mb = Positive(_mb);
                    }
                    else
                    {
                        _mb = Negative(_mb);
                    }
                }
                else
                {
                    if (_mb > 0)
                    {
                        _mb = Positive(_mb);
                    }
                    else
                    {
                        _mb = Negative(_mb);
                    }
                }

                constant = (_mb - _ma) / _length;

                foreach (Poly moment in _zeromoment)
                {
                    var poly = new Poly();
                    var poly1 = new Poly(_ma.ToString());
                    poly1.StartPoint = moment.StartPoint;
                    poly1.EndPoint = moment.EndPoint;
                    var poly2 = new Poly("x");
                    poly2.StartPoint = moment.StartPoint;
                    poly2.EndPoint = moment.EndPoint;

                    if (constant != 0)
                    {
                        if (Math.Abs(constant) < 0.0001)
                        {
                            poly = moment + poly1;
                        }
                        else
                        {
                            var poly3 = new Poly(constant.ToString());
                            poly = moment + poly1 + poly2 * poly3;
                        }
                    }
                    else
                    {
                        poly = moment + poly1;
                    }
                    poly.StartPoint = moment.StartPoint;
                    poly.EndPoint = moment.EndPoint;
                    polylist.Add(poly);
                }
            }
            else
            {
                //There is no load on this beam
                if (_ma > 0)
                {
                    _ma = Negative(_ma);
                }
                else
                {
                    _ma = Positive(_ma);
                }

                if (_mb > 0)
                {
                    _mb = Positive(_mb);
                }
                else
                {
                    _mb = Negative(_mb);
                }

                constant = (_mb - _ma) / _length;

                if (Math.Abs(constant) < 0.000001)
                {
                    constant = 0.0;
                }

                var poly = new Poly();
                var poly1 = new Poly(_ma.ToString());
                poly1.StartPoint = 0;
                poly1.EndPoint = _length;
                var poly2 = new Poly("x");
                poly2.StartPoint = 0;
                poly2.EndPoint = _length;

                if (constant != 0)
                {
                    var poly3 = new Poly(constant.ToString());
                    poly = poly1 + poly2 * poly3;
                }
                else
                {
                    poly = poly1;
                }

                poly.StartPoint = 0;
                poly.EndPoint = _length;
                polylist.Add(poly);
            }

            _fixedendmoment = new PiecewisePoly(polylist);

            WritePPolytoConsole(_name + " : Fixed End Moment", _fixedendmoment);

            _maxmoment = _fixedendmoment.Max;

            _maxabsmoment = _fixedendmoment.MaxAbs;

            _minmoment = _fixedendmoment.Min;
        }

        /// <summary>
        /// Calculates final force distribution after cross loop.
        /// </summary>
        private void updateforces()
        {
            _fixedendforce = _fixedendmoment.Derivate();

            WritePPolytoConsole(_name + " : Fixed End Force", _fixedendforce);

            _maxforce = _fixedendforce.Max;

            _maxabsforce = _fixedendforce.MaxAbs;

            _minforce = _fixedendforce.Min;
        }

        /// <summary>
        /// Calculates stress distribution after cross loop.
        /// </summary>
        private void updatestresses()
        {
            double precision = 0.001;
            _stress = new KeyValueCollection();
            double stress = 0;
            double y = 0;
            double e = 0;
            double d = 0;

            for (int i = 0; i < _length / precision; i++)
            {
                e = _e.Calculate(i * precision);
                d = _d.Calculate(i * precision);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = Math.Pow(10, 3) * _fixedendmoment.Calculate(i * precision) * y / (_inertiappoly.Calculate(i * precision));
                _stress.Add(i * precision, stress);
            }

            if (!_stress.ContainsKey(_length))
            {
                e = _e.Calculate(_length);
                d = _d.Calculate(_length);
                if (e > d - e)
                {
                    y = e;
                }
                else
                {
                    y = d - e;
                }
                stress = Math.Pow(10, 3) * _fixedendmoment.Calculate(_length) * y / (_inertiappoly.Calculate(_length));
                _stress.Add(_length, stress);
            }

            _maxstress = _stress.YMax;
            _maxabsstress = _stress.YMaxAbs;
        }

        private void updatedeflection()
        {
            double precision = 0.001;
            var function = new List<Func>();

            Func value;
            value.id = 0;
            value.xposition = 0;
            value.yposition = 0;

            int id = 0;
            for (int i = 0; i < _length / precision; i++)
            {
                value.id = id++;
                value.xposition = i * precision;
                value.yposition = -_fixedendmoment.Calculate(i * precision) / (_elasticity * _inertiappoly.Calculate(i * precision));
                function.Add(value);
            }

            var angle = TrapezeIntegrator.Integrate(function, precision);

            _deflection = TrapezeIntegrator.Integrate(angle, precision);

            _maxdeflection = _deflection.MaxBy(x => x.yposition);

            if (_maxdeflection.yposition > Global.MaxDeflection)
            {
                Global.MaxDeflection = _maxdeflection.yposition;
            }
        }

        #endregion

        #region ReDraw Functions

        public void ReDrawMoment(int c)
        {
            if (_femoment != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing moment for c = " + c);
                _femoment.Draw(c);
            }
            else if (_fixedendmoment?.Count > 0)
            {
                ShowFixedEndMomentDiagram(c);
            }
        }

        public void ReDrawDistLoad(int c)
        {
            if (_distload != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing distributed load for c = " + c);
                _distload.Draw(c);
            }
            else if (_distributedloads?.Count > 0)
            {
                ShowDistLoadDiagram(c);
            }
        }

        public void ReDrawConcLoad(int c)
        {
            if (_concload != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing concentrated load for c = " + c);
                _concload.Draw(c);
            }
            else if (_concentratedloads?.Count > 0)
            {
                ShowConcLoadDiagram(c);
            }
        }

        public void ReDrawInertia(int c)
        {
            if (_inertia != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing inertia for c = " + c);
                _inertia.Draw(c);
            }
            else if (_inertiappoly?.Count > 0)
            {
                ShowInertiaDiagram(c);
            }
        }

        public void ReDrawForce(int c)
        {
            if (_feforce != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing force for c = " + c);
                _feforce.Draw(c);
            }
            else if (_fixedendforce?.Count > 0)
            {
                ShowFixedEndForceDiagram(c);
            }
        }

        public void ReDrawStress(int c)
        {
            if (_stressdiagram != null)
            {
                MesnetDebug.WriteInformation(_name + " : redrawing stress for c = " + c);
                _stressdiagram.Draw(c);
            }
            else if (_stress?.Count > 0)
            {
                ShowStressDiagram(c);
            }
        }

        #endregion

        #endregion

        #endregion

        #region properties

        #region ui

        public int Id
        {
            get { return _id; }
            set { _id = value; }
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

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                Width = value * 100 + 14;
            }
        }

        public int BeamId
        {
            get { return _beamid; }
            set { _beamid = value; }
        }

        public double ElasticityModulus
        {
            get { return _elasticity; }
            set { _elasticity = value; }
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
            get
            {
                _leftpos = Canvas.GetLeft(this);
                return _leftpos;
            }
            set
            {
                _leftpos = value;
                Canvas.SetLeft(this, _leftpos);
            }
        }

        public double TopPos
        {
            get
            {
                _toppos = Canvas.GetTop(this);
                return _toppos;
            }
            set
            {
                _toppos = value;
                Canvas.SetTop(this, _toppos);
            }
        }

        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
            set
            {
                var transform = value;
                _angle = transform.Angle;
                rotateTransform = transform;
            }
        }

        public TransformGeometry TGeometry
        {
            get { return _tgeometry; }
        }

        public double IZero
        {
            get { return _izero; }
            set { _izero = value; }
        }

        public PiecewisePoly Loads
        {
            get { return _loads; }
        }

        public KeyValueCollection ConcentratedLoads
        {
            get { return _concentratedloads; }
        }

        public PiecewisePoly DistributedLoads
        {
            get { return _distributedloads; }
        }

        public PiecewisePoly Inertias
        {
            get { return _inertiappoly; }
        }

        public PiecewisePoly ZeroForce
        {
            get { return _zeroforce; }
        }

        public PiecewisePoly ZeroMoment
        {
            get { return _zeromoment; }
        }

        public PiecewisePoly FixedEndMoment
        {
            get { return _fixedendmoment; }
        }

        public PiecewisePoly FixedEndForce
        {
            get { return _fixedendforce; }
        }

        public KeyValueCollection Stress
        {
            get { return _stress; }
        }

        public PiecewisePoly E
        {
            get { return _e; }
            set { _e = value; }
        }

        public PiecewisePoly D
        {
            get { return _d; }
            set { _d = value; }
        }

        public bool PerformStressAnalysis
        {
            get { return _stressanalysis; }
            set { _stressanalysis = value; }
        }

        public double Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public Point LeftPoint
        {
            get { return _tgeometry.LeftPoint; }
        }

        public Point RightPoint
        {
            get { return _tgeometry.RightPoint; }
        }

        public double LeftEndMoment
        {
            get { return _ma; }
            set { _ma = value; }
        }

        public double RightEndMoment
        {
            get { return _mb; }
            set { _mb = value; }
        }

        public bool IndexPassed
        {
            get { return _indexpassed; }
            set { _indexpassed = value; }
        }

        public bool IsBound
        {
            get { return _isbound; }
            set { _isbound = value; }
        }

        public Func MaxDeflection
        {
            get { return _maxdeflection; }
        }

        public double MaxAllowableStress
        {
            get { return _maxallowablestress; }
            set { _maxallowablestress = value; }
        }

        public bool IsLeftSelected
        {
            get
            {
                return _leftcircleseleted;
            }
        }

        public bool IsRightSelected
        {
            get
            {
                return _rightcircleselected;
            }
        }

        public bool DirectionShown
        {
            get { return _directionshown; }
            set { _directionshown = value; }
        }

        #endregion

        #region Cross Properties

        public double CarryOverAB
        {
            get
            {
                return Math.Round(_gamaab, 4);
            }
        }

        public double CarryOverBA
        {
            get { return Math.Round(_gamaba, 4); }
        }

        public double StiffnessA
        {
            get { return _stiffnessa; }
        }

        public double StiffnessB
        {
            get { return _stiffnessb; }
        }

        public double MaxMoment
        {
            get { return _maxmoment; }
        }

        public double MaxAbsMoment
        {
            get { return _maxabsmoment; }
        }

        public double MinMoment
        {
            get { return _minmoment; }
        }

        public double MaxForce
        {
            get { return _maxforce; }
        }

        public double MaxAbsForce
        {
            get { return _maxabsforce; }
        }

        public double MinForce
        {
            get { return _minforce; }
        }

        public double MaxInertia
        {
            get { return _maxinertia; }
        }

        public double MaxStress
        {
            get { return _maxstress; }
        }

        public double MaxAbsStress
        {
            get { return _maxabsstress; }
        }

        public double MaxConcLoad
        {
            get { return _maxconcload; }
        }

        public double MaxAbsConcLoad
        {
            get { return _maxabsconcload; }
        }

        public double MaxDistLoad
        {
            get { return _maxdistload; }
        }

        public double MaxAbsDistLoad
        {
            get { return _maxabsdistload; }
        }
        #endregion

        #endregion

    }
}

