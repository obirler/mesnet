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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Mesnet.Classes.Math;
using Mesnet.Classes.Tools;
using Mesnet.Classes.Ui;
using Mesnet.Properties;
using Mesnet.Xaml.Pages;
using Mesnet.Xaml.User_Controls;
using MoreLinq;
using ZoomAndPan;
using static Mesnet.Classes.Global;
using System.Xml;
using Mesnet.Classes.IO.Xml;
using System.Xml.Linq;
using Mesnet.Classes.IO;

namespace Mesnet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (Settings.Default.language != "none")
            {
                SetLanguageDictionary(Settings.Default.language);
            }
            else
            {
                SetLanguageDictionary();
            }

            SetDecimalSeperator();

            InitializeComponent();

            //moment.Header = "Show Moment";

            //force.Header = "Show Force";

            //deflection.Header = "Show Deflection";

            //stress.Header = "Show Stress";

            scaleslider.Value = zoomAndPanControl.ContentScale;

            zoomAndPanControl.MaxContentScale = 12;

            scaleslider.Maximum = zoomAndPanControl.MaxContentScale;
            scaleslider.Minimum = 0;

            scroller.ScrollToHorizontalOffset(9980);
            scroller.ScrollToVerticalOffset(9335);

            bwupdate = new BackgroundWorker();
            bwupdate.DoWork += bwupdate_DoWork;

            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += timer_Tick;

            Canvas.SetZIndex(viewbox, 1);

            if(App.AssociationPath != null)
            {
                open(App.AssociationPath);
                _savefilepath = App.AssociationPath;
            }

            var tests = new TestCases(this);
            tests.RegisterTests();
        }

        private object selectedobj;

        private object preselect;

        private Point selectpoint;

        private Point circlelocation;

        private Point beammousedownpoint;

        private Point beammouseuppoint;

        private Beam tempbeam;

        private Beam selectedbeam;

        private Beam assemblybeam;

        private int beamcount = 0;

        private DispatcherTimer timer = new DispatcherTimer();

        private double beamangle = 0;

        private bool assembly = false;

        private int _leftcount = 0;

        private int _rightcount = 0;

        private double _maxstress = 150;

        private bool _momentshown = false;

        private bool _forceshown = false;

        private bool _stressshown = false;

        public bool LoadsShown = false;
     
        private string _savefilepath = null;

        #region zoomandpancontrol

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the canvas that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        private Point origContentMouseUpPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous canvas scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        private Point mousedownpoint;

        private Point mouseuppoint;

        public static BackgroundWorker bwupdate;

        private bool mousemoved = false;

        private bool writetodebug = true;

        private void SetMouseHandlingMode(string sender, MouseHandlingMode mode)
        {
            mouseHandlingMode = mode;

            if (mouseHandlingMode == MouseHandlingMode.None)
            {
                btnonlybeammode();
                btndisablerotation();
            }

            if (writetodebug)
            {
                MyDebug.WriteInformation(mode.ToString());
            }
        }

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousemoved = false;
            canvas.Focus();
            Keyboard.Focus(canvas);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(canvas);
            //MyDebug.WriteInformation("zoomAndPanControl_MouseDown origContentMouseDownPoint :", origContentMouseDownPoint.X + " : " + origContentMouseDownPoint.Y);

            if (mouseHandlingMode == MouseHandlingMode.CircularBeamConnection)
            {
                Reset();
                e.Handled = true;
                return;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 && (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                SetMouseHandlingMode("zoomAndPanControl_MouseDown", MouseHandlingMode.Zooming);
            }
            else if (mouseButtonDown == MouseButton.Left && mouseHandlingMode != MouseHandlingMode.BeamPlacing && !assembly)
            {
                // Just a plain old left-down initiates panning mode.
                SetMouseHandlingMode("zoomAndPanControl_MouseDown", MouseHandlingMode.Panning);
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "mouse handling mode : " + mouseHandlingMode.ToString());
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomAndPanControl.CaptureMouse();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the canvas.
                        //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "Shift + left-click zooms in");
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the canvas.
                        //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "Shift + left-click zooms out");
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "drag-zooming");
                    ApplyDragZoomRect();
                }
                else if (mouseHandlingMode == MouseHandlingMode.BeamPlacing)
                {
                    var x = origContentMouseDownPoint.X;
                    var y = origContentMouseDownPoint.Y;

                    tempbeam.AddCenter(canvas, x, y);
                    if (tempbeam.MaxInertia > MaxInertia)
                    {
                        MaxInertia = tempbeam.MaxInertia;
                    }
                    tempbeam.SetAngleCenter(beamangle);
                    //tempbeam.ShowCorners(4);

                    UpdateBeamTree(tempbeam);

                    Reset();

                    //MyDebug.WriteInformation("zoomAndPanControl_MouseDown", "beam has been put");
                }

                zoomAndPanControl.ReleaseMouseCapture();
                SetMouseHandlingMode("zoomAndPanControl_MouseUp", MouseHandlingMode.None);
                e.Handled = true;
            }

            origContentMouseUpPoint = e.GetPosition(canvas);

            if (origContentMouseDownPoint == origContentMouseUpPoint && !mousemoved)
            {
                if (selectedbeam != null)
                {
                    //selectedbeam.ShowCorners(5);
                    if (selectedbeam.IsInside(origContentMouseUpPoint))
                    {
                        return;
                    }
                }
                zoomAndPanControl_Clicked();
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point coord = e.GetPosition(canvas);
            coordinate.Text = "X : " + Math.Round(coord.X - 10000, 4) + " Y : " + Math.Round(10000 - coord.Y, 4);
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "panning");
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(canvas);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "zooming");
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    SetMouseHandlingMode("zoomAndPanControl_MouseMove", MouseHandlingMode.DragZooming);
                    Point curContentMousePoint = e.GetPosition(canvas);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //MyDebug.WriteInformation("zoomAndPanControl_MouseMove", "drag zooming");
                //
                // When in drag zooming mode continously upda.te the position of the rectangle
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(canvas);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        private void zoomAndPanControl_Clicked()
        {
            MyDebug.WriteInformation("Clicked!");
            Reset();
        }

        private void zoomAndPanControl_ScaleChanged(object sender, EventArgs eventArgs)
        {
            if (Scale > 0)
            {
                horizontalrect.Height = 1 / Scale;
                horizontalrect.Width = 50 / Scale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / Scale;
                verticalrect.Width = 1 / Scale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }
        }
        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(canvas);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(canvas);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in canvas coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            var newscale = zoomAndPanControl.ContentScale - 0.01;
            zoomAndPanControl.ZoomAboutPoint(newscale, contentZoomCenter);
            Scale = newscale;
            scaletext.Text = Scale.ToString();
            scaleslider.Value = Scale;

            /*if (newscale > 0)
            {
                horizontalrect.Height = 1 / newscale;
                horizontalrect.Width = 50 / newscale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / newscale;
                verticalrect.Width = 1 / newscale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }*/

            //MyDebug.WriteInformation("ZoomOut", "Canvas Scale = " + newscale);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in canvas coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            var newscale = zoomAndPanControl.ContentScale + 0.01;
            zoomAndPanControl.ZoomAboutPoint(newscale, contentZoomCenter);
            Scale = newscale;
            scaletext.Text = Scale.ToString();
            scaleslider.Value = Scale;

            /*if (newscale > 0)
            {
                horizontalrect.Height = 1 / newscale;
                horizontalrect.Width = 50 / newscale;
                Canvas.SetTop(horizontalrect, 10000 - horizontalrect.Height / 2);
                Canvas.SetLeft(horizontalrect, 10000 - horizontalrect.Width / 2);
                verticalrect.Height = 50 / newscale;
                verticalrect.Width = 1 / newscale;
                Canvas.SetTop(verticalrect, 10000 - verticalrect.Height / 2);
                Canvas.SetLeft(verticalrect, 10000 - verticalrect.Width / 2);
            }*/
            //MyDebug.WriteInformation("ZoomIn", "Canvas Scale = " + newscale);
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from canvas coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            //zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));     

            zoomAndPanControl.ZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            double scaleX = zoomAndPanControl.ContentViewportWidth / contentWidth;
            double scaleY = zoomAndPanControl.ContentViewportHeight / contentHeight;
            double newScale = zoomAndPanControl.ContentScale * Math.Min(scaleX, scaleY);
            Scale = newScale;
            scaleslider.Value = Scale;
            scaletext.Text = Scale.ToString();

            /*var timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(zoomAndPanControl.AnimationDuration);

            timer.Tick += delegate
            {
                timer.Stop();
                double scaleX = zoomAndPanControl.ContentViewportWidth/contentWidth;
                double scaleY = zoomAndPanControl.ContentViewportHeight/contentHeight;
                double newScale = zoomAndPanControl.ContentScale*Math.Min(scaleX, scaleY);
                Global.Scale = newScale;
                scaleslider.Value = Global.Scale;
                scaletext.Text = Global.Scale.ToString();
                
            };

            timer.Start();*/

            dragZoomCanvas.Visibility = Visibility.Collapsed;

            //FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
            prevZoomScale = zoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }

        #endregion

        /// <summary>
        /// Event raised when a mouse button is clicked
        ///  down over beam rectangle.
        /// </summary>
        public void core_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            Keyboard.Focus(canvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                //MyDebug.WriteInformation("Object_MouseDown", "Shift key is held down special zooming logic is executed, returning");
                return;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                //
                //MyDebug.WriteInformation("Object_MouseDown", "MouseHandlingMode.None, returning");
                return;
            }

            origContentMouseDownPoint = e.GetPosition(canvas);

            beammousedownpoint = e.GetPosition(canvas);

            //MyDebug.WriteInformation("Object_MouseDown", "mousedownpoint : " + beammousedownpoint.X + " : " + beammousedownpoint.Y);

            if (!assembly)
            {
                SetMouseHandlingMode("core_MouseDown", MouseHandlingMode.Dragging);
                var core = (Rectangle)sender;
                core.CaptureMouse();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over beam rectangle.
        /// </summary>
        public void core_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var core = (Rectangle)sender;

            if (!assembly && mouseHandlingMode != MouseHandlingMode.Dragging)
            {
                // We are not in dragging mode.
                MyDebug.WriteInformation("not dragging not assembly, returning");
                return;
            }

            if (!assembly)
            {
                SetMouseHandlingMode("core_MouseUp", MouseHandlingMode.None);
            }

            beammouseuppoint = e.GetPosition(canvas);

            MyDebug.WriteInformation("mouseuppoint : " + beammouseuppoint.X + " : " + beammouseuppoint.Y);

            if (beammouseuppoint.Equals(beammousedownpoint))
            {               
                MyDebug.WriteInformation("clicked");             
                var grid1 = core.Parent as Grid;
                var grid2 = grid1.Parent as Grid;
                var beam = grid2.Parent as Beam;
                if (beam.IsSelected())
                {
                    MyDebug.WriteInformation("beam is already selected");

                    var beamdialog = new BeamPrompt(beam, checkbeam(beam));
                    beamdialog.maxstresstbx.Text = _maxstress.ToString();
                    beamdialog.Owner = this;
                    if ((bool)beamdialog.ShowDialog())
                    {
                        beam.SetLength(beamdialog.beamlength);
                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                        beam.AddInertia(beamdialog.inertiappoly);
                        if (beam.MaxInertia > MaxInertia)
                        {
                            MaxInertia = beam.MaxInertia;
                        }
                        if ((bool)beamdialog.stresscbx.IsChecked)
                        {
                            beam.PerformStressAnalysis = true;
                            beam.AddE(beamdialog.eppoly);
                            beam.AddD((beamdialog.dppoly));
                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                            beam.MaxAllowableStress = _maxstress;
                        }

                        canvas.UpdateLayout();
                        notify.Text = (string)FindResource("beamput");
                        UpdateAllBeamTree();
                        UpdateAllSupportTree();
                    }
                }
                else
                {
                    if (!assembly)
                    {
                        UnselectAll();
                        UnSelectAllBeamItem();
                    }
                    SelectBeam(beam);

                    SelectBeamItem(beam);
                }
            }

            core.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Checks the beam whether it is connected to other beams.
        /// </summary>
        /// <param name="beam">The beam.</param>
        /// <returns>False if the beam is connected to other beam from both sides.</returns>
        private bool checkbeam(Beam beam)
        {
            if (beam.LeftSide == null && beam.RightSide == null)
            {
                return true;
            }

            if (beam.LeftSide != null)
            {
                switch (GetObjectType(beam.LeftSide))
                {
                    case ObjectType.BasicSupport:

                        var bs = beam.LeftSide as BasicSupport;
                        if (bs.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case ObjectType.SlidingSupport:

                        var ss = beam.LeftSide as SlidingSupport;
                        if (ss.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case ObjectType.LeftFixedSupport:

                        return true;
                }
            }

            if (beam.RightSide != null)
            {
                switch (GetObjectType(beam.RightSide))
                {
                    case ObjectType.BasicSupport:

                        var bs = beam.RightSide as BasicSupport;
                        if (bs.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case ObjectType.SlidingSupport:

                        var ss = beam.RightSide as SlidingSupport;
                        if (ss.Members.Count == 1)
                        {
                            return true;
                        }

                        break;

                    case ObjectType.RightFixedSupport:

                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved over an object.
        /// </summary>
        public void core_MouseMove(object sender, MouseEventArgs e)
        {
            var core = (Rectangle)sender;
            if (mouseHandlingMode != MouseHandlingMode.Dragging)
            {
                // We are not in rectangle dragging mode, so don't do anything.
                core.ReleaseMouseCapture();
                //MyDebug.WriteInformation("Object_MouseMove", "dragging, returning");
                return;
            }

            Point curContentPoint = e.GetPosition(canvas);
            coordinate.Text = "X : " + Math.Round(curContentPoint.X - 10000, 4) + " Y : " + Math.Round(curContentPoint.Y - 10000, 4);
            Vector DragVector = curContentPoint - origContentMouseDownPoint;

            //MyDebug.WriteInformation("Object_MouseMove", "moved to " + curContentPoint.X.ToString() + " : " + curContentPoint.Y.ToString());

            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.

            origContentMouseDownPoint = curContentPoint;

            foreach (object item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        var beam = item as Beam;

                        beam.Move(DragVector);

                        break;

                    default:

                        var uielement = item as UIElement;
                        Canvas.SetLeft(uielement, Canvas.GetLeft(uielement) + DragVector.X);
                        Canvas.SetTop(uielement, Canvas.GetTop(uielement) + DragVector.Y);
                        break;

                }
            }

            e.Handled = true;
        }

        public void StartCircle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;
            var grid = ellipse.Parent as Grid;
            var beam = grid.Parent as Beam;

            MyDebug.WriteInformation("Left circle selected");

            if (beam.IsSelected())
            {
                if (assemblybeam != null)
                {
                    if (!Equals(beam, assemblybeam))
                    {
                        //There should be a beam that either start circle or end circle selected. So, assemble this beam to that beam
                        switch (assemblybeam.circledirection)
                        {
                            case Direction.Left:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.LeftSide != null && beam.LeftSide != null)
                                    {
                                        if (GetObjectType(assemblybeam.LeftSide) != ObjectType.LeftFixedSupport &&
                                            GetObjectType(beam.LeftSide) != ObjectType.LeftFixedSupport)
                                        {
                                            SetMouseHandlingMode("StartCircle_MouseDown",
                                                MouseHandlingMode.CircularBeamConnection);
                                            //Both beam is bound. This will be a circular beam system, so the user want to add a beam between 
                                            //two selected beam instead of connecting them
                                            var beamdialog = new BeamPrompt(assemblybeam.LeftPoint, beam.LeftPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.inertiappoly);
                                                if (newbeam.MaxInertia > MaxInertia)
                                                {
                                                    MaxInertia = newbeam.MaxInertia;
                                                }
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.eppoly);
                                                    newbeam.AddD((beamdialog.dppoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Direction.Left, assemblybeam, Direction.Left);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Direction.Right, beam, Direction.Left);

                                                newbeam.core.MouseDown += core_MouseDown;
                                                newbeam.core.MouseUp += core_MouseUp;
                                                newbeam.core.MouseMove += core_MouseMove;
                                                newbeam.startcircle.MouseDown += StartCircle_MouseDown;
                                                newbeam.endcircle.MouseDown += EndCircle_MouseDown;

                                                canvas.UpdateLayout();
                                                notify.Text = (string)FindResource("beamput");
                                                UpdateAllBeamTree();
                                                UpdateAllSupportTree();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Reset();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Reset();
                                        return;
                                    }
                                }
                                else if (assemblybeam.LeftSide != null && beam.LeftSide != null)
                                {
                                    //If both beam has support on selected sides do nothing
                                    //Reset();
                                    return;
                                }

                                if (assemblybeam.LeftSide != null || beam.LeftSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support
                                    beam.Connect(Direction.Left, assemblybeam, Direction.Left);
                                }

                                break;

                            case Direction.Right:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.RightSide != null && beam.LeftSide != null)
                                    {
                                        if (GetObjectType(assemblybeam.RightSide) != ObjectType.RightFixedSupport &&
                                            GetObjectType(beam.LeftSide) != ObjectType.LeftFixedSupport)
                                        {
                                            SetMouseHandlingMode("StartCircle_MouseDown",
                                                MouseHandlingMode.CircularBeamConnection);
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            var beamdialog = new BeamPrompt(assemblybeam.RightPoint, beam.LeftPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.inertiappoly);
                                                if (newbeam.MaxInertia > MaxInertia)
                                                {
                                                    MaxInertia = newbeam.MaxInertia;
                                                }
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.eppoly);
                                                    newbeam.AddD((beamdialog.dppoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Direction.Left, assemblybeam, Direction.Right);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Direction.Right, beam, Direction.Left);

                                                newbeam.core.MouseDown += core_MouseDown;
                                                newbeam.core.MouseUp += core_MouseUp;
                                                newbeam.core.MouseMove += core_MouseMove;
                                                newbeam.startcircle.MouseDown += StartCircle_MouseDown;
                                                newbeam.endcircle.MouseDown += EndCircle_MouseDown;

                                                canvas.UpdateLayout();
                                                notify.Text = (string)FindResource("beamput");
                                                UpdateAllBeamTree();
                                                UpdateAllSupportTree();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Reset();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Reset();
                                        return;
                                    }
                                }
                                else if (assemblybeam.RightSide != null && beam.LeftSide != null)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.RightSide != null || beam.LeftSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support
                                    beam.Connect(Direction.Left, assemblybeam, Direction.Right);
                                }

                                break;
                        }
                        Reset();
                        UpdateAllBeamTree();
                        UpdateAllSupportTree();
                        return;
                    }
                }

                beam.SelectLeftCircle();
                circlelocation = beam.LeftPoint;
                assembly = true;
                assemblybeam = beam;

                if (beam.LeftSide != null)
                {
                    //check if there is a fixed support bonded to beam's selected side. If there is, the user should not put any support or beam to selected side, 
                    //so disable the fixed support button
                    switch (beam.LeftSide.GetType().Name)
                    {
                        case "LeftFixedSupport":
                            btnonlybeammode();
                            break;

                        case "BasicSupport":
                            btnonlybeammode();
                            break;

                        case "SlidingSupport":
                            btnonlybeammode();
                            break;
                    }
                }
                else
                {
                    btnassemblymode();
                }

                MyDebug.WriteInformation("Mouse point = " + e.GetPosition(canvas).X + " : " + e.GetPosition(canvas).Y);

                MyDebug.WriteInformation("Circle location = " + circlelocation.X + " : " + circlelocation.Y);

                selectedbeam = beam;
            }
        }

        public void EndCircle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = sender as Ellipse;
            var grid = ellipse.Parent as Grid;
            var beam = grid.Parent as Beam;

            MyDebug.WriteInformation("Right circle selected");

            if (beam.IsSelected())
            {
                //notify.Text = (string)FindResource("selectsupport");

                if (assemblybeam != null)
                {
                    if (!Equals(beam, assemblybeam))
                    {
                        //There is a beam that either start circle or end circle selected. So, assemble this beam to that beam
                        switch (assemblybeam.circledirection)
                        {
                            case Direction.Left:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.LeftSide != null && beam.RightSide != null)
                                    {
                                        if (GetObjectType(assemblybeam.LeftSide) != ObjectType.LeftFixedSupport &&
                                            GetObjectType(beam.RightSide) != ObjectType.RightFixedSupport)
                                        {
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            SetMouseHandlingMode("EndCircle_MouseDown", MouseHandlingMode.CircularBeamConnection);
                                            var beamdialog = new BeamPrompt(assemblybeam.LeftPoint, beam.RightPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.inertiappoly);
                                                if (newbeam.MaxInertia > MaxInertia)
                                                {
                                                    MaxInertia = newbeam.MaxInertia;
                                                }
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.eppoly);
                                                    newbeam.AddD((beamdialog.dppoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Direction.Left, assemblybeam, Direction.Left);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Direction.Right, beam, Direction.Right);

                                                newbeam.core.MouseDown += core_MouseDown;
                                                newbeam.core.MouseUp += core_MouseUp;
                                                newbeam.core.MouseMove += core_MouseMove;
                                                newbeam.startcircle.MouseDown += StartCircle_MouseDown;
                                                newbeam.endcircle.MouseDown += EndCircle_MouseDown;

                                                canvas.UpdateLayout();
                                                notify.Text = (string)FindResource("beamput");
                                                UpdateAllBeamTree();
                                                UpdateAllSupportTree();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Reset();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Reset();
                                        return;
                                    }
                                }
                                else if (assemblybeam.LeftSide != null && beam.RightSide != null)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.LeftSide != null || beam.RightSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support

                                    beam.Connect(Direction.Right, assemblybeam, Direction.Left);
                                }

                                break;

                            case Direction.Right:

                                if (beam.IsBound && assemblybeam.IsBound)
                                {
                                    if (assemblybeam.RightSide != null && beam.RightSide != null)
                                    {
                                        if (GetObjectType(assemblybeam.RightSide) != ObjectType.RightFixedSupport &&
                                            GetObjectType(beam.RightSide) != ObjectType.RightFixedSupport)
                                        {
                                            //Both beam is bound. This will be a circular beam system, so the user want to add beam between 
                                            //two selected beam instead of connecting them
                                            SetMouseHandlingMode("EndCircle_MouseDown", MouseHandlingMode.CircularBeamConnection);
                                            var beamdialog = new BeamPrompt(assemblybeam.RightPoint, beam.RightPoint);
                                            beamdialog.maxstresstbx.Text = _maxstress.ToString();
                                            beamdialog.Owner = this;
                                            if ((bool)beamdialog.ShowDialog())
                                            {
                                                var newbeam = new Beam(canvas, beamdialog.beamlength);
                                                newbeam.AddElasticity(beamdialog.beamelasticitymodulus);
                                                newbeam.AddInertia(beamdialog.inertiappoly);
                                                if (newbeam.MaxInertia > MaxInertia)
                                                {
                                                    MaxInertia = newbeam.MaxInertia;
                                                }
                                                if ((bool)beamdialog.stresscbx.IsChecked)
                                                {
                                                    newbeam.PerformStressAnalysis = true;
                                                    newbeam.AddE(beamdialog.eppoly);
                                                    newbeam.AddD((beamdialog.dppoly));
                                                    _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                                    beam.MaxAllowableStress = _maxstress;
                                                }
                                                newbeam.Connect(Direction.Left, assemblybeam, Direction.Right);
                                                newbeam.SetAngleLeft(beamdialog.angle);
                                                newbeam.CircularConnect(Direction.Right, beam, Direction.Right);

                                                newbeam.core.MouseDown += core_MouseDown;
                                                newbeam.core.MouseUp += core_MouseUp;
                                                newbeam.core.MouseMove += core_MouseMove;
                                                newbeam.startcircle.MouseDown += StartCircle_MouseDown;
                                                newbeam.endcircle.MouseDown += EndCircle_MouseDown;

                                                UpdateSupportTree(assemblybeam.RightSide);
                                                newbeam.SetAngleRight(beamangle);
                                                canvas.UpdateLayout();
                                                notify.Text = (string)FindResource("beamput");
                                                UpdateAllBeamTree();
                                                UpdateAllSupportTree();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Reset();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Reset();
                                        return;
                                    }
                                }
                                else if (assemblybeam.RightSide != null && beam.RightSide != null)
                                {
                                    Reset();
                                    return;
                                }

                                if (assemblybeam.RightSide != null || beam.RightSide != null)
                                {
                                    //A beam can not be added to a point that has no support, so one of the two beam has a support                                                                        
                                    beam.Connect(Direction.Right, assemblybeam, Direction.Right);
                                }

                                break;
                        }
                        Reset();
                        UpdateAllBeamTree();
                        UpdateAllSupportTree();
                        return;
                    }
                }

                beam.SelectRightCircle();
                circlelocation = beam.RightPoint;
                assembly = true;
                assemblybeam = beam;

                if (beam.RightSide != null)
                {
                    //check if there is a fixed support bonded to beam's selected side. If there is the user should not put any support selected side, 
                    //so disable the fixed support button
                    switch (beam.RightSide.GetType().Name)
                    {
                        case "RightFixedSupport":
                            btnonlybeammode();
                            break;

                        case "BasicSupport":
                            btnonlybeammode();
                            break;

                        case "SlidingSupport":
                            btnonlybeammode();
                            break;
                    }
                }
                else
                {
                    btnassemblymode();
                }

                MyDebug.WriteInformation("Mouse point = " + e.GetPosition(canvas).X + " : " + e.GetPosition(canvas).Y);

                MyDebug.WriteInformation("Beam Left = " + Canvas.GetLeft(beam).ToString() + " : Beam Top = " + Canvas.GetTop(beam).ToString());

                MyDebug.WriteInformation("Circle location = " + circlelocation.X + " : " + circlelocation.Y);

                selectedbeam = beam;
            }
        }

        private void beambtn_Click(object sender, RoutedEventArgs e)
        {
            //Check if there are any beam in the canvas
            if (objects.Any(x => GetObjectType(x) == ObjectType.Beam))
            {
                var beamdialog = new BeamPrompt();
                beamdialog.maxstresstbx.Text = _maxstress.ToString();
                beamdialog.Owner = this;
                if ((bool)beamdialog.ShowDialog())
                {
                    if (assembly)
                    {
                        //There must be a selected beam whose start circle or end circle is also selected. So place the beam to the circle location.
                        //Because we will immediately connect this beam, we must use the constructor with canvas.
                        var beam = new Beam(canvas, beamdialog.beamlength);

                        switch (selectedbeam.circledirection)
                        {
                            case Direction.Left:

                                if (selectedbeam.LeftSide != null)
                                {
                                    if (GetObjectType(selectedbeam.LeftSide) != ObjectType.LeftFixedSupport)
                                    {
                                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                                        beam.AddInertia(beamdialog.inertiappoly);
                                        if (beam.MaxInertia > MaxInertia)
                                        {
                                            MaxInertia = beam.MaxInertia;
                                        }

                                        if ((bool)beamdialog.stresscbx.IsChecked)
                                        {
                                            beam.PerformStressAnalysis = true;
                                            beam.AddE(beamdialog.eppoly);
                                            beam.AddD((beamdialog.dppoly));
                                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                            beam.MaxAllowableStress = _maxstress;
                                        }
                                        beamangle = beamdialog.angle;

                                        beam.Connect(Direction.Right, selectedbeam, Direction.Left);
                                        UpdateSupportTree(selectedbeam.LeftSide);
                                        beam.SetAngleRight(beamangle);
                                        canvas.UpdateLayout();
                                        notify.Text = (string)FindResource("beamput");
                                    }
                                }
                                else
                                {
                                    beam.Remove();
                                    Reset();
                                    return;
                                }

                                break;

                            case Direction.Right:

                                if (selectedbeam.RightSide != null)
                                {
                                    if (GetObjectType(selectedbeam.RightSide) != ObjectType.RightFixedSupport)
                                    {
                                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                                        beam.AddInertia(beamdialog.inertiappoly);
                                        if (beam.MaxInertia > MaxInertia)
                                        {
                                            MaxInertia = beam.MaxInertia;
                                        }
                                        if ((bool)beamdialog.stresscbx.IsChecked)
                                        {
                                            beam.PerformStressAnalysis = true;
                                            beam.AddE(beamdialog.eppoly);
                                            beam.AddD((beamdialog.dppoly));
                                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                                            beam.MaxAllowableStress = _maxstress;
                                        }
                                        beamangle = beamdialog.angle;

                                        beam.core.MouseDown += core_MouseDown;
                                        beam.core.MouseUp += core_MouseUp;
                                        beam.core.MouseMove += core_MouseMove;
                                        beam.startcircle.MouseDown += StartCircle_MouseDown;
                                        beam.endcircle.MouseDown += EndCircle_MouseDown;

                                        beam.Connect(Direction.Left, selectedbeam, Direction.Right);
                                        UpdateSupportTree(selectedbeam.RightSide);
                                        beam.SetAngleLeft(beamangle);
                                        canvas.UpdateLayout();
                                        notify.Text = (string)FindResource("beamput");
                                    }
                                }
                                else
                                {
                                    beam.Remove();
                                    Reset();
                                    return;
                                }

                                break;
                        }

                        UpdateBeamTree(beam);
                        Reset();
                    }
                    else
                    {
                        //There is no start or end circle selected beam in the canvas. So place the beam where the user want.
                        var beam = new Beam(beamdialog.beamlength);
                        beam.AddElasticity(beamdialog.beamelasticitymodulus);
                        beam.AddInertia(beamdialog.inertiappoly);
                        if ((bool)beamdialog.stresscbx.IsChecked)
                        {
                            beam.PerformStressAnalysis = true;
                            beam.AddE(beamdialog.eppoly);
                            beam.AddD((beamdialog.dppoly));
                            _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                            beam.MaxAllowableStress = _maxstress;
                        }
                        beamangle = beamdialog.angle;

                        tempbeam = beam;
                        SetMouseHandlingMode("beambtn_Click", MouseHandlingMode.BeamPlacing);
                        notify.Text = "Please select the point for beam";
                    }
                }
            }
            else
            {
                //There are at least one beam in the canvas but there is no selected circle. 
                //So, the user wants to put the beam wherever he want and he will connect it later.
                var beamdialog = new BeamPrompt();
                beamdialog.maxstresstbx.Text = _maxstress.ToString();
                beamdialog.Owner = this;
                if ((bool)beamdialog.ShowDialog())
                {
                    var beam = new Beam(beamdialog.beamlength);
                    beam.AddElasticity(beamdialog.beamelasticitymodulus);
                    beam.AddInertia(beamdialog.inertiappoly);
                    if ((bool)beamdialog.stresscbx.IsChecked)
                    {
                        beam.PerformStressAnalysis = true;
                        beam.AddE(beamdialog.eppoly);
                        beam.AddD((beamdialog.dppoly));
                        _maxstress = Convert.ToDouble(beamdialog.maxstresstbx.Text);
                        beam.MaxAllowableStress = _maxstress;
                    }
                    beamangle = beamdialog.angle;

                    tempbeam = beam;
                    SetMouseHandlingMode("beambtn_Click", MouseHandlingMode.BeamPlacing);
                    notify.Text = "Please select the point for beam";
                }
            }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsdialog = new SettingsPrompt();
            settingsdialog.Owner = this;

            settingsdialog.ShowDialog();
        }

        private void concentratedloadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var concentratedloadprompt = new ConcentratedLoadPrompt(selectedbeam.Length);
                concentratedloadprompt.Owner = this;

                if ((bool)concentratedloadprompt.ShowDialog())
                {
                    var load = concentratedloadprompt.Loads;
                    var concentratedload = new ConcentratedLoad(load, selectedbeam.Length);
                    selectedbeam.AddLoad(concentratedload, Direction.Up);
                    LoadsShown = true;
                    loads.Header = GetString("hideloads");
                    loads.IsEnabled = true;
                }

                assemblybeam = null;
                assembly = false;
                UnselectAll();
                btndisableall();
                SetMouseHandlingMode("concentratedloadbtn_Click", MouseHandlingMode.None);
            }
        }

        private void distributedloadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var distloadprompt = new DistributedLoadPrompt(selectedbeam);
                distloadprompt.Owner = this;

                if ((bool)distloadprompt.ShowDialog())
                {
                    selectedbeam.RemoveDistributedLoad();
                    var ppoly = new PiecewisePoly(distloadprompt.Loadpolies);
                    var load = new DistributedLoad(ppoly, selectedbeam.Length);
                    selectedbeam.AddLoad(load, Direction.Up);
                    LoadsShown = true;
                    loads.Header = GetString("hideloads");
                    loads.IsEnabled = true;
                }

                assemblybeam = null;
                assembly = false;
                UnselectAll();
                btndisableall();
                SetMouseHandlingMode("distributedloadbtn_Click", MouseHandlingMode.None);
            }
        }

        private void zoominbtn_Click(object sender, RoutedEventArgs e)
        {
            zoomAndPanControl.AnimatedZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        private void zoomoutbtn_Click(object sender, RoutedEventArgs e)
        {
            zoomAndPanControl.AnimatedZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        private void fixedsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                switch (selectedbeam.circledirection)
                {
                    case Direction.Left:
                        var leftfixedsupport = new LeftFixedSupport(canvas);
                        leftfixedsupport.AddBeam(selectedbeam);
                        notify.Text = (string)FindResource("fixedsupportput");
                        UpdateSupportTree(leftfixedsupport);
                        break;

                    case Direction.Right:
                        var rightfixedsupport = new RightFixedSupport(canvas);
                        rightfixedsupport.AddBeam(selectedbeam);
                        notify.Text = (string)FindResource("fixedsupportput");
                        UpdateSupportTree(rightfixedsupport);
                        break;

                    default:

                        MyDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                UpdateBeamTree(selectedbeam);
                SetMouseHandlingMode("fixedsupportbtn_Click", MouseHandlingMode.None);
            }
            else
            {
                MyDebug.WriteWarning("selected beam is null!");
            }

            Reset();
        }

        private void basicsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var basicsupport = new BasicSupport(canvas);

                switch (selectedbeam.circledirection)
                {
                    case Direction.Left:

                        basicsupport.AddBeam(selectedbeam, Direction.Left);

                        break;

                    case Direction.Right:

                        basicsupport.AddBeam(selectedbeam, Direction.Right);

                        break;

                    default:

                        MyDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                notify.Text = (string)FindResource("basicsupportput");
                SetMouseHandlingMode("basicsupportbtn_Click", MouseHandlingMode.None);
                UpdateSupportTree(basicsupport);
                UpdateBeamTree(selectedbeam);
            }
            else
            {
                MyDebug.WriteWarning("selected beam is null!");
            }
            Reset();
        }

        private void slidingsupportbtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                var slidingsupport = new SlidingSupport(canvas);

                switch (selectedbeam.circledirection)
                {
                    case Direction.Left:

                        slidingsupport.AddBeam(selectedbeam, Direction.Left);

                        break;

                    case Direction.Right:

                        slidingsupport.AddBeam(selectedbeam, Direction.Right);

                        break;

                    default:

                        MyDebug.WriteWarning("invalid beam circle direction!");

                        break;
                }
                SetMouseHandlingMode("slidingsupportbtn_Click", MouseHandlingMode.None);
                notify.Text = (string)FindResource("slidingsupportput");
                UpdateSupportTree(slidingsupport);
                UpdateBeamTree(selectedbeam);
            }
            else
            {
                MyDebug.WriteWarning("selected beam is null!");
            }
            Reset();
        }

        private void rotatebtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedbeam != null)
            {
                if (selectedbeam.LeftSide != null && selectedbeam.RightSide != null && selectedbeam.IsBound)
                {
                    return;
                }

                var rotateprompt = new RotatePrompt();
                rotateprompt.Owner = this;
                if (selectedbeam.LeftSide != null || selectedbeam.RightSide != null)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        if (selectedbeam.LeftSide == null && selectedbeam.RightSide != null)
                        {
                            selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));

                            switch (GetObjectType(selectedbeam.RightSide))
                            {
                                case ObjectType.RightFixedSupport:

                                    var rs = selectedbeam.RightSide as RightFixedSupport;
                                    rs.UpdatePosition(selectedbeam);

                                    break;

                                case ObjectType.SlidingSupport:

                                    var ss = selectedbeam.RightSide as SlidingSupport;
                                    if (ss.Members.Count == 1)
                                    {
                                        ss.UpdatePosition(selectedbeam);
                                    }

                                    break;

                                case ObjectType.BasicSupport:

                                    var bs = selectedbeam.RightSide as BasicSupport;
                                    if (bs.Members.Count == 1)
                                    {
                                        bs.UpdatePosition(selectedbeam);
                                    }

                                    break;
                            }
                        }
                        else if (selectedbeam.RightSide == null && selectedbeam.LeftSide != null)
                        {
                            selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));

                            switch (selectedbeam.LeftSide.GetType().Name)
                            {
                                case "LeftFixedSupport":

                                    var rs = selectedbeam.LeftSide as LeftFixedSupport;
                                    rs.UpdatePosition(selectedbeam);

                                    break;

                                case "SlidingSupport":

                                    var ss = selectedbeam.LeftSide as SlidingSupport;
                                    if (ss.Members.Count == 1)
                                    {
                                        ss.UpdatePosition(selectedbeam);
                                    }

                                    break;

                                case "BasicSupport":

                                    var bs = selectedbeam.LeftSide as BasicSupport;
                                    if (bs.Members.Count == 1)
                                    {
                                        bs.UpdatePosition(selectedbeam);
                                    }

                                    break;
                            }
                        }
                        else if (selectedbeam.RightSide != null && selectedbeam.LeftSide != null && !selectedbeam.IsBound)
                        {
                            if (selectedbeam.IsLeftSelected)
                            {
                                selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                            else if (selectedbeam.IsRightSelected)
                            {
                                selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                            else
                            {
                                selectedbeam.SetAngleCenter(Convert.ToDouble(rotateprompt.angle.Text));
                                selectedbeam.MoveSupports();
                            }
                        }
                    }
                }
                else if (selectedbeam.IsLeftSelected)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleLeft(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
                else if (selectedbeam.IsRightSelected)
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleRight(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
                else
                {
                    if ((bool)rotateprompt.ShowDialog())
                    {
                        selectedbeam.SetAngleCenter(Convert.ToDouble(rotateprompt.angle.Text));
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MyDebug.WriteInformation("Esc key down");
                Reset();
                MyDebug.WriteInformation("mouse handling mode has been set to None");
            }
        }

        /// <summary>
        /// Only beam button is enabled.
        /// </summary>
        private void btndisableall()
        {
            //beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        /// <summary>
        /// Enables load buttons.
        /// </summary>
        private void btnloadmode()
        {
            //beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = true;
            distributedloadbtn.IsEnabled = true;
        }

        private void btnassemblymode()
        {
            //assembly = true;
            beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = true;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        private void btnonlybeammode()
        {
            //assembly = false;
            beambtn.IsEnabled = true;
            fixedsupportbtn.IsEnabled = false;
            basicsupportbtn.IsEnabled = false;
            slidingsupportbtn.IsEnabled = false;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        private void supportonlymode()
        {
            beambtn.IsEnabled = false;
            fixedsupportbtn.IsEnabled = true;
            basicsupportbtn.IsEnabled = true;
            slidingsupportbtn.IsEnabled = true;
            concentratedloadbtn.IsEnabled = false;
            distributedloadbtn.IsEnabled = false;
        }

        private void btnenablerotation()
        {
            rotatebtn.IsEnabled = true;
        }

        private void btndisablerotation()
        {
            rotatebtn.IsEnabled = false;
        }

        /// <summary>
        /// Unselects all the objects in the canvas.
        /// </summary>
        private void UnselectAll()
        {
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        Beam beam = (Beam)item;
                        beam.UnSelect();
                        break;

                    case "SlidingSupport":

                        var slidingsupport = item as SlidingSupport;
                        slidingsupport.UnSelect();

                        break;

                    case "BasicSupport":

                        var basicsupport = item as BasicSupport;
                        basicsupport.UnSelect();

                        break;

                    case "LeftFixedSupport":

                        var leftfixedsupport = item as LeftFixedSupport;
                        leftfixedsupport.UnSelect();

                        break;

                    case "RightFixedSupport":

                        var rightfixedsupport = item as RightFixedSupport;
                        rightfixedsupport.UnSelect();

                        break;
                }
            }
            selectedbeam = null;
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

                Canvas.SetZIndex(viewbox, maxZ + 1);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Updates given beam in the beam tree view.
        /// </summary>
        /// <param name="beam">The beam.</param>
        private void UpdateBeamTree(Beam beam)
        {
            var beamitem = new TreeViewBeamItem(beam);
            bool exists = false;

            foreach (TreeViewBeamItem item in tree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    item.Items.Clear();
                    beamitem = item;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                beamitem.Header = new BeamItem(beam);
                tree.Items.Add(beamitem);
            }
            else
            {
                beamitem.Header = new BeamItem(beam);
            }

            if (beam.PerformStressAnalysis)
            {
                if (beam.Stress != null)
                {
                    if (beam.Stress.YMaxAbs >= beam.MaxAllowableStress)
                    {
                        beamitem.Background = new SolidColorBrush(Colors.Red);
                    }
                }
            }
            else
            {
                beamitem.Background = new SolidColorBrush(Colors.Transparent);
            }

            beamitem.Selected += BeamTreeItemSelected;

            var arrowitem = new TreeViewItem();
            var arrowbutton = new ButtonItem();
            if (!beam.DirectionShown)
            {
                arrowbutton.SetName(GetString("showdirection"));
            }
            else
            {
                arrowbutton.SetName(GetString("hidedirection"));
            }
            arrowitem.Header = arrowbutton;
            arrowbutton.content.Click += arrow_Click;
            beamitem.Items.Add(arrowitem);

            var lengthitem = new TreeViewItem();
            lengthitem.Header = new LengthItem(GetString("length") + " : " + beam.Length + " m");
            beamitem.Items.Add(lengthitem);

            var leftsideitem = new BeamSupportItem();

            if (beam.LeftSide != null)
            {
                string leftname = GetString("null");
                switch (beam.LeftSide.GetType().Name)
                {
                    case "LeftFixedSupport":
                        var ls = beam.LeftSide as LeftFixedSupport;
                        leftname = GetString("leftfixedsupport") + " " + ls.SupportId;
                        leftsideitem.Support = ls;
                        break;

                    case "SlidingSupport":
                        var ss = beam.LeftSide as SlidingSupport;
                        leftname = GetString("slidingsupport") + " " + ss.SupportId;
                        leftsideitem.Support = ss;
                        break;

                    case "BasicSupport":
                        var bs = beam.LeftSide as BasicSupport;
                        leftname = GetString("basicsupport") + " " + bs.SupportId;
                        leftsideitem.Support = bs;
                        break;
                }
                leftsideitem.Header.Text = GetString("leftside") + " : " + leftname;
                beamitem.Items.Add(leftsideitem);
            }
            else
            {
                leftsideitem.Header.Text = GetString("leftside") + " : " + GetString("null");
                beamitem.Items.Add(leftsideitem);
            }

            var rightsideitem = new BeamSupportItem();

            if (beam.RightSide != null)
            {
                string rightname = GetString("null");
                switch (beam.RightSide.GetType().Name)
                {
                    case "RightFixedSupport":
                        var rs = beam.RightSide as RightFixedSupport;
                        rightname = GetString("rightfixedsupport") + " " + rs.SupportId;
                        rightsideitem.Support = rs;
                        break;

                    case "SlidingSupport":
                        var ss = beam.RightSide as SlidingSupport;
                        rightname = GetString("slidingsupport") + " " + ss.SupportId;
                        rightsideitem.Support = ss;
                        break;

                    case "BasicSupport":
                        var bs = beam.RightSide as BasicSupport;
                        rightname = GetString("basicsupport") + " " + bs.SupportId;
                        rightsideitem.Support = bs;
                        break;
                }
                rightsideitem.Header.Text = GetString("rightside") + " : " + rightname;
                beamitem.Items.Add(rightsideitem);
            }
            else
            {
                rightsideitem.Header.Text = GetString("rightside") + " : " + GetString("null");
                beamitem.Items.Add(rightsideitem);
            }

            var elasticityitem = new TreeViewItem();
            elasticityitem.Header = new ElasticityItem(GetString("elasticity") + " : " + beam.ElasticityModulus + " GPa");
            beamitem.Items.Add(elasticityitem);

            var inertiaitem = new TreeViewItem();

            inertiaitem.Header = new InertiaItem(GetString("inertia"));
            beamitem.Items.Add(inertiaitem);

            var inertiabutton = new ButtonItem();
            if (!beam.InertiaShown)
            {
                inertiabutton.SetName(GetString("show"));
            }
            else
            {
                inertiabutton.SetName(GetString("hide"));
            }
            inertiabutton.content.Click += showinertia_Click;
            var inertiabuttonitem = new TreeViewItem();
            inertiabuttonitem.Header = inertiabutton;
            inertiaitem.Items.Add(inertiabuttonitem);

            foreach (Poly inertiapoly in beam.Inertias)
            {
                var inertiachilditem = new TreeViewItem();
                inertiachilditem.Header = inertiapoly.GetString(4) + " ,  " + inertiapoly.StartPoint + " <= x <= " + inertiapoly.EndPoint;
                inertiaitem.Items.Add(inertiachilditem);
            }

            if (beam.ConcentratedLoads.Count > 0)
            {
                var concloaditem = new TreeViewItem();
                concloaditem.Header = new ConcentratedLoadItem(GetString("concentratedloads"));
                beamitem.Items.Add(concloaditem);

                var concloadbutton = new ButtonItem();
                if (!beam.ConcentratedLoadShown)
                {
                    concloadbutton.SetName(GetString("show"));
                }
                else
                {
                    concloadbutton.SetName(GetString("hide"));
                }
                concloadbutton.content.Click += showconcload_Click;

                var concloadbuttonitem = new TreeViewItem();
                concloadbuttonitem.Header = concloadbutton;
                concloaditem.Items.Add(concloadbuttonitem);

                foreach (var item in beam.ConcentratedLoads)
                {
                    var concloadchilditem = new TreeViewItem();
                    concloadchilditem.Header = Math.Round(item.Value, 4) + " ,  " + Math.Round(item.Key, 4) + " m";
                    concloaditem.Items.Add(concloadchilditem);
                }
            }

            if (beam.DistributedLoads.Count > 0)
            {
                var distloaditem = new TreeViewItem();
                distloaditem.Header = new LoadItem(GetString("distributedloads"));
                beamitem.Items.Add(distloaditem);

                var distloadbutton = new ButtonItem();
                if (!beam.DistributedLoadShown)
                {
                    distloadbutton.SetName(GetString("show"));
                }
                else
                {
                    distloadbutton.SetName(GetString("hide"));
                }
                distloadbutton.content.Click += showdistload_Click;

                var distloadbuttonitem = new TreeViewItem();
                distloadbuttonitem.Header = distloadbutton;
                distloaditem.Items.Add(distloadbuttonitem);

                foreach (Poly distloadpoly in beam.DistributedLoads)
                {
                    var distloadchilditem = new TreeViewItem();
                    distloadchilditem.Header = distloadpoly.GetString(4) + " ,  " + distloadpoly.StartPoint + " <= x <= " + distloadpoly.EndPoint;
                    distloaditem.Items.Add(distloadchilditem);
                }
            }

            #region zerofield
            /*
            if (beam.ZeroForce != null)
            {
                var forceitem = new TreeViewItem();
                forceitem.Header = new ForceItem("Zero Force Function");
                beamitem.Items.Add(forceitem);

                var forcebutton = new ButtonItem("Show");
                forcebutton.content.Click += showforce_Click;
                var forcebuttonitem = new TreeViewItem();
                forcebuttonitem.Header = forcebutton;
                forceitem.Items.Add(forcebuttonitem);

                foreach (Poly forcepoly in beam.ZeroForce)
                {
                    var forcechilditem = new TreeViewItem();
                    forcechilditem.Header = forcepoly.GetString(4) + " ,  " + forcepoly.StartPoint + " <= x <= " + forcepoly.EndPoint;
                    forceitem.Items.Add(forcechilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information("Information");
                infoitem.Header = info;
                forceitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = "Left Side : " + Math.Round(beam.ZeroForce.Calculate(0), 4) + " kN";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = "Right Side : " + Math.Round(beam.ZeroForce.Calculate(beam.Length), 4) + " kN";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = "Max Value : " + Math.Round(beam.ZeroForce.Max, 4) + " kN";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = "Max Location : " + Math.Round(beam.ZeroForce.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = "Min Value : " + Math.Round(beam.ZeroForce.Min, 4) + " kN";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = "Min Location : " + Math.Round(beam.ZeroForce.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var forceexplorer = new ForceExplorer();
                forceexplorer.xvalue.Text = "0";
                forceexplorer.funcvalue.Text = Math.Round(beam.ZeroForce.Calculate(0), 4).ToString();
                forceexplorer.xvalue.TextChanged += forceexplorer_TextChanged;               
                exploreritem.Header = forceexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.ZeroMoment != null)
            {
                var momentitem = new TreeViewItem();
                momentitem.Header = new MomentItem("Zero Moment Function");
                beamitem.Items.Add(momentitem);

                var momentbutton = new ButtonItem("Show");
                momentbutton.content.Click += showzeromoment_Click;
                var momentbuttonitem = new TreeViewItem();
                momentbuttonitem.Header = momentbutton;
                momentitem.Items.Add(momentbuttonitem);

                foreach (Poly momentpoly in beam.ZeroMoment)
                {
                    var momentchilditem = new TreeViewItem();
                    momentchilditem.Header = momentpoly.GetString(4) + " ,  " + momentpoly.StartPoint + " <= x <= " + momentpoly.EndPoint;
                    momentitem.Items.Add(momentchilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information("Information");
                infoitem.Header = info;
                momentitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = "Left Side : " + Math.Round(beam.ZeroMoment.Calculate(0), 4) + " kNm";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = "Right Side : " + Math.Round(beam.ZeroMoment.Calculate(beam.Length), 4) + " kNm";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = "Max Value : " + Math.Round(beam.ZeroMoment.Max, 4) + " kNm";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = "Max Location : " + Math.Round(beam.ZeroMoment.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = "Min Value : " + Math.Round(beam.ZeroMoment.Min, 4) + " kNm";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = "Min Location : " + Math.Round(beam.ZeroMoment.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var momentexplorer = new MomentExplorer();
                momentexplorer.xvalue.Text = "0";
                momentexplorer.funcvalue.Text = Math.Round(beam.ZeroMoment.Calculate(0), 4).ToString();
                momentexplorer.xvalue.TextChanged += zeromomentexplorer_TextChanged;
                exploreritem.Header = momentexplorer;
                infoitem.Items.Add(exploreritem);
            }
            */
            #endregion

            if (beam.FixedEndForce != null)
            {
                var forcetitem = new TreeViewItem();
                forcetitem.Header = new ForceItem(GetString("forcefunction"));
                beamitem.Items.Add(forcetitem);

                var forcebutton = new ButtonItem();
                if (!beam.ForceShown)
                {
                    forcebutton.SetName(GetString("show"));
                }
                else
                {
                    forcebutton.SetName(GetString("hide"));
                }
                forcebutton.content.Click += showfixedendforce_Click;
                var forcebuttonitem = new TreeViewItem();
                forcebuttonitem.Header = forcebutton;
                forcetitem.Items.Add(forcebuttonitem);

                foreach (Poly forcepoly in beam.FixedEndForce)
                {
                    var forcechilditem = new TreeViewItem();
                    forcechilditem.Header = forcepoly.GetString(4) + " ,  " + forcepoly.StartPoint + " <= x <= " + forcepoly.EndPoint;
                    forcetitem.Items.Add(forcechilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                forcetitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + Math.Round(beam.FixedEndForce.Calculate(0), 4) + " kN";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + Math.Round(beam.FixedEndForce.Calculate(beam.Length), 4) + " kN";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + Math.Round(beam.FixedEndForce.Max, 4) + " kN";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + Math.Round(beam.FixedEndForce.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + Math.Round(beam.FixedEndForce.Min, 4) + " kN";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + Math.Round(beam.FixedEndForce.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var forceexplorer = new ForceExplorer();
                forceexplorer.xvalue.Text = "0";
                forceexplorer.funcvalue.Text = Math.Round(beam.FixedEndForce.Calculate(0), 4).ToString();
                forceexplorer.xvalue.TextChanged += fixedendforceexplorer_TextChanged;
                exploreritem.Header = forceexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.FixedEndMoment != null)
            {
                var momentitem = new TreeViewItem();
                momentitem.Header = new MomentItem(GetString("momentfunction"));
                beamitem.Items.Add(momentitem);

                var momentbutton = new ButtonItem();
                if (!beam.MomentShown)
                {
                    momentbutton.SetName(GetString("show"));
                }
                else
                {
                    momentbutton.SetName(GetString("hide"));
                }
                momentbutton.content.Click += showfixedendmoment_Click;
                var momentbuttonitem = new TreeViewItem();
                momentbuttonitem.Header = momentbutton;
                momentitem.Items.Add(momentbuttonitem);

                foreach (Poly momentpoly in beam.FixedEndMoment)
                {
                    var momentchilditem = new TreeViewItem();
                    momentchilditem.Header = momentpoly.GetString(4) + " ,  " + momentpoly.StartPoint + " <= x <= " + momentpoly.EndPoint;
                    momentitem.Items.Add(momentchilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                momentitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + Math.Round(beam.FixedEndMoment.Calculate(0), 4) + " kNm";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + Math.Round(beam.FixedEndMoment.Calculate(beam.Length), 4) + " kNm";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + Math.Round(beam.FixedEndMoment.Max, 4) + " kNm";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + Math.Round(beam.FixedEndMoment.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + Math.Round(beam.FixedEndMoment.Min, 4) + " kNm";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + Math.Round(beam.FixedEndMoment.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var momentexplorer = new MomentExplorer();
                momentexplorer.xvalue.Text = "0";
                momentexplorer.funcvalue.Text = Math.Round(beam.FixedEndMoment.Calculate(0), 4).ToString();
                momentexplorer.xvalue.TextChanged += fixedendmomentexplorer_TextChanged;
                exploreritem.Header = momentexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.PerformStressAnalysis && beam.Stress != null)
            {
                var stressitem = new TreeViewItem();
                stressitem.Header = new StressItem(GetString("stressdist"));
                beamitem.Items.Add(stressitem);

                var stressbutton = new ButtonItem();
                if (!beam.StressShown)
                {
                    stressbutton.SetName(GetString("show"));
                }
                else
                {
                    stressbutton.SetName(GetString("hide"));
                }
                stressbutton.content.Click += showstress_Click;
                var stressbuttonitem = new TreeViewItem();
                stressbuttonitem.Header = stressbutton;
                stressitem.Items.Add(stressbuttonitem);

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                stressitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + Math.Round(beam.Stress.Calculate(0), 4) + " MPa";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + Math.Round(beam.Stress.Calculate(beam.Length), 4) + " MPa";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + Math.Round(beam.Stress.YMax, 4) + " MPa";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + Math.Round(beam.Stress.YMaxPosition, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + Math.Round(beam.Stress.YMin, 4) + " MPa";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + Math.Round(beam.Stress.YMinPosition, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var stressexplorer = new StressExplorer();
                stressexplorer.xvalue.Text = "0";
                stressexplorer.funcvalue.Text = Math.Round(beam.Stress.Calculate(0), 4).ToString();
                stressexplorer.xvalue.TextChanged += stressexplorer_TextChanged;
                exploreritem.Header = stressexplorer;
                infoitem.Items.Add(exploreritem);
            }
        }

        /// <summary>
        /// Updates all the tree view items.
        /// </summary>
        public void UpdateAllBeamTree()
        {
            MyDebug.WriteInformation("Update All Tree Started");
            foreach (var item in objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = (Beam)item;
                        UpdateBeamTree(beam);
                        break;
                }
            }
        }

        /// <summary>
        /// Updates given support in the support tree view.
        /// </summary>
        /// <param name="support">The support.</param>
        private void UpdateSupportTree(object support)
        {
            var supportitem = new TreeViewSupportItem(support);
            bool exists = false;

            switch (GetObjectType(support))
            {
                case ObjectType.SlidingSupport:

                    var slidingsup = support as SlidingSupport;

                    foreach (TreeViewSupportItem item in supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == ObjectType.SlidingSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new SlidingSupportItem(slidingsup);
                        supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                            new SlidingSupportItem(slidingsup);
                    }

                    if (slidingsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + slidingsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var slmembersitem = new TreeViewItem();
                    slmembersitem.Header = GetString("connections");
                    supportitem.Items.Add(slmembersitem);

                    foreach (Member member in slidingsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var ssbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = ssbeamitem;

                        slmembersitem.Items.Add(memberitem);
                    }

                    break;

                case ObjectType.BasicSupport:

                    var basicsup = support as BasicSupport;

                    foreach (TreeViewSupportItem item in supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == ObjectType.BasicSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                        supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                    }

                    if (basicsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + basicsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var bsmembersitem = new TreeViewItem();
                    bsmembersitem.Header = GetString("connections");
                    supportitem.Items.Add(bsmembersitem);

                    foreach (Member member in basicsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var bsbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = bsbeamitem;
                        bsmembersitem.Items.Add(memberitem);
                    }

                    break;

                case ObjectType.LeftFixedSupport:

                    var lfixedsup = support as LeftFixedSupport;

                    foreach (TreeViewSupportItem item in supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == ObjectType.LeftFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                        supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                    }

                    if (lfixedsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + lfixedsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var lfmembersitem = new TreeViewItem();
                    lfmembersitem.Header = GetString("connection");
                    supportitem.Items.Add(lfmembersitem);

                    var lfmemberitem = new TreeViewItem();

                    var lfbeamitem = new SupportBeamItem(lfixedsup.Member.Beam.BeamId, lfixedsup.Member.Direction,
                        lfixedsup.Member.Moment);

                    lfmemberitem.Header = lfbeamitem;

                    lfmembersitem.Items.Add(lfmemberitem);

                    break;

                case ObjectType.RightFixedSupport:

                    var rfixedsup = support as RightFixedSupport;

                    foreach (TreeViewSupportItem item in supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == ObjectType.RightFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }

                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new RightFixedSupportItem(rfixedsup);
                        supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                           new RightFixedSupportItem(rfixedsup);
                    }

                    if (rfixedsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + rfixedsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var rfmembersitem = new TreeViewItem();
                    rfmembersitem.Header = GetString("connection");
                    supportitem.Items.Add(rfmembersitem);

                    var rfmemberitem = new TreeViewItem();

                    var rfbeamitem = new SupportBeamItem(rfixedsup.Member.Beam.BeamId, rfixedsup.Member.Direction,
                        rfixedsup.Member.Moment);

                    rfmemberitem.Header = rfbeamitem;

                    rfmembersitem.Items.Add(rfmemberitem);

                    break;
            }
        }

        /// <summary>
        /// Updates all the support tree view items.
        /// </summary>
        public void UpdateAllSupportTree()
        {
            MyDebug.WriteInformation("Update All Support Tree Started");
            foreach (var item in objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.SlidingSupport:

                        SlidingSupport sliding = (SlidingSupport)item;
                        UpdateSupportTree(sliding);

                        break;

                    case ObjectType.BasicSupport:

                        BasicSupport basic = (BasicSupport)item;
                        UpdateSupportTree(basic);

                        break;

                    case ObjectType.LeftFixedSupport:

                        LeftFixedSupport left = (LeftFixedSupport)item;
                        UpdateSupportTree(left);

                        break;

                    case ObjectType.RightFixedSupport:

                        RightFixedSupport right = (RightFixedSupport)item;
                        UpdateSupportTree(right);

                        break;
                }
            }
        }

        private void BeamTreeItemSelected(object sender, RoutedEventArgs e)
        {
            Reset();

            var treeitem = sender as TreeViewItem;
            var beam = (treeitem.Header as BeamItem).Beam;

            SelectBeamItem(beam);

            foreach (var item in objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        var beam1 = item as Beam;

                        if (Equals(beam1, beam))
                        {
                            SelectBeam(beam1);
                            return;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Selects the beam item corresponds to given beam in beam tree.
        /// </summary>
        /// <param name="beam">The beam.</param>
        private void SelectBeamItem(Beam beam)
        {
            foreach (TreeViewBeamItem item in tree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    item.Selected -= BeamTreeItemSelected;
                    item.IsSelected = true;
                    item.Selected += BeamTreeItemSelected;
                    break;
                }
            }
        }

        /// <summary>
        /// Unselects all beam items in beam tree.
        /// </summary>
        private void UnSelectAllBeamItem()
        {
            MyDebug.WriteInformation("UnSelectAllBeamItem executed");
            foreach (TreeViewBeamItem item in tree.Items)
            {
                item.Selected -= BeamTreeItemSelected;
                item.IsSelected = false;
                item.Selected += BeamTreeItemSelected;
            }
        }

        /// <summary>
        /// Executed when any support item selected in the treeview. It selects and highlights corresponding support.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SupportTreeItemSelected(object sender, RoutedEventArgs e)
        {
            MyDebug.WriteInformation("SupportTreeItemSelected");
            Reset();
            var treeitem = sender as TreeViewItem;
            switch (treeitem.Header.GetType().Name)
            {
                case "SlidingSupportItem":

                    var ss = (treeitem.Header as SlidingSupportItem).Support;

                    foreach (var item in objects)
                    {
                        switch (GetObjectType(item))
                        {
                            case ObjectType.SlidingSupport:

                                var slidingsupport = item as SlidingSupport;

                                if (Equals(ss, slidingsupport))
                                {
                                    slidingsupport.Select();
                                    return;
                                }

                                break;
                        }
                    }

                    break;

                case "BasicSupportItem":

                    var bs = (treeitem.Header as BasicSupportItem).Support;

                    foreach (var item in objects)
                    {
                        switch (GetObjectType(item))
                        {
                            case ObjectType.BasicSupport:

                                var basicsupport = item as BasicSupport;

                                if (Equals(bs, basicsupport))
                                {
                                    basicsupport.Select();
                                    return;
                                }

                                break;
                        }
                    }

                    break;

                case "LeftFixedSupportItem":

                    var ls = (treeitem.Header as LeftFixedSupportItem).Support;

                    foreach (var item in objects)
                    {
                        switch (GetObjectType(item))
                        {
                            case ObjectType.LeftFixedSupport:

                                var leftfixedsupport = item as LeftFixedSupport;

                                if (Equals(ls, leftfixedsupport))
                                {
                                    leftfixedsupport.Select();
                                    return;
                                }

                                break;
                        }
                    }

                    break;

                case "RightFixedSupportItem":

                    var rs = (treeitem.Header as RightFixedSupportItem).Support;

                    foreach (var item in objects)
                    {
                        switch (GetObjectType(item))
                        {
                            case ObjectType.RightFixedSupport:

                                var rightfixedsupport = item as RightFixedSupport;

                                if (Equals(rs, rightfixedsupport))
                                {
                                    rightfixedsupport.Select();
                                    return;
                                }

                                break;
                        }
                    }

                    break;
            }
        }

        private void selectsupport(object support)
        {
            
        }

        private void bwupdate_DoWork(object sender, DoWorkEventArgs e)
        {
            SetDecimalSeperator();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateAllBeamTree();
                UpdateAllSupportTree();
            }));
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Shows or hides the distributed loads on the beam.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void showdistload_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var zeroforceitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = zeroforceitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.DistributedLoadShown)
            {
                beam.ShowDistLoad();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideDistLoad();
                uc.content.Content = GetString("show");
            }
        }

        /// <summary>
        /// Shows or hides concentrated loads on the beam.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void showconcload_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var zeroforceitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = zeroforceitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.ConcentratedLoadShown)
            {
                beam.ShowConcLoad();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideConcLoad();
                uc.content.Content = GetString("show");
            }
        }

        /// <summary>
        /// Shows or hides the direction arrow on the beam
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void arrow_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var beamitem = showbuttonitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.DirectionShown)
            {
                beam.ShowDirectionArrow();
                uc.content.Content = GetString("hidedirection");
            }
            else
            {
                beam.HideDirectionArrow();
                uc.content.Content = GetString("showdirection");
            }
        }

        private void showfixedendforce_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var forceitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = forceitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.ForceShown)
            {
                beam.AddFixedEndForceDiagram();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideFixedEndForceDiagram();
                uc.content.Content = GetString("show");
            }
        }

        private void showfixedendmoment_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var zeromomentitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = zeromomentitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.MomentShown)
            {
                beam.AddFixedEndMomentDiagram();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideFixedEndMomentDiagram();
                uc.content.Content = GetString("show");
            }
        }

        private void showinertia_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var inertiaitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = inertiaitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.InertiaShown)
            {
                beam.AddInertiaDiagram();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideInertiaDiagram();
                uc.content.Content = GetString("show");
            }
        }

        private void showstress_Click(object sender, RoutedEventArgs e)
        {
            var uc = (sender as Button).Parent as ButtonItem;
            var showbuttonitem = uc.Parent as TreeViewItem;
            var inertiaitem = showbuttonitem.Parent as TreeViewItem;
            var beamitem = inertiaitem.Parent as TreeViewBeamItem;
            var beam = beamitem.Beam;
            if (!beam.StressShown)
            {
                beam.AddStressDiagram();
                uc.content.Content = GetString("hide");
            }
            else
            {
                beam.HideStressDiagram();
                uc.content.Content = GetString("show");
            }
        }

        private void fixedendforceexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as MomentExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as MomentItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var forcevalue = Math.Round(beam.FixedEndForce.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = forcevalue.ToString();
            }
            catch (Exception)
            { }
        }

        private void fixedendmomentexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as MomentExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as MomentItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var momentvalue = Math.Round(beam.FixedEndMoment.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = momentvalue.ToString();
            }
            catch (Exception)
            { }
        }

        private void stressexplorer_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tbx = (sender as TextBox);
                double xvalue = Convert.ToDouble(tbx.Text);
                var stk = tbx.Parent as StackPanel;
                var exploreritem = stk.Parent as StressExplorer;
                var infoitem = exploreritem.Parent as TreeViewItem;
                var item1 = infoitem.Parent as TreeViewItem;
                var item2 = item1.Parent as TreeViewItem;
                var item3 = item2.Header as StressItem;
                var item4 = item3.Parent as TreeViewItem;
                var beamitem = item4.Parent as TreeViewBeamItem;
                var beam = beamitem.Beam;
                var stressvalue = Math.Round(beam.Stress.Calculate(xvalue), 4);
                exploreritem.funcvalue.Text = stressvalue.ToString();
            }
            catch (Exception)
            { }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = Math.Round(Convert.ToDouble(e.NewValue), 3);
            zoomAndPanControl.ZoomAboutPoint(value, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
            Scale = value;
            scaletext.Text = value.ToString();
        }

        private void scaletext_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var value = Math.Round(Convert.ToDouble(scaletext.Text), 3);
                scaletext.Text = value.ToString();


                var timer = new DispatcherTimer();

                timer.Interval = TimeSpan.FromSeconds(zoomAndPanControl.AnimationDuration);

                if (value > 10)
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(10, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));

                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Scale = 10;
                        scaleslider.Value = Scale;
                    };
                }
                else if (value < 0)
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(0, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));

                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Scale = 0;
                        scaleslider.Value = Scale;
                    };
                }
                else
                {
                    zoomAndPanControl.AnimatedZoomAboutPoint(value, new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
                    timer.Tick += delegate
                    {
                        timer.Stop();

                        Scale = value;
                        scaleslider.Value = Scale;
                    };
                }

                //timer.Start();
            }
            catch (Exception)
            { }
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            //tooltip.Visibility = Visibility.Visible;
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //tooltip.Visibility = Visibility.Collapsed;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            /*var mousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, mousepoint.Y + 12 / Global.Scale);
            Canvas.SetLeft(viewbox, mousepoint.X + 12 / Global.Scale);
            tooltip.Text = Canvas.GetLeft(viewbox).ToString() + " : " + Canvas.GetTop(viewbox).ToString();
            viewbox.Height = 20 / Global.Scale;*/
        }

        private void zoomAndPanControl_ContentOffsetXChanged(object sender, EventArgs e)
        {
            mousemoved = true;
        }

        private void zoomAndPanControl_ContentOffsetYChanged(object sender, EventArgs e)
        {
            mousemoved = true;
        }

        /// <summary>
        /// Resets the system.
        /// </summary>
        private void Reset()
        {
            tempbeam = null;
            assemblybeam = null;
            assembly = false;
            UnselectAll();
            btndisableall();
            UnSelectAllBeamItem();
            SetMouseHandlingMode("Reset", MouseHandlingMode.None);
        }

        public void WriteStatus(string keytext)
        {
            notify.Text = (string)FindResource(keytext);
        }

        /// <summary>
        /// Selects the given beam.
        /// </summary>
        /// <param name="beam">The beam to be selected.</param>
        private void SelectBeam(Beam beam)
        {
            beam.Select();
            selectedbeam = beam;

            BringToFront(canvas, beam);
            btnloadmode();

            if (selectedbeam.LeftSide != null && selectedbeam.RightSide != null && selectedbeam.IsBound)
            {
                btndisablerotation();
            }
            else
            {
                btnenablerotation();
            }
        }

        #region Cross Solve

        /// <summary>
        /// Initializes the cross solution
        /// </summary>
        private void PreCalculate()
        {
            //CrossSolve concurently executes CrossCalculate in every beam.
            var crossdialog = new CrossSolve(this);
            crossdialog.Owner = this;
            notify.Text = "Solving";
            if ((bool)crossdialog.ShowDialog())
            {
                force.IsEnabled = true;
                //deflection.IsEnabled = true;
                stress.IsEnabled = true;
                notify.Text = "Solved";
            }
        }

        public void CrossLoop()
        {
            int step = 0;
            bool stop = false;
            List<bool> checklist = new List<bool>();

            Logger.NextLine();
            Logger.WriteLine("Cross Solver Initialized");
            Logger.NextLine();

            while (!stop)
            {
                checklist.Clear();
                MyDebug.WriteInformation("Step : " + step);
                Logger.WriteLine("**********************************************STEP : " + step + "*************************************************");
                Logger.NextLine();

                if (step % 2 == 0)
                {
                    foreach (var support in objects)
                    {
                        switch (GetObjectType(support))
                        {
                            case ObjectType.BasicSupport:

                                var bs = support as BasicSupport;

                                if (bs.CrossIndex % 2 == 0 && bs.CrossIndex <= step && bs.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + bs.MomentDifference + " at BasicSupport, " + bs.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(bs.Name + " : cross index = " + bs.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(bs.Seperate());
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var ss = support as SlidingSupport;

                                if (ss.CrossIndex % 2 == 0 && ss.CrossIndex <= step && ss.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + ss.MomentDifference + " at SlidingSupport, " + ss.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(ss.Name + " : cross index = " + ss.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(ss.Seperate());
                                }

                                break;
                        }
                    }
                }
                else
                {
                    foreach (var support in objects)
                    {
                        switch (GetObjectType(support))
                        {
                            case ObjectType.BasicSupport:

                                var bs = support as BasicSupport;

                                if (bs.CrossIndex % 2 == 1 && bs.CrossIndex <= step && bs.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + bs.MomentDifference + " at BasicSupport, " + bs.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(bs.Name + " : cross index = " + bs.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(bs.Seperate());
                                }

                                break;

                            case ObjectType.SlidingSupport:

                                var ss = support as SlidingSupport;

                                if (ss.CrossIndex % 2 == 1 && ss.CrossIndex <= step && ss.Members.Count > 1)
                                {
                                    MyDebug.WriteInformation("Moment difference = " + ss.MomentDifference + " at SlidingSupport, " + ss.Name);
                                    Logger.SplitLine();
                                    Logger.WriteLine(ss.Name + " : cross index = " + ss.CrossIndex);
                                    Logger.NextLine();
                                    checklist.Add(ss.Seperate());
                                }

                                break;
                        }
                    }
                }
                step++;

                if (checklist.All(x => x == true))
                {
                    stop = true;
                }

                if (stop)
                {
                    MyDebug.WriteInformation("stopped");
                }
            }

            Logger.WriteLine("Cross loop stopped");
            Logger.NextLine();

            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        Beam beam = (Beam)item;
                        MyDebug.WriteInformation("beam " + beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        Logger.WriteLine(beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        MyDebug.WriteInformation("beam " + beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        Logger.WriteLine(beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        break;
                }
            }

            Logger.CloseLogger();
        }

        public void BasicCrossLoop()
        {
            int step = 0;
            bool stop = false;
            List<bool> checklist = new List<bool>();

            Logger.WriteLine("Basic Cross Solver Initialized");
            Logger.NextLine();

            while (!stop)
            {
                checklist.Clear();
                MyDebug.WriteInformation("Step : " + step);
                Logger.WriteLine("**********************************************STEP : " + step + "*************************************************");
                Logger.NextLine();

                foreach (var support in objects)
                {
                    string name = support.GetType().Name;
                    switch (name)
                    {
                        case "BasicSupport":

                            var bs = support as BasicSupport;

                            if (bs.Members.Count > 1)
                            {
                                MyDebug.WriteInformation("Moment difference = " + bs.MomentDifference + " at BasicSupport, " + bs.Name);
                                Logger.SplitLine();
                                Logger.WriteLine(bs.Name + " : cross index = " + bs.CrossIndex);
                                Logger.NextLine();
                                checklist.Add(bs.Seperate());
                            }

                            break;

                        case "SlidingSupport":

                            var ss = support as SlidingSupport;

                            if (ss.Members.Count > 1)
                            {
                                MyDebug.WriteInformation("Moment difference = " + ss.MomentDifference + " at SlidingSupport, " + ss.Name);
                                Logger.SplitLine();
                                Logger.WriteLine(ss.Name + " : cross index = " + ss.CrossIndex);
                                Logger.NextLine();
                                checklist.Add(ss.Seperate());
                            }

                            break;
                    }
                }
                step++;

                if (checklist.All(x => x == true))
                {
                    stop = true;
                }

                if (stop)
                {
                    MyDebug.WriteInformation("stopped");
                }
            }

            Logger.WriteLine("Cross loop stopped");
            Logger.NextLine();

            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        Beam beam = (Beam)item;
                        MyDebug.WriteInformation("beam " + beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        Logger.WriteLine(beam.Name + " Left End Moment = " + beam.LeftEndMoment);
                        MyDebug.WriteInformation("beam " + beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        Logger.WriteLine(beam.Name + " Right End Moment = " + beam.RightEndMoment);
                        break;
                }
            }

            Logger.CloseLogger();
        }

        public void UpdateBeams()
        {
            List<double> _maxmomentlist = new List<double>();

            List<double> _maxforcelist = new List<double>();

            List<double> _maxstresslist = new List<double>();

            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        Beam beam = item as Beam;
                        beam.PostCrossUpdate();
                        _maxmomentlist.Add(beam.MaxMoment);
                        _maxmomentlist.Add(beam.MinMoment);
                        _maxforcelist.Add(beam.MaxForce);
                        _maxforcelist.Add(beam.MinForce);
                        _maxstresslist.Add(beam.MaxStress);
                        MyDebug.WriteInformation(beam.Name + " has been updated");
                        break;
                }
            }
            MaxMoment = _maxmomentlist.Max(x => Math.Abs(x));
            MaxForce = _maxforcelist.Max(x => Math.Abs(x));
            MaxStress = _maxstresslist.Max();
        }

        public void UpdateBeam()
        {
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        List<double> _maxmomentlist = new List<double>();

                        List<double> _maxforcelist = new List<double>();

                        List<double> _maxstresslist = new List<double>();
                        Beam beam = item as Beam;
                        beam.PostClapeyronUpdate();
                        _maxmomentlist.Add(beam.MaxMoment);
                        _maxmomentlist.Add(beam.MinMoment);
                        _maxforcelist.Add(beam.MaxForce);
                        _maxforcelist.Add(beam.MinForce);
                        _maxstresslist.Add(beam.MaxStress);
                        MyDebug.WriteInformation(beam.Name + " has been updated");
                        MaxMoment = _maxmomentlist.Max(x => Math.Abs(x));
                        MaxForce = _maxforcelist.Max(x => Math.Abs(x));
                        MaxStress = _maxstresslist.Max();

                        break;
                }
            }
        }

        public void WriteCarryoverFactors()
        {
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        var beam = item as Beam;
                        MyDebug.WriteInformation(beam.Name + " : Carryover AB = " + beam.CarryOverAB);

                        MyDebug.WriteInformation(beam.Name + " : Carryover BA = " + beam.CarryOverBA);

                        break;
                }
            }
        }

        /// <summary>
        /// Returns the support object whose moment difference is maximum.
        /// </summary>
        /// <returns></returns>
        public object MaxMomentSupport()
        {
            var list = new Dictionary<int, double>();
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "BasicSupport":

                        var bs = item as BasicSupport;

                        bs.CalculateTotalStiffness();

                        foreach (Member member in bs.Members)
                        {
                            var beam = member.Beam;
                            var direction = member.Direction;
                            double coeff = 0;

                            switch (direction)
                            {
                                case Direction.Left:

                                    coeff = beam.StiffnessA / bs.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " left side stiffness = " + coeff);

                                    break;

                                case Direction.Right:

                                    coeff = beam.StiffnessB / bs.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " right side stiffness = " + coeff);

                                    break;
                            }
                        }

                        list.Add(bs.Id, Math.Abs(bs.MomentDifference));

                        break;

                    case "SlidingSupport":

                        var ss = item as SlidingSupport;

                        list.Add(ss.Id, Math.Abs(ss.MomentDifference));

                        ss.CalculateTotalStiffness();

                        foreach (Member member in ss.Members)
                        {
                            var beam = member.Beam;
                            var direction = member.Direction;
                            double coeff = 0;

                            switch (direction)
                            {
                                case Direction.Left:

                                    coeff = beam.StiffnessA / ss.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " left side stiffness = " + coeff);

                                    break;

                                case Direction.Right:

                                    coeff = beam.StiffnessB / ss.TotalStiffness;

                                    MyDebug.WriteInformation(beam.Name + " right side stiffness = " + coeff);

                                    break;
                            }
                        }

                        break;

                    case "LeftFixedSupport":

                        //We search max moment in sliding or basic supports. If the max moment is at one fixed support the algorithm does not run correctly and immediately finishes 

                        break;

                    case "RightFixedSupport":

                        //We search max moment in sliding or basic supports. If the max moment is at one fixed support the algorithm does not run correctly and immediately finishes

                        break;
                }
            }

            int id = list.MaxBy(x => x.Value).Key;

            return GetObject(id);
        }

        /// <summary>
        /// Indexes all the supports. The support that has max moment difference has index of 0 and its neighbours has one and their neighbours has 2 etc.
        /// </summary>
        public void IndexAll(object support)
        {
            MyDebug.WriteInformation("IndexAll started");

            string startsupport = SupportName(support);

            Graph graph = new Graph();

            #region create graph
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "BasicSupport":
                        {
                            var bs = item as BasicSupport;

                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in bs.Members)
                            {
                                Beam beam = member.Beam;
                                Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(bs), supportdict);

                            break;
                        }

                    case "SlidingSupport":
                        {
                            var ss = item as SlidingSupport;

                            var supportdict = new Dictionary<string, int>();

                            foreach (Member member in ss.Members)
                            {
                                Beam beam = member.Beam;
                                Direction direction = member.Direction;

                                switch (direction)
                                {
                                    case Direction.Left:

                                        var supright = beam.RightSide;

                                        supportdict.Add(SupportName(supright), 1);

                                        break;

                                    case Direction.Right:

                                        var supleft = beam.LeftSide;

                                        supportdict.Add(SupportName(supleft), 1);

                                        break;
                                }
                            }

                            graph.AddSupport(SupportName(ss), supportdict);

                            break;
                        }

                    case "LeftFixedSupport":
                        {
                            var ls = item as LeftFixedSupport;

                            var supportdict = new Dictionary<string, int>();

                            Beam beam = ls.Member.Beam;
                            Direction direction = ls.Member.Direction;

                            switch (direction)
                            {
                                case Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(ls), supportdict);

                            break;
                        }

                    case "RightFixedSupport":
                        {
                            var rs = item as RightFixedSupport;

                            var supportdict = new Dictionary<string, int>();

                            Beam beam = rs.Member.Beam;
                            Direction direction = rs.Member.Direction;

                            switch (direction)
                            {
                                case Direction.Left:

                                    var supright = beam.RightSide;

                                    supportdict.Add(SupportName(supright), 1);

                                    break;

                                case Direction.Right:

                                    var supleft = beam.LeftSide;

                                    supportdict.Add(SupportName(supleft), 1);

                                    break;
                            }

                            graph.AddSupport(SupportName(rs), supportdict);

                            break;
                        }
                }
            }
            #endregion

            foreach (var supportname in graph.Vertices.Keys)
            {
                int index = graph.ShortestPath(startsupport, supportname).Count;

                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "BasicSupport":

                            var bs = item as BasicSupport;

                            if (bs.Name == supportname)
                            {
                                bs.CrossIndex = index;
                            }

                            break;

                        case "SlidingSupport":

                            var ss = item as SlidingSupport;

                            if (ss.Name == supportname)
                            {
                                ss.CrossIndex = index;
                            }

                            break;

                        case "LeftFixedSupport":

                            var ls = item as LeftFixedSupport;

                            if (ls.Name == supportname)
                            {
                                ls.CrossIndex = index;
                            }

                            break;

                        case "RightFixedSupport":

                            var rs = item as RightFixedSupport;

                            if (rs.Name == supportname)
                            {
                                rs.CrossIndex = index;
                            }

                            break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Button E-event that starts cross solution.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void solve_Click(object sender, RoutedEventArgs e)
        {
            if (objects.Count > 0)
            {
                hidediagrams();
                PreCalculate();
            }
        }

        private void moment_Click(object sender, RoutedEventArgs e)
        {
            ShowMoments();
            /*
            if (moment.Header == GetString("showmoment"))
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.AddFixedEndMomentDiagram();

                            break;
                    }
                }

                moment.Header = Application.Current.FindResource("hidemoment");
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.HideFixedEndMomentDiagram();

                            break;
                    }
                }
                moment.Header = "Show Moment";
            }
            */
        }

        public void ShowMoments()
        {
            MyDebug.WriteInformation("Show Moments Started");

            moment.IsEnabled = true;

            if (!_momentshown)
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.AddFixedEndMomentDiagram();

                            break;
                    }
                }
                moment.Header = GetString("hidemoment");
                _momentshown = true;
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.HideFixedEndMomentDiagram();

                            break;
                    }
                }
                moment.Header = GetString("showmoment");
                _momentshown = false;
            }
        }

        private void force_Click(object sender, RoutedEventArgs e)
        {
            if (!_forceshown)
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.AddFixedEndForceDiagram();

                            break;
                    }

                    force.Header = GetString("hideforce");
                    _forceshown = true;
                }
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.HideFixedEndForceDiagram();

                            break;
                    }
                }
                force.Header = GetString("showforce");
                _forceshown = false;
            }
        }

        private void deflection_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (deflection.Header == "Show Deflection")
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.AddDeflectionDiagram();

                            break;
                    }
                }
                deflection.Header = "Hide Deflection";
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.HideDeflectionDiagram();

                            break;
                    }
                }
                deflection.Header = "Show Deflection";
            }
            */
        }

        private void stress_Click(object sender, RoutedEventArgs e)
        {
            bool check = false;

            foreach (var item in objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        var beam = item as Beam;
                        if (beam.PerformStressAnalysis)
                        {
                            check = true;
                        }

                        break;
                }
            }

            if (check)
            {
                if (!_stressshown)
                {
                    foreach (var item in objects)
                    {
                        switch (item.GetType().Name)
                        {
                            case "Beam":

                                Beam beam = item as Beam;
                                if (beam.PerformStressAnalysis)
                                {
                                    beam.AddStressDiagram();
                                }

                                break;
                        }
                    }
                    stress.Header = GetString("hidestress");
                    _stressshown = true;
                }
                else
                {
                    foreach (var item in objects)
                    {
                        switch (item.GetType().Name)
                        {
                            case "Beam":

                                Beam beam = item as Beam;
                                if (beam.PerformStressAnalysis)
                                {
                                    beam.HideStressDiagram();
                                }

                                break;
                        }
                    }
                    stress.Header = GetString("showstress");
                    _stressshown = false;
                }
            }
        }

        private void loads_Click(object sender, RoutedEventArgs e)
        {
            if (!LoadsShown)
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.ShowDistLoad();
                            beam.ShowConcLoad();

                            break;
                    }
                }
                loads.Header = GetString("hideloads");
                LoadsShown = true;
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = item as Beam;
                            beam.HideDistLoad();
                            beam.HideConcLoad();

                            break;
                    }
                }
                loads.Header = GetString("showloads");
                LoadsShown = false;
            }

            UpdateAllBeamTree();
        }    
            
        public void DisableTestMenus()
        {
            foreach (MenuItem menuitem in testmenu.Items)
            {
                menuitem.IsEnabled = false;
            }
        }

        private void enabletestmenus()
        {
            foreach (MenuItem menuitem in testmenu.Items)
            {
                menuitem.IsEnabled = true;
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            resetsystem();
        }

        private void resetsystem()
        {
            clearcanvas();
            objects.Clear();
            MaxMoment = Double.MinValue;
            MaxForce = Double.MinValue;
            MaxInertia = Double.MinValue;
            MaxLoad = Double.MinValue;
            MaxDeflection = Double.MinValue;
            MaxStress = Double.MinValue;
            BeamCount = 0;
            SupportCount = 0;           
            tree.Items.Clear();
            supporttree.Items.Clear();
            enabletestmenus();
            solve.IsEnabled = true;
            moment.IsEnabled = false;
            moment.Header = "Show Moment";
            force.IsEnabled = false;
            force.Header = "Show Force";
            stress.Header = "Show Stress";
            _savefilepath = null;
        }

        private string SupportName(object support)
        {
            string name = null;
            switch (support.GetType().Name)
            {
                case "LeftFixedSupport":

                    var ls = support as LeftFixedSupport;

                    name = ls.Name;

                    break;

                case "RightFixedSupport":

                    var rs = support as RightFixedSupport;

                    name = rs.Name;

                    break;

                case "BasicSupport":

                    var bs = support as BasicSupport;

                    name = bs.Name;

                    break;

                case "SlidingSupport":

                    var ss = support as SlidingSupport;

                    name = ss.Name;

                    break;
            }
            return name;
        }

        public void distloadmousemove(object sender, MouseEventArgs e)
        {
            Canvas loadcanvas = (sender as CardinalSplineShape).Parent as Canvas;
            DistributedLoad load = loadcanvas.Parent as DistributedLoad;
            var mousepoint = e.GetPosition(loadcanvas);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(load.LoadPpoly.Calculate(mousepoint.X / 100), 4) + " kN/m";
            viewbox.Height = 20 / Scale;
        }

        public void inertiamousemove(object sender, MouseEventArgs e)
        {
            Canvas inertiacanvas = (sender as CardinalSplineShape).Parent as Canvas;
            Inertia inertia = inertiacanvas.Parent as Inertia;
            var mousepoint = e.GetPosition(inertiacanvas);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(inertia.InertiaPpoly.Calculate(mousepoint.X / 100), 4) + " cm^4";
            viewbox.Height = 20 / Scale;
        }

        public void momentmousemove(object sender, MouseEventArgs e)
        {
            Canvas momentcanvas = (sender as CardinalSplineShape).Parent as Canvas;
            Moment moment = momentcanvas.Parent as Moment;
            var mousepoint = e.GetPosition(momentcanvas);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(moment.MomentPpoly.Calculate(mousepoint.X / 100), 4) + " kNm";
            viewbox.Height = 20 / Scale;
        }

        public void forcemousemove(object sender, MouseEventArgs e)
        {
            Canvas forcecanvas = (sender as CardinalSplineShape).Parent as Canvas;
            Force force = forcecanvas.Parent as Force;
            var mousepoint = e.GetPosition(forcecanvas);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(force.ForcePpoly.Calculate(mousepoint.X / 100), 4) + " kN";
            viewbox.Height = 20 / Scale;
        }

        public void stressmousemove(object sender, MouseEventArgs e)
        {
            Canvas stresscanvas = (sender as CardinalSplineShape).Parent as Canvas;
            Stress stress = stresscanvas.Parent as Stress;
            var mousepoint = e.GetPosition(stresscanvas);
            var globalmousepoint = e.GetPosition(canvas);
            Canvas.SetTop(viewbox, globalmousepoint.Y + 12 / Scale);
            Canvas.SetLeft(viewbox, globalmousepoint.X + 12 / Scale);
            tooltip.Text = Math.Round(mousepoint.X / 100, 4) + " , " + Math.Round(stress.Calculate(mousepoint.X / 100), 4) + " MPa";
            viewbox.Height = 20 / Scale;
        }

        public void mouseenter(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Visible;
        }

        public void mouseleave(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Collapsed;
        }

        public void UpdateLanguages()
        {
            if (_momentshown)
            {
                moment.Header = GetString("hidemoment");
            }
            else
            {
                moment.Header = GetString("showmoment");
            }

            if (_forceshown)
            {
                force.Header = GetString("hideforce");
            }
            else
            {
                force.Header = GetString("showforce");
            }

            if (_stressshown)
            {
                stress.Header = GetString("hidestress");
            }
            else
            {
                stress.Header = GetString("showstress");
            }

            if (LoadsShown)
            {
                loads.Header = GetString("hideloads");
            }
            else
            {
                loads.Header = GetString("showloads");
            }

            UpdateAllBeamTree();

            UpdateAllSupportTree();
        }

        private void hidediagrams()
        {
            foreach (var item in objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        var beam = item as Beam;
                        beam.HideFixedEndForceDiagram();
                        beam.HideFixedEndMomentDiagram();
                        beam.HideInertiaDiagram();
                        beam.HideDirectionArrow();
                        beam.HideStressDiagram();

                        break;
                }
            }
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            menunew();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            menuopen();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            menusave();
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            menusaveas();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            menuexit();
        }

        private void menunew()
        {
            MyDebug.WriteInformation("Open menu clicked");
            if (objects.Count > 0)
            {
                MyDebug.WriteInformation("there are at least one pbject in the workspace");
                var prompt = new MessagePrompt(GetString("asktosavebeforeopenning"));
                prompt.Owner = this;
                if ((bool)prompt.ShowDialog())
                {
                    switch (prompt.Result)
                    {
                        case Classes.Global.DialogResult.Yes:
                            {
                                //the user has accepted to save
                                var ioxml = new MesnetIO();

                                if (_savefilepath != null)
                                {
                                    //the file has been saved before, so save with the previous path
                                    MyDebug.WriteInformation("save file path exists " + _savefilepath);
                                    ioxml.WriteXml(_savefilepath);
                                    MyDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                    WriteStatus("filesave");
                                }
                                else
                                {
                                    //the file has not been saved before, open file save dialog
                                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                    saveFileDialog.Filter = GetString("filefilter");
                                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                    {
                                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                        saveFileDialog.InitialDirectory = directory;
                                        MyDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                                    }
                                    else
                                    {
                                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        MyDebug.WriteInformation("there is no path exists in save file path settings");
                                    }

                                    if ((bool)saveFileDialog.ShowDialog())
                                    {
                                        string path = saveFileDialog.FileName;
                                        MyDebug.WriteInformation("user selected a file from save file dialog: " + path);
                                        ioxml.WriteXml(path);
                                        MyDebug.WriteInformation("xml file has been written to " + path);
                                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                                        WriteStatus("filesave");
                                    }
                                    else
                                    {
                                        MyDebug.WriteInformation("User closed the dialog, aborting saving and new operation");
                                        return;
                                    }
                                }
                            }
                            break;

                        case Classes.Global.DialogResult.No:
                            //The user dont want to save current file, do nothing
                            MyDebug.WriteInformation("Dialog result No");
                            break;

                        case Classes.Global.DialogResult.Cancel:
                            //The user cancelled the dialog, abort the operation
                            MyDebug.WriteInformation("Dialog result Cancel");
                            return;

                        case Classes.Global.DialogResult.None:
                            //Something we dont know happened, abort the operation
                            MyDebug.WriteInformation("Dialog result None");
                            return;
                    }
                }
            }

            resetsystem();
        }

        private void menuopen()
        {
            MyDebug.WriteInformation("Open menu clicked");
            if (objects.Count > 0)
            {
                MyDebug.WriteInformation("there are at least one pbject in the workspace");
                var prompt = new MessagePrompt(GetString("asktosavebeforeopenning"));
                prompt.Owner = this;
                if ((bool)prompt.ShowDialog())
                {
                    switch (prompt.Result)
                    {
                        case Classes.Global.DialogResult.Yes:
                            {
                                //the user has accepted to save
                                var ioxml = new MesnetIO();

                                if (_savefilepath != null)
                                {
                                    //the file has been saved before, so save with the previous path
                                    MyDebug.WriteInformation("save file path exists " + _savefilepath);
                                    ioxml.WriteXml(_savefilepath);
                                    MyDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                    WriteStatus("filesave");
                                }
                                else
                                {
                                    //the file has not been saved before, open file save dialog
                                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                    saveFileDialog.Filter = GetString("filefilter");
                                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                    {
                                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                        saveFileDialog.InitialDirectory = directory;
                                        MyDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                                    }
                                    else
                                    {
                                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        MyDebug.WriteInformation("there is no path exists in save file path settings");
                                    }

                                    if ((bool)saveFileDialog.ShowDialog())
                                    {
                                        string path = saveFileDialog.FileName;
                                        MyDebug.WriteInformation("user selected a file from save file dialog: " + path);
                                        ioxml.WriteXml(path);
                                        MyDebug.WriteInformation("xml file has been written to " + path);
                                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                                        WriteStatus("filesave");
                                    }
                                    else
                                    {
                                        MyDebug.WriteInformation("User closed the dialog, aborting saving and opening operation");
                                        return;
                                    }
                                }
                            }
                            break;

                        case Classes.Global.DialogResult.No:
                            //The user dont want to save current file, do nothing
                            MyDebug.WriteInformation("Dialog result No");
                            break;

                        case Classes.Global.DialogResult.Cancel:
                            //The user cancelled the dialog, abort the operation
                            MyDebug.WriteInformation("Dialog result Cancel");
                            return;

                        case Classes.Global.DialogResult.None:
                            //Something we dont know happened, abort the operation
                            MyDebug.WriteInformation("Dialog result None");
                            return;
                    }
                }
            }

            resetsystem();

            var openxmlio = new MesnetIO();
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = GetString("filefilter");
            if (MesnetSettings.IsSettingExists("openpath", "mainwindow"))
            {
                string directory = MesnetSettings.ReadSetting("openpath", "mainwindow");
                openFileDialog.InitialDirectory = directory;
                MyDebug.WriteInformation("there is a path exists in open file path settings: " + directory);
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                MyDebug.WriteInformation("there is no path exists in save file path settings");
            }

            if ((bool)openFileDialog.ShowDialog())
            {
                string path = openFileDialog.FileName;
                MyDebug.WriteInformation("user selected a file from open file dialog: " + path);
                if (openxmlio.ReadXml(canvas, path))
                {
                    UpdateAllBeamTree();
                    UpdateAllSupportTree();
                    MyDebug.WriteInformation("xml file has been read from " + path);
                    MesnetSettings.WriteSetting("openpath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                    WriteStatus("fileread");
                }
                else
                {
                    MyDebug.WriteWarning("xml file could not be read!");
                    WriteStatus("filereaderror");
                }               
            }
            else
            {
                MyDebug.WriteInformation("User closed the dialog, aborting opening operation");
                return;
            }
        }

        private void menusave()
        {
            if (objects.Count > 0)
            {
                var ioxml = new MesnetIO();
                if (_savefilepath != null)
                {
                    //the file has been saved before, so save with the previous path
                    MyDebug.WriteInformation("save file path exists " + _savefilepath);
                    ioxml.WriteXml(_savefilepath);
                    MyDebug.WriteInformation("xml file has been written to " + _savefilepath);
                    WriteStatus("filesave");
                }
                else
                {
                    //the file has not been saved before, open file save dialog
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    saveFileDialog.Filter = GetString("filefilter");
                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                    {
                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                        saveFileDialog.InitialDirectory = directory;
                        MyDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                    }
                    else
                    {
                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        MyDebug.WriteInformation("there is no path exists in save file path settings");
                    }

                    if ((bool)saveFileDialog.ShowDialog())
                    {
                        string path = saveFileDialog.FileName;
                        MyDebug.WriteInformation("user selected a file from save file dialog: " + path);
                        ioxml.WriteXml(path);
                        MyDebug.WriteInformation("xml file has been written to " + path);
                        MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                        WriteStatus("filesave");
                        _savefilepath = path;
                    }
                    else
                    {
                        MyDebug.WriteInformation("User closed the dialog, aborting saving operation");
                        return;
                    }
                }
            }
        }

        private void menusaveas()
        {
            if (objects.Count > 0)
            {
                var ioxml = new MesnetIO();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = GetString("filefilter");
                if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                {
                    string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                    saveFileDialog.InitialDirectory = directory;
                    MyDebug.WriteInformation("there is a path exists in save file path settings: " + directory);
                }
                else
                {
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    MyDebug.WriteInformation("there is no path exists in save file path settings");
                }

                if ((bool)saveFileDialog.ShowDialog())
                {
                    string path = saveFileDialog.FileName;
                    MyDebug.WriteInformation("user selected a file from save file dialog: " + path);
                    ioxml.WriteXml(path);
                    MyDebug.WriteInformation("xml file has been written to " + path);
                    MesnetSettings.WriteSetting("savepath", System.IO.Path.GetDirectoryName(path), "mainwindow");
                    WriteStatus("filesave");
                    _savefilepath = path;
                }
                else
                {
                    MyDebug.WriteInformation("User closed the dialog, aborting saving operation");
                    return;
                }
            }
        }

        private void menuexit()
        {
            if(objects.Count > 0)
            {
                MyDebug.WriteInformation("there are at least one object in the workspace");
                var prompt = new MessagePrompt(GetString("asktosavebeforeopenning"));
                prompt.Owner = this;

                try
                {
                    if ((bool)prompt.ShowDialog())
                    {
                        switch (prompt.Result)
                        {
                            case Classes.Global.DialogResult.Yes:
                            {
                                //the user has accepted to save
                                var ioxml = new MesnetIO();
                                if (_savefilepath != null)
                                {
                                    //the file has been saved before, so save with the previous path
                                    MyDebug.WriteInformation("save file path exists " + _savefilepath);
                                    ioxml.WriteXml(_savefilepath);
                                    MyDebug.WriteInformation("xml file has been written to " + _savefilepath);
                                    WriteStatus("filesave");
                                }
                                else
                                {
                                    //the file has not been saved before, open file save dialog
                                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                                    saveFileDialog.Filter = GetString("filefilter");
                                    if (MesnetSettings.IsSettingExists("savepath", "mainwindow"))
                                    {
                                        string directory = MesnetSettings.ReadSetting("savepath", "mainwindow");
                                        saveFileDialog.InitialDirectory = directory;
                                        MyDebug.WriteInformation(
                                            "there is a path exists in save file path settings: " + directory);
                                    }
                                    else
                                    {
                                        saveFileDialog.InitialDirectory =
                                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                        MyDebug.WriteInformation(
                                            "there is no path exists in save file path settings");
                                    }

                                    if ((bool)saveFileDialog.ShowDialog())
                                    {
                                        string path = saveFileDialog.FileName;
                                        MyDebug.WriteInformation("user selected a file from save file dialog: " +
                                                                 path);
                                        ioxml.WriteXml(path);
                                        MyDebug.WriteInformation("xml file has been written to " + path);
                                        MesnetSettings.WriteSetting("savepath",
                                            System.IO.Path.GetDirectoryName(path), "mainwindow");
                                        WriteStatus("filesave");
                                        _savefilepath = path;
                                    }
                                    else
                                    {
                                        MyDebug.WriteInformation(
                                            "User closed the dialog, aborting saving operation");
                                        return;
                                    }
                                }
                            }
                                break;

                            case Classes.Global.DialogResult.No:
                                //The user dont want to save current file, do nothing
                                MyDebug.WriteInformation("Dialog result No");
                                break;

                            case Classes.Global.DialogResult.Cancel:
                                //The user cancelled the dialog, abort the operation
                                MyDebug.WriteInformation("Dialog result Cancel");
                                return;

                            case Classes.Global.DialogResult.None:
                                //Something we dont know happened, abort the operation
                                MyDebug.WriteInformation("Dialog result None");
                                return;
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }              
            }            
            Application.Current.Shutdown();
        }

        private void open(string path)
        {
            var openxmlio = new MesnetIO();
            openxmlio.ReadXml(canvas, path);
            UpdateAllBeamTree();
            UpdateAllSupportTree();
            MyDebug.WriteInformation("xml file has been read from " + path);
            WriteStatus("fileread");
        }

        private void clearcanvas()
        {
            foreach(object item in objects)
            {
                canvas.Children.Remove(item as UIElement);
            }
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menunew();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menuopen();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            menusave();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            menuexit();
        }
    }
}
