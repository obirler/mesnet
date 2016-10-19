using System;
using System.Collections.Generic;
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

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for NewBeam.xaml
    /// </summary>
    public partial class Beam : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Beam"/> with specified length. Depre
        /// </summary>
        /// <param name="length">The length in meters.</param>
        /// <param name="canbedragged">Sets whether the beam can be dragged in the canvas.</param>
        public Beam(double length, bool canbedragged = true)
        {
            InitializeComponent();

            InitializeVariables(length, canbedragged);
        }

        public Beam(Canvas canvas, double length, bool canbedragged = true)
        {
            _canvas = canvas;

            InitializeComponent();

            InitializeVariables(length, canbedragged);

            AddTopLeft(_canvas, 10000, 10000);
        }

        private void InitializeVariables(double length, bool canbedragged = true)
        {
            _length = length;
            contentgrid.Width = length * 100;
            Width = contentgrid.Width;
            contentgrid.Height = 14;
            Height = contentgrid.Height;
            _canbedragged = canbedragged;
            LeftSide = new List<int>();
            RightSide = new List<int>();
            beampoint.X = Canvas.GetLeft(this);
            beampoint.Y = Canvas.GetTop(this);
            MyDebug.WriteInformation(this.Name, "Beam has been added : length = " + _length + " m, Width = " + Width + " Height = " + Height);
            MyDebug.WriteInformation(this.Name, "Width = " + Width + " core width = " + core.Width);

            _concentratedloads = new List<KeyValuePair<double, double>>();

            _distributedloads = new PiecewisePoly();

            _zeroforceconc = new PiecewisePoly();

            _zeroforceconc = new PiecewisePoly();

            RightSide = null;

            LeftSide = null;
        }

        private int _id;

        private bool selected;

        private double _length;

        private string _name = null;

        private double _izero;

        private double _elasticity;

        private bool _canbedragged;

        private List<KeyValuePair<double, double>> _concentratedloads;

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

        private DotCollection _stress;

        private PiecewisePoly _e;

        private PiecewisePoly _d;

        public object LeftSide;

        public object RightSide;

        public double LeftDistributionFactor;

        public double RightDistributionFactor;

        public Direction circledirection;

        private Canvas _canvas;

        private Point corepoint;

        private Point beampoint;

        private ConcentratedLoad _concload;

        private DistributedLoad _distload;

        private Force _force;

        private Moment _moment;

        private Force _feforce;

        private Moment _femoment;

        private Inertia _inertia;

        private Deflection _deflectiondigram;

        private Stress _stressdiagram;

        /// <summary>
        /// The left support force of the beam.
        /// </summary>
        private double _leftsupportforcedist;

        private double _leftsupportforceconc;

        //private double _leftsupportforce;

        /// <summary>
        /// The right support force of the beam.
        /// </summary>
        private double _rightsupportforcedist;

        private double _rightsupportforceconc;

        //private double _rightsupportforce;

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

        private double _minmoment;

        private TransformGeometry _outertgeometry;

        private TransformGeometry _innertgeometry;

        private bool _leftcircleseleted;

        private bool _rightcircleselected;

        private bool _indexpassed;

        private bool _isbound = false;

        private bool _stressanalysis = false;

        private List<Func> _deflection;

        private Func _maxdeflection;

        private double _maxallowablestress;

        #region methods

        public void Add(Canvas canvas)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                _id = AddObject(this);
                BeamCount++;
                _name = "Beam " + BeamCount;
                Canvas.SetZIndex(this, 1);
            }
            else
            {
                MyDebug.WriteWarning(this.Name + " : Add", "This beam has already been added to canvas!");
            }
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
                _name = "Beam " + BeamCount;

                SetTransformGeometry(canvas);
                SetAngleCenter(0);
            }
            else
            {
                MyDebug.WriteWarning(this.Name + " : Add", "This beam has already been added to canvas!");
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
                _name = "Beam " + BeamCount;

                SetTransformGeometry(canvas);
            }
            else
            {
                MyDebug.WriteWarning(this.Name + " : Add", "This beam has already been added to canvas!");
            }
        }

        /// <summary>
        /// (DEPRECATED) Adds the beam in specified canvas on specified point and specified direction. Direction is important when the beam is assembling
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        /// <param name="direction">The direction based on the beam that is assembling.</param>
        public void Add(Canvas canvas, Point point, Direction direction)
        {
            if (!canvas.Children.Contains(this))
            {
                _canvas = canvas;
                canvas.Children.Add(this);
                _id = AddObject(this);

                var x = point.X;
                var y = point.Y;

                Canvas.SetZIndex(this, 1);

                switch (direction)
                {
                    //The beam that is supposed to be added will be added left side of another beam.
                    case Direction.Left:

                        Canvas.SetLeft(this, x - _length * 100);

                        Canvas.SetTop(this, y - 7);

                        break;

                    //The beam that is supposed to be added will be added right side of another beam.
                    case Direction.Right:

                        Canvas.SetLeft(this, x);

                        Canvas.SetTop(this, y - 7);

                        break;
                }

                BeamCount++;
                _name = "Beam " + BeamCount;

                SetTransformGeometry(canvas);
            }
            else
            {
                MyDebug.WriteWarning(this.Name + " : Add", "This beam has already been added to canvas!");
            }
        }

        public void Remove()
        {
            objects.Remove(this);
            _canvas.Children.Remove(this);
        }

        /// <summary>
        /// Connects the direction1 of the beam to the direction2 of the oldbeam.
        /// </summary>
        /// <param name="direction1">The direction of the beam to be connected.</param>
        /// <param name="oldbeam">The beam that this beam will be connected to.</param>
        /// <param name="direction2">The direction of the beam that this beam will be connected to.</param>
        /// <param name="manueloverride">if set to <c>true</c> [manueloverride] does not move the beam when it is connected.</param>        
        public void Connect(Direction direction1, Beam oldbeam, Direction direction2, bool manueloverride = false)
        {
            if (!manueloverride)
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
                                if (oldbeam.LeftSide != null)
                                {
                                    if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        if (oldbeam.IsBound)
                                        {
                                            //We will move this beam
                                            Canvas.SetLeft(this, oldbeam.LeftPoint.X);

                                            Canvas.SetTop(this, oldbeam.LeftPoint.Y - 7);

                                            oldbeam.SetTransformGeometry(_canvas);

                                            oldbeam.MoveSupports();
                                        }
                                        else if (this._isbound)
                                        {
                                            //We will move the old beam
                                            Canvas.SetLeft(oldbeam, LeftPoint.X);

                                            Canvas.SetTop(oldbeam, LeftPoint.Y - 7);

                                            SetTransformGeometry(_canvas);

                                            MoveSupports();
                                        }

                                        switch (oldbeam.LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.LeftSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.LeftSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Left);

                                                break;

                                            case "RightFixedSupport":

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
                                    if (LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.LeftPoint.X);

                                        Canvas.SetTop(this, oldbeam.LeftPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = LeftSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Left);

                                                Canvas.SetLeft(ss, LeftPoint.X - 13);

                                                Canvas.SetTop(ss, LeftPoint.Y);

                                                break;

                                            case "BasicSupport":

                                                var bs = LeftSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Left);

                                                Canvas.SetLeft(bs, LeftPoint.X - 13);

                                                Canvas.SetTop(bs, LeftPoint.Y);

                                                break;

                                            case "RightFixedSupport":

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

                                break;

                            #endregion

                            #region Left-Right

                            case Direction.Right:

                                if (LeftSide != null && oldbeam.RightSide != null)
                                {
                                    throw new InvalidOperationException("Both beam has supports on the assembly points");
                                }
                                //Left side of this beam will be connected to the right side of lodbeam.
                                if (oldbeam.RightSide != null)
                                {
                                    if (oldbeam.RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.RightPoint.X);

                                        Canvas.SetTop(this, oldbeam.RightPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (oldbeam.RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.RightSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.RightSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Left);

                                                break;

                                            case "LeftFixedSupport":

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
                                else if (LeftSide != null)
                                {
                                    if (LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.RightPoint.X);

                                        Canvas.SetTop(this, oldbeam.RightPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = LeftSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Right);

                                                Canvas.SetLeft(ss, LeftPoint.X - 13);

                                                Canvas.SetTop(ss, LeftPoint.Y);

                                                break;

                                            case "BasicSupport":

                                                var bs = LeftSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Right);

                                                Canvas.SetLeft(bs, LeftPoint.X - 13);

                                                Canvas.SetTop(bs, LeftPoint.Y);

                                                break;

                                            case "RightFixedSupport":

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
                                //Right side of this beam will be connected to the left side of lodbeam.
                                if (oldbeam.LeftSide != null)
                                {
                                    if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.LeftPoint.X - _length * 100);

                                        Canvas.SetTop(this, oldbeam.LeftPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (oldbeam.LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.LeftSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.LeftSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Right);

                                                break;

                                            case "RightFixedSupport":

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
                                    if (RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.LeftPoint.X - _length * 100);

                                        Canvas.SetTop(this, oldbeam.LeftPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = RightSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Left);

                                                Canvas.SetLeft(ss, RightPoint.X - 13);

                                                Canvas.SetTop(ss, RightPoint.Y);

                                                break;

                                            case "BasicSupport":

                                                var bs = RightSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Left);

                                                Canvas.SetLeft(bs, RightPoint.X - 13);

                                                Canvas.SetTop(bs, RightPoint.Y);

                                                break;

                                            case "LeftFixedSupport":

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

                                break;

                            #endregion

                            #region Right-Right

                            case Direction.Right:

                                if (RightSide != null && oldbeam.RightSide != null)
                                {
                                    throw new InvalidOperationException("Both beam has supports on the assembly points");
                                }
                                //Right side of this beam will be connected to the right side of lodbeam.
                                if (oldbeam.RightSide != null)
                                {
                                    if (oldbeam.RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.RightPoint.X - _length * 100);

                                        Canvas.SetTop(this, oldbeam.RightPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (oldbeam.RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.RightSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.RightSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Right);

                                                break;

                                            case "LeftFixedSupport":

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
                                    if (RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        Canvas.SetLeft(this, oldbeam.RightPoint.X - _length * 100);

                                        Canvas.SetTop(this, oldbeam.RightPoint.Y - 7);

                                        SetTransformGeometry(_canvas);

                                        switch (RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = RightSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = RightSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "LeftFixedSupport":

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

                                break;

                                #endregion
                        }

                        break;
                }
            }
            else
            {
                //A special case that allows connecting 2 connected beams

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
                                if (oldbeam.LeftSide != null)
                                {
                                    if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        switch (oldbeam.LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.LeftSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.LeftSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Left);

                                                break;

                                            case "RightFixedSupport":

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
                                    if (LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        switch (LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = LeftSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = LeftSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Left);

                                                break;

                                            case "RightFixedSupport":

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

                                break;

                            #endregion

                            #region Left-Right

                            case Direction.Right:

                                if (LeftSide != null && oldbeam.RightSide != null)
                                {
                                    throw new InvalidOperationException("Both beam has supports on the assembly points");
                                }
                                //Left side of this beam will be connected to the right side of lodbeam.
                                if (oldbeam.RightSide != null)
                                {
                                    if (oldbeam.RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        switch (oldbeam.RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.RightSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.RightSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Left);

                                                break;

                                            case "LeftFixedSupport":

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
                                else if (LeftSide != null)
                                {
                                    if (LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        switch (LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = LeftSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = LeftSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "RightFixedSupport":

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
                                //Right side of this beam will be connected to the left side of lodbeam.
                                if (oldbeam.LeftSide != null)
                                {
                                    if (oldbeam.LeftSide.GetType().Name != "LeftFixedSupport")
                                    {
                                        switch (oldbeam.LeftSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.LeftSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.LeftSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Right);

                                                break;

                                            case "RightFixedSupport":

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
                                    if (RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        switch (RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = RightSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Left);

                                                break;

                                            case "BasicSupport":

                                                var bs = RightSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Left);

                                                break;

                                            case "LeftFixedSupport":

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

                                break;

                            #endregion

                            #region Right-Right

                            case Direction.Right:

                                if (RightSide != null && oldbeam.RightSide != null)
                                {
                                    throw new InvalidOperationException("Both beam has supports on the assembly points");
                                }
                                //Right side of this beam will be connected to the right side of lodbeam.
                                if (oldbeam.RightSide != null)
                                {
                                    if (oldbeam.RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        switch (oldbeam.RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = oldbeam.RightSide as SlidingSupport;

                                                ss.AddBeam(this, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = oldbeam.RightSide as BasicSupport;

                                                bs.AddBeam(this, Direction.Right);

                                                break;

                                            case "LeftFixedSupport":

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
                                    if (RightSide.GetType().Name != "RightFixedSupport")
                                    {
                                        switch (RightSide.GetType().Name)
                                        {
                                            case "SlidingSupport":

                                                var ss = RightSide as SlidingSupport;

                                                ss.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "BasicSupport":

                                                var bs = RightSide as BasicSupport;

                                                bs.AddBeam(oldbeam, Direction.Right);

                                                break;

                                            case "LeftFixedSupport":

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

                                break;

                                #endregion
                        }

                        break;
                }
            }

            _isbound = true;
            oldbeam.IsBound = true;
        }

        /// <summary>
        /// Adds the distributed load to beam with specified direction.
        /// </summary>
        /// <param name="load">The desired distributed load.</param>
        /// <param name="direction">The direction of load. It can be Up or Down</param>
        public void AddLoad(DistributedLoad load, Direction direction)
        {
            if (direction == Direction.Up)
            {
                //upcanvas.Height = load.Height;
                load.Length = _length;
                upcanvas.Children.Add(load);
                load.VerticalAlignment = VerticalAlignment.Center;
                Canvas.SetTop(load, upcanvas.Height / 2 - load.Height);
                _distributedloads = load.LoadPpoly;

                _distload = load;
            }
            else if (direction == Direction.Down)
            { }
        }

        public void AddLoad(ConcentratedLoad load, Direction direction)
        {
            if (direction == Direction.Up)
            {
                //upcanvas.Height = load.Height;
                load.Length = _length;
                upcanvas.Children.Add(load);
                load.VerticalAlignment = VerticalAlignment.Center;
                Canvas.SetTop(load, upcanvas.Height / 2 - load.Height);

                _concentratedloads = load.Loads;

                _concload = load;
            }
            else if (direction == Direction.Down)
            { }
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
                upcanvas.Children.Remove(distload);
                _distributedloads.Clear();
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
                upcanvas.Children.Remove(concload);
                _concentratedloads.Clear();
                _concload = null;
            }
        }

        public void ShowDistLoad()
        {
            _distload.Visibility = Visibility.Visible;
        }

        public void HideDistLoad()
        {
            _distload.Visibility = Visibility.Hidden;
        }

        public void ShowConcLoad()
        {
            _concload.Visibility = Visibility.Visible;
        }

        public void HideConcLoad()
        {
            _concload.Visibility = Visibility.Hidden;
        }

        public void AddForceDiagram()
        {
            var force = new Force(_zeroforce, this);
            force.Length = _length;
            upcanvas.Children.Add(force);
            force.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(force, upcanvas.Height / 2 - force.Height);
            _force = force;
        }

        public void HideForceDiagram()
        {
            if (_force != null)
            {
                upcanvas.Children.Remove(_force);
                _force = null;
            }
        }

        public void AddMomentDiagram()
        {
            var moment = new Moment(_zeromoment, this);
            moment.Length = _length;
            upcanvas.Children.Add(moment);
            moment.VerticalAlignment = VerticalAlignment.Center;
            Canvas.SetTop(moment, upcanvas.Height / 2 - moment.Height);
            _moment = moment;
        }

        public void HideMomentDiagram()
        {
            if (_moment != null)
            {
                upcanvas.Children.Remove(_moment);
                _moment = null;
            }
        }

        public void AddFixedEndForceDiagram()
        {
            if (_feforce != null)
            {
                _feforce.Show();
            }
            else
            {
                var force = new Force(_fixedendforce, this);
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

        public void AddFixedEndMomentDiagram()
        {
            if (_femoment != null)
            {
                _femoment.Show();
            }
            else
            {
                var moment = new Moment(_fixedendmoment, this);
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

        public void AddInertiaDiagram()
        {
            if (_inertia != null)
            {
                _inertia.Show();
            }
            else
            {
                var inertia = new Inertia(_inertiappoly, _length);
                inertia.Length = _length;
                downcanvas.Children.Add(inertia);
                _inertia = inertia;
            }

        }

        public void HideInertiaDiagram()
        {
            if (_inertia != null)
            {
                downcanvas.Children.Remove(_inertia);
                _inertia = null;
            }
        }

        public void AddDeflectionDiagram()
        {
            if (_deflectiondigram != null)
            {
                _deflectiondigram.Show();
            }
            else
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

        public void AddStressDiagram()
        {
            if (_stressdiagram != null)
            {
                _stressdiagram.Show();
            }
            else
            {
                var stress = new Stress(_stress, this);
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

        public void ShowDirectionArrow()
        {
            directionarrow.Visibility = Visibility.Visible;
        }

        public void HideDirectionArrow()
        {
            directionarrow.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds inertia moment function.
        /// </summary>
        /// <param name="inertiappoly">The inertia Piecewise Polynomial.</param>
        public void AddInertia(PiecewisePoly inertiappoly)
        {
            _inertiappoly = inertiappoly;
            _izero = _inertiappoly.Min;
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
            core.Fill = new SolidColorBrush(Colors.Black);
            startcircle.Visibility = Visibility.Collapsed;
            startcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            endcircle.Visibility = Visibility.Collapsed;
            endcircle.Stroke = new SolidColorBrush(Color.FromArgb(255, 5, 118, 0));
            circledirection = Direction.None;
            selected = false;
            UnSelectCircle();
            _outertgeometry.HideCorners();
            MyDebug.WriteInformation(this.Name + " Unselect", "Beam unselected : left = " + Canvas.GetLeft(this) + " top = " + Canvas.GetTop(this));
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
        /// Sets the position of the beam based on top-right point of it.
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
                MyDebug.WriteWarning("Beam", "The beam to be dragged can not be dragged");
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
                MyDebug.WriteWarning("Beam", "The beam to be dragged can not be dragged");
            }
        }

        public void SetTransformGeometry(Canvas canvas)
        {
            _innertgeometry = new TransformGeometry(this, canvas);
            Point tl = new Point(_innertgeometry.TopLeft.X - 7, _innertgeometry.TopLeft.Y);
            Point tr = new Point(_innertgeometry.TopRight.X + 7, _innertgeometry.TopRight.Y);
            Point br = new Point(_innertgeometry.BottomRight.X + 7, _innertgeometry.BottomRight.Y);
            Point bl = new Point(_innertgeometry.BottomLeft.X - 7, _innertgeometry.BottomLeft.Y);
            _outertgeometry = new TransformGeometry(tl, tr, br, bl, canvas);
        }

        /// <summary>
        /// Changes the position of the beam by the given amount.
        /// </summary>
        /// <param name="dx">The change in x.</param>
        /// <param name="dy">The change in y.</param>
        public void Move(Vector delta)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + delta.X);
            Canvas.SetTop(this, Canvas.GetTop(this) + delta.Y);
            _innertgeometry.Move(delta);
            _outertgeometry.Move(delta);
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
            rotateTransform.CenterX = Width / 2;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = angle;
            _angle = angle;

            _innertgeometry.RotateAboutCenter(angle);
            _outertgeometry.RotateAboutCenter(angle);
        }

        /// <summary>
        /// Rotates the beam about its left point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleLeft(double angle)
        {
            rotateTransform.CenterX = 0;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = angle;
            _angle = angle;
            _innertgeometry.Rotate(new Point(Canvas.GetLeft(this), Canvas.GetTop(this) + this.Height / 2), angle);
            _outertgeometry.Rotate(new Point(Canvas.GetLeft(this), Canvas.GetTop(this) + this.Height / 2), angle);
        }

        /// <summary>
        /// Rotates the beam about its right point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAngleRight(double angle)
        {
            rotateTransform.CenterX = Width;
            rotateTransform.CenterY = Height / 2;
            rotateTransform.Angle = angle;
            _angle = angle;
            _innertgeometry.Rotate(new Point(Canvas.GetLeft(this) + this.Width, Canvas.GetTop(this) + this.Height / 2), angle);
            _outertgeometry.Rotate(new Point(Canvas.GetLeft(this) + this.Width, Canvas.GetTop(this) + this.Height / 2), angle);
        }

        public bool IsInside(Point point)
        {
            return _outertgeometry.IsInside(point);
        }

        public void ShowCorners(double radius)
        {
            _outertgeometry.ShowCorners(radius);
        }

        public void HideCorners()
        {
            _outertgeometry.HideCorners();
        }

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

        public void WriteToDebug()
        {

        }

        #region SoM

        /// <summary>
        /// Finds the support forces.
        /// </summary>
        private void finddistributedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_distributedloads.Count == 0)
            {
                _leftsupportforcedist = 0;
                _rightsupportforcedist = 0;
            }
            else
            {
                foreach (Poly load in _distributedloads)
                {
                    var magnitude = load.DefiniteIntegral(load.StartPoint, load.EndPoint);
                    var actdistance = load.LoadCenter(load.StartPoint, load.EndPoint);
                    resultantforce += magnitude;
                    multiply += magnitude * actdistance;
                }

                resultantforcedistance = multiply / resultantforce;

                _resultantforcedistance = resultantforcedistance;

                _resultantforce = resultantforce;

                _leftsupportforcedist = resultantforce * (_length - resultantforcedistance) / _length;
                _rightsupportforcedist = resultantforce * resultantforcedistance / _length;
            }

            MyDebug.WriteInformation(this.Name + " finddistributedsupportforces", "resultantforce = " + resultantforce + " resultantforcedistance = " + resultantforcedistance);

            MyDebug.WriteInformation(this.Name + " finddistributedsupportforces", "leftsupportforcedist = " + _leftsupportforcedist + " rightsupportforcedist = " + _leftsupportforcedist);

        }

        private void findconcentratedsupportforces()
        {
            double resultantforce = 0;
            double resultantforcedistance = 0;
            double multiply = 0;

            if (_concentratedloads.Count == 0)
            {
                _leftsupportforceconc = 0;
                _rightsupportforceconc = 0;
            }
            else
            {
                foreach (var item in _concentratedloads)
                {
                    resultantforce += item.Value;
                    multiply += item.Key * item.Value;
                }

                resultantforcedistance = multiply / resultantforce;

                _leftsupportforceconc = resultantforce * (_length - resultantforcedistance) / _length;

                _rightsupportforceconc = resultantforce * resultantforcedistance / _length;
            }

            MyDebug.WriteInformation(this.Name + " findconcentratedsupportforces", "resultantforcedistance = " + resultantforcedistance);

            MyDebug.WriteInformation(this.Name + " findconcentratedsupportforces", "leftsupportforceconc = " + _leftsupportforceconc + " rightsupportforceconc = " + _rightsupportforceconc);
        }

        #region Zero Condition

        private void findconcentratedzeroforce()
        {
            _zeroforceconc = new PiecewisePoly();

            if (_concentratedloads.Count != 0)
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
                        if (i + 1 < _length)
                        {
                            poly.EndPoint = _length;
                        }
                        else
                        {
                            break;
                        }
                    }

                    _zeroforceconc.Add(poly);
                }
                WritePPolytoConsole("_zeroforceconc", _zeroforceconc);
            }
        }

        /// <summary>
        /// Finds the zero force polynomial which is the force polynomial when there is no fixed support in the end of the beam.
        /// </summary>
        private void finddistributedzeroforce()
        {
            _zeroforcedist = new PiecewisePoly();

            if (_distributedloads.Count != 0)
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
                    _zeroforcedist.Sort();
                }

                if (_distributedloads.Last().EndPoint != _length)
                {
                    var weights = findforcebefore(_distributedloads.Count);
                    var ply = new Poly(weights.ToString());
                    ply.StartPoint = _distributedloads.Last().EndPoint;
                    ply.EndPoint = _length;
                    _zeroforcedist.Add(ply);
                }

                WritePPolytoConsole("_zeroforcedist", _zeroforcedist);
            }
        }

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

                MyDebug.WriteInformation(this.Name + " findzeromoment", "integration = " + integration.ToString() + " momentsbefore = " + momentsbefore + " zeroforcevalue = " + zerovalue + " startpoint = " + force.StartPoint + " endpoint = " + force.EndPoint);

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
                //var poly1 = new Poly("-1");
                //poly1.StartPoint = force.StartPoint;
                //poly1.EndPoint = force.EndPoint;
                //var momentpoly = poly * poly1;
                //momentpoly.StartPoint = force.StartPoint;
                //momentpoly.EndPoint = force.EndPoint;
                //_zeromoment.Add(momentpoly);
                _zeromoment.Add(poly);
                _zeromoment.Sort();
            }

            foreach (Poly pol in _zeromoment)
            {
                MyDebug.WriteInformation(this.Name + " findzeromoment", "zeromomentpoly[" + _zeromoment.IndexOf(pol) + "] = " + pol.ToString() + " , startpoint = " + pol.StartPoint + " , endpoint = " + pol.EndPoint);
                MyDebug.WriteInformation(this.Name + " findzeromoment", "definite force integral = " + _zeroforce.DefiniteIntegral(0, pol.EndPoint));
            }
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
            double ma1 = 0;
            double ma2 = 0;
            double mb1 = 0;
            double mb2 = 0;
            double r1 = 0;
            double r2 = 0;

            var polylist = new List<Poly>();

            foreach (Poly pol in _zeromoment)
            {
                var ply = -1 * pol;
                ply.StartPoint = pol.StartPoint;
                ply.EndPoint = pol.EndPoint;
                polylist.Add(ply);
            }

            var zeromoment = new PiecewisePoly(polylist);

            ///////////////////////////////////////////////////////////
            /////////////////Left Equation Solve///////////////////////
            //////////////////////////////////////////////////////////

            var xsquare = new Poly("x^2");
            xsquare.StartPoint = 0;
            xsquare.EndPoint = _length;

            var conjugateinertia = _inertiappoly.Conjugate(_length);

            var simpson1 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
            }

            simpson1.Calculate();

            ma1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

            MyDebug.WriteInformation(this.Name + " ffsolver", "ma1 = " + ma1);

            //////////////////////////////////////////////////////////

            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            var simpson2 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson2.AddData(_izero / conjugateinertia.Calculate(i) * x.Calculate(i));
            }

            simpson2.Calculate();

            var value1 = 1 / _length * simpson2.Result;

            mb1 = value1 - ma1;

            MyDebug.WriteInformation(this.Name + " ffsolver", "mb1 = " + mb1);

            ///////////////////////////////////////////////////////////

            var simpson3 = new SimpsonIntegrator(SimpsonStep);

            var conjugatemoment = zeromoment.Conjugate(_length);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) * x.Calculate(i));
            }

            simpson3.Calculate();

            r1 = -1 / _length * simpson3.Result;

            MyDebug.WriteInformation(this.Name + " ffsolver", "r1 = " + r1);

            ///////////////////////////////////////////////////////////
            /////////////////Right Equation Solve///////////////////////
            //////////////////////////////////////////////////////////

            var xsquareppoly = new PiecewisePoly();
            xsquareppoly.Add(xsquare);

            var simpson4 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                var iner = _izero / _inertiappoly.Calculate(i);
                var xsq = xsquareppoly.Calculate(i);
                simpson4.AddData(iner * xsq);
            }

            simpson4.Calculate();

            var value2 = 1 / Math.Pow(_length, 2) * simpson4.Result;

            var xppoly = new PiecewisePoly();
            xppoly.Add(x);

            var simpson5 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson5.AddData((_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
            }

            simpson5.Calculate();

            ma2 = 1 / _length * simpson5.Result - value2;

            MyDebug.WriteInformation(this.Name + " ffsolver", "ma2 = " + ma2);

            ///////////////////////////////////////////////////////////

            mb2 = value2;

            MyDebug.WriteInformation(this.Name + " ffsolver", "mb2 = " + mb2);

            ///////////////////////////////////////////////////////////

            var simpson6 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson6.AddData(zeromoment.Calculate(i) * (_izero / _inertiappoly.Calculate(i)) * xppoly.Calculate(i));
            }

            simpson6.Calculate();

            r2 = -1 / _length * simpson6.Result;

            MyDebug.WriteInformation(this.Name + " ffsolver", "r2 = " + r2);

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

            MyDebug.WriteInformation(this.Name + " ffsolver", "ma = " + _ma);
            MyDebug.WriteInformation(this.Name + " ffsolver", "mb = " + _mb);
        }

        /// <summary>
        /// Finds end moments in case of the left end has fixed support and the right and basic or sliding support.
        /// </summary>
        private void fbsolver()
        {
            double ma1 = 0;
            double ma2 = 0;
            double mb1 = 0;
            double mb2 = 0;
            double r1 = 0;
            double r2 = 0;

            var polylist = new List<Poly>();

            foreach (Poly pol in _zeromoment)
            {
                var ply = -1 * pol;
                ply.StartPoint = pol.StartPoint;
                ply.EndPoint = pol.EndPoint;
                polylist.Add(ply);
            }

            var zeromoment = new PiecewisePoly(polylist);

            //////////////////////////////////////////////////////////

            var xsquare = new Poly("x^2");
            xsquare.StartPoint = 0;
            xsquare.EndPoint = _length;

            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            var conjugateinertia = _inertiappoly.Conjugate(_length);

            var simpson1 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson1.AddData(_izero / conjugateinertia.Calculate(i) * xsquare.Calculate(i));
            }

            simpson1.Calculate();

            ma1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

            MyDebug.WriteInformation(this.Name + " fbsolver", "ma1 = " + ma1);

            //////////////////////////////////////////////////////////

            var simpson3 = new SimpsonIntegrator(SimpsonStep);

            var conjugatemoment = zeromoment.Conjugate(_length);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson3.AddData(conjugatemoment.Calculate(i) * _izero / conjugateinertia.Calculate(i) * x.Calculate(i));
            }

            simpson3.Calculate();

            r1 = -1 / _length * simpson3.Result;

            MyDebug.WriteInformation(this.Name + " fbsolver", "r1 = " + r1);

            ///////////////////////////////////////////////////////////

            _ma = -Math.Round(r1 / ma1, 4);
            _mb = 0;
        }

        /// <summary>
        /// Finds end moments in case of the right end has fixed support and the left and basic or sliding support.
        /// </summary>
        private void bfsolver()
        {
            double ma1 = 0;
            double ma2 = 0;
            double mb1 = 0;
            double mb2 = 0;
            double r1 = 0;
            double r2 = 0;

            var polylist = new List<Poly>();

            foreach (Poly pol in _zeromoment)
            {
                var ply = -1 * pol;
                ply.StartPoint = pol.StartPoint;
                ply.EndPoint = pol.EndPoint;
                polylist.Add(ply);
            }

            var zeromoment = new PiecewisePoly(polylist);

            //////////////////////////////////////////////////////////

            var xsquare = new Poly("x^2");
            xsquare.StartPoint = 0;
            xsquare.EndPoint = _length;

            var conjugateinertia = _inertiappoly.Conjugate(_length);

            var simpson1 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson1.AddData(xsquare.Calculate(i) / _inertiappoly.Calculate(i));
            }

            simpson1.Calculate();

            mb1 = 1 / Math.Pow(_length, 2) * simpson1.Result;

            MyDebug.WriteInformation(this.Name + " bfsolver", "mb1 = " + mb1);

            ///////////////////////////////////////////////////////////

            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            var simpson3 = new SimpsonIntegrator(SimpsonStep);

            for (double i = 0; i <= _length; i = i + SimpsonStep)
            {
                simpson3.AddData(zeromoment.Calculate(i) * x.Calculate(i) / _inertiappoly.Calculate(i));
            }

            simpson3.Calculate();

            r1 = -1 / _length * simpson3.Result;

            MyDebug.WriteInformation(this.Name + " bfsolver", "r1 = " + r1);

            _mb = -r1 / mb1;  //Math.Round(r1 / mb1, 4);
            _ma = 0;
        }

        /// <summary>
        /// Finds end moments in case of both end have basic or sliding support.
        /// </summary>
        private void bbsolver()
        {
            _mb = 0;
            _ma = 0;
        }

        /// <summary>
        /// Finds end moments according to support types on the ends.
        /// </summary>
        private void findfixedendmoment()
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
                var poly1 = new Poly(_ma.ToString());
                poly1.StartPoint = moment.StartPoint;
                poly1.EndPoint = moment.EndPoint;
                var poly2 = new Poly("x");
                poly2.StartPoint = moment.StartPoint;
                poly2.EndPoint = moment.EndPoint;
                if (constant != 0)
                {
                    var poly3 = new Poly(constant.ToString());
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
        }

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
            MyDebug.WriteInformation(this.Name + " : ClapeyronCalculate", "ClapeyronCalculate has started to work");
            findconcentratedsupportforces();
            finddistributedsupportforces();
            findconcentratedzeroforce();
            finddistributedzeroforce();
            _zeroforce = _zeroforceconc + _zeroforcedist;
            WritePPolytoConsole(this.Name + " : _zeroforce", _zeroforce);
            findzeromoment();

            clapeyronsupportcase();

            findfixedendmoment();

            updateclapeyronmoments();
        }

        /// <summary>
        /// Calculates moments when there is only one beam presented.
        /// </summary>
        private void clapeyronsupportcase()
        {
            #region cross support cases

            switch (LeftSide.GetType().Name)
            {
                case "LeftFixedSupport":

                    switch (RightSide.GetType().Name)
                    {
                        case "RightFixedSupport":

                            MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "ffsolver has been executed");
                            ffsolver();

                            break;

                        case "BasicSupport":

                            var basic = RightSide as BasicSupport;

                            MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "fbsolver has been executed");
                            fbsolver();

                            break;

                        case "SlidingSupport":

                            var sliding = RightSide as SlidingSupport;

                            MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "fbsolver has been executed");
                            fbsolver();

                            break;
                    }

                    break;

                case "BasicSupport":

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "ffsolver has been executed");
                                ffsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "fbsolver has been executed");
                                fbsolver();

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                fbsolver();

                                break;
                        }
                    }
                    else
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bfsolver has been executed");
                                bfsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                bbsolver();

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                bbsolver();

                                break;
                        }
                    }

                    break;

                case "SlidingSupport":

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "ffsolver has been executed");
                                ffsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "fbsolver has been executed");
                                fbsolver();

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                fbsolver();

                                break;
                        }
                    }
                    else
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bfsolver has been executed");
                                bfsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                bbsolver();

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                MyDebug.WriteInformation(this.Name + ": ClapeyronCalculate", "bbsolver has been executed");
                                bbsolver();

                                break;
                        }
                    }

                    break;
            }

            #endregion
        }

        private void updateclapeyronmoments()
        {
            var polylist = new List<Poly>();

            double constant = 0;

            if (_zeromoment.Length > 0)
            {
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
                        if (Math.Abs(constant) < 0.000001)
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

            _maxmoment = _fixedendmoment.Max;

            _minmoment = _fixedendmoment.Min;

            _fixedendforce = _fixedendmoment.Derivate();
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

                    switch (RightSide.GetType().Name)
                    {
                        case "BasicSupport":

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

                        case "SlidingSupport":

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

                        case "RightFixedSupport":

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

                    switch (LeftSide.GetType().Name)
                    {
                        case "BasicSupport":

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

                        case "SlidingSupport":

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

                        case "LeftFixedSupport":

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

        private void findcrosscoefficients()
        {
            var simpson1 = new SimpsonIntegrator(0.0001);

            var lxpoly = new Poly(_length.ToString() + "-x");
            lxpoly.StartPoint = 0;
            lxpoly.EndPoint = _length;

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                simpson1.AddData(Math.Pow(lxpoly.Calculate(i), 2) / _inertiappoly.Calculate(i));
            }

            simpson1.Calculate();

            _alfaa = _izero / Math.Pow(_length, 3) * simpson1.Result;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "alfaa = " + _alfaa);

            Logger.WriteLine(this.Name + " : alfaa = " + _alfaa);

            var simpson2 = new SimpsonIntegrator(0.0001);

            var xsquare = new Poly("x^2");
            xsquare.StartPoint = 0;
            xsquare.EndPoint = _length;

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                simpson2.AddData(xsquare.Calculate(i) / _inertiappoly.Calculate(i));
            }

            simpson2.Calculate();

            _alfab = _izero / Math.Pow(_length, 3) * simpson2.Result;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "alfab = " + _alfab);

            Logger.WriteLine(this.Name + " : alfab = " + _alfab);

            var simpson3 = new SimpsonIntegrator(0.0001);

            var x = new Poly("x");
            x.StartPoint = 0;
            x.EndPoint = _length;

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                simpson3.AddData((lxpoly.Calculate(i) * x.Calculate(i)) / _inertiappoly.Calculate(i));
            }

            simpson3.Calculate();

            _beta = _izero / Math.Pow(_length, 3) * simpson3.Result;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "beta = " + _beta);

            Logger.WriteLine(this.Name + " : beta = " + _beta);

            var simpson4 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                simpson4.AddData((_zeromoment.Calculate(i) * x.Calculate(i)) / _inertiappoly.Calculate(i));
            }

            simpson4.Calculate();

            _ka = 6 * _izero / Math.Pow(_length, 2) * simpson4.Result;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "ka = " + _ka);

            Logger.WriteLine(this.Name + " : ka = " + _ka);

            var simpson5 = new SimpsonIntegrator(0.0001);

            for (double i = 0; i <= _length; i = i + 0.0001)
            {
                simpson5.AddData((_zeromoment.Calculate(i) * lxpoly.Calculate(i)) / _inertiappoly.Calculate(i));
            }

            simpson5.Calculate();

            _kb = 6 * _izero / Math.Pow(_length, 2) * simpson5.Result;

            Logger.WriteLine(this.Name + " : kb = " + _kb);

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "kb = " + _kb);

            _fia = _length * (_kb / 6 + _mb * _beta + _ma * _alfaa) / (_elasticity * _izero);

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "fia = " + _fia);

            _fib = -_length * (_ka / 6 + _ma * _beta + _mb * _alfab) / (_elasticity * _izero);

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "fib = " + _fib);

            _gamaba = _beta / _alfaa;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "gamaba = " + _gamaba);

            _gamaab = _beta / _alfab;

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "gamaab = " + _gamaab);

            #region stiffnesses with support cases

            switch (LeftSide.GetType().Name)
            {
                case "LeftFixedSupport":

                    switch (RightSide.GetType().Name)
                    {
                        case "RightFixedSupport":

                            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 1");

                            _stiffnessa = 0;

                            _stiffnessb = 0;

                            break;

                        case "BasicSupport":


                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 2");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 3");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;

                        case "SlidingSupport":

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 4");

                                _stiffnessa = 0;

                                _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                            }
                            else
                            {
                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 5");

                                _stiffnessa = 0;

                                _stiffnessb = 0;
                            }

                            break;
                    }

                    break;

                case "BasicSupport":

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 6");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 7");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 8");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 9");

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
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 10");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 11");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 12");
                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 13");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 14");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;

                case "SlidingSupport":

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 15");

                                _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                _stiffnessb = 0;

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 16");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 17");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 18");

                                    _stiffnessa = _alfab / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;

                                    _stiffnessb = _alfaa / (_alfaa * _alfab - _beta * _beta) * _elasticity * _izero / _length;
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 19");

                                    _stiffnessa = _elasticity * _izero / (_length * _alfaa);

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 20");

                                _stiffnessa = 0;

                                _stiffnessb = 0;

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 21");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 22");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 23");

                                    _stiffnessa = 0;

                                    _stiffnessb = _elasticity * _izero / (_length * _alfab);
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "stiffness case 24");

                                    _stiffnessa = 0;

                                    _stiffnessb = 0;
                                }

                                break;
                        }
                    }

                    break;
            }

            _stiffnessa = Math.Round(_stiffnessa, 4);

            _stiffnessb = Math.Round(_stiffnessb, 4);

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "StiffnessA = " + _stiffnessa);

            Logger.WriteLine(this.Name + " : StiffnessA = " + _stiffnessa);

            MyDebug.WriteInformation(this.Name + " findcrosscoefficients", "StiffnessB = " + _stiffnessb);

            Logger.WriteLine(this.Name + " : StiffnessB = " + _stiffnessb);
            #endregion
        }

        private void crosssupportcases()
        {
            #region cross support cases

            switch (LeftSide.GetType().Name)
            {
                case "LeftFixedSupport":

                    switch (RightSide.GetType().Name)
                    {
                        case "RightFixedSupport":

                            MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                            ffsolver();

                            break;

                        case "BasicSupport":

                            var basic = RightSide as BasicSupport;

                            if (basic.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "fbsolver has been executed");
                                fbsolver();
                            }

                            break;

                        case "SlidingSupport":

                            var sliding = RightSide as SlidingSupport;

                            if (sliding.Members.Count > 1)
                            {
                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                ffsolver();
                            }
                            else
                            {
                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "fbsolver has been executed");
                                fbsolver();
                            }

                            break;
                    }

                    break;

                case "BasicSupport":

                    var basic1 = LeftSide as BasicSupport;

                    if (basic1.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                ffsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                bfsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;

                case "SlidingSupport":

                    var sliding2 = LeftSide as SlidingSupport;

                    if (sliding2.Members.Count > 1)
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                ffsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "fbsolver has been executed");
                                    fbsolver();
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "ffsolver has been executed");
                                    ffsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    fbsolver();
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (RightSide.GetType().Name)
                        {
                            case "RightFixedSupport":

                                MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                bfsolver();

                                break;

                            case "BasicSupport":

                                var basic3 = RightSide as BasicSupport;

                                if (basic3.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    bbsolver();
                                }

                                break;

                            case "SlidingSupport":

                                var sliding1 = RightSide as SlidingSupport;

                                if (sliding1.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bfsolver has been executed");
                                    bfsolver();
                                }
                                else
                                {
                                    MyDebug.WriteInformation(this.Name + ": CrossCalculate", "bbsolver has been executed");
                                    bbsolver();
                                }

                                break;
                        }
                    }

                    break;
            }

            #endregion
        }

        public void CrossCalculate()
        {
            MyDebug.WriteInformation(this.Name + " CrossCalculate", "CrossCalculate has started to work");
            findconcentratedsupportforces();
            finddistributedsupportforces();
            findconcentratedzeroforce();
            finddistributedzeroforce();
            _zeroforce = _zeroforceconc + _zeroforcedist;
            WritePPolytoConsole(this.Name + " : _zeroforce", _zeroforce);
            findzeromoment();

            crosssupportcases();

            findfixedendmoment();

            findcrosscoefficients();

            _maclapeyron = _ma;

            MyDebug.WriteInformation(this.Name + ": CrossCalculate", "Clapeyron Ma = " + _maclapeyron);

            Logger.WriteLine(this.Name + " : Clapeyron Ma = " + _maclapeyron);

            _mbclapeyron = _mb;

            MyDebug.WriteInformation(this.Name + ": CrossCalculate", "Clapeyron Mb = " + _mbclapeyron);

            Logger.WriteLine(this.Name + " : Clapeyron Mb = " + _mbclapeyron);

            if (Deflection(0.001) < 0)
            {
                _ma = Negative(_ma);
            }
            else
            {
                _ma = Positive(_ma);
            }

            MyDebug.WriteInformation(this.Name + ": CrossCalculate", "Ma = " + _ma);

            Logger.WriteLine(this.Name + " : Cross Ma = " + _ma);

            if (Deflection(_length - 0.001) < 0)
            {
                _mb = Positive(_mb);
            }
            else
            {
                _mb = Negative(_mb);
            }

            Logger.WriteLine(this.Name + " : Cross Mb = " + _mb);

            Logger.NextLine();

            MyDebug.WriteInformation(this.Name + ": CrossCalculate", "Mb = " + _mb);

            MyDebug.WriteInformation(this.Name + ": CrossCalculate", ": CrossCalculate has finished to work");
        }

        #endregion

        #region post-cross

        /// <summary>
        /// Updates the fixed end moment after cross loop.
        /// </summary>
        public void UpdateMoments()
        {
            var polylist = new List<Poly>();

            double constant = 0;

            if (_zeromoment.Length > 0)
            {
                if (Deflection(0.001) < 0)
                {
                    if (_ma > 0)
                    {
                        _ma = Positive(_ma);
                    }
                    else
                    {
                        _ma = Negative(_ma);
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
                        _mb = Negative(_mb);
                    }
                    else
                    {
                        _mb = Positive(_mb);
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
                        if (Math.Abs(constant) < 0.000001)
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
                    _ma = Positive(_ma);
                }
                else
                {
                    _ma = Negative(_ma);
                }

                if (_mb > 0)
                {
                    _mb = Negative(_mb);
                }
                else
                {
                    _mb = Positive(_mb);
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

            _maxmoment = _fixedendmoment.Max;

            _minmoment = _fixedendmoment.Min;

            _fixedendforce = _fixedendmoment.Derivate();
        }

        public void CalculateDeflection()
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

            if (_maxdeflection.yposition > maxdeflection)
            {
                maxdeflection = _maxdeflection.yposition;
            }
        }

        public void CalculateStress()
        {

            double precision = 0.001;
            _stress = new DotCollection();
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
                stress = Math.Pow(10,3) * _fixedendmoment.Calculate(i * precision) * y / (_inertiappoly.Calculate(i * precision) );
                _stress.Add(i * precision, stress);

                double max = _stress.YMaxAbs;

                if (max > maxstress)
                {
                    maxstress = max;
                }
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

            using (StreamWriter stw = new StreamWriter(@"stress.txt"))
            {
                for (int i = 0; i < _stress.Count; i++)
                {
                    stw.WriteLine(_stress[i].Key + " , " + _stress[i].Value * Math.Pow(10, 3));
                }
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

        public double ElasticityModulus
        {
            get { return _elasticity; }
            set { _elasticity = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PiecewisePoly Loads
        {
            get { return _loads; }
        }

        public List<KeyValuePair<double, double>> ConcentratedLoads
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

        public DotCollection Stress
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
        }

        public Point LeftPoint
        {
            get { return _innertgeometry.LeftPoint; }
        }

        public Point RightPoint
        {
            get { return _innertgeometry.RightPoint; }
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

        private double _stiffnessa;

        private double _stiffnessb;

        private double _newstiffnessa;

        private double _newstiffnessb;

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

        public double MinMoment
        {
            get { return _minmoment; }
        }

        #endregion

        #endregion

    }
}

