using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mesnet.Classes.Ui
{
    public class CardinalSplineShape : Shape
    {
        #region Fields

        StreamGeometry Stream_geometry;

        #endregion

        #region Constructors

        public CardinalSplineShape(PointCollection points)
        {
            Points = points;
        }

        public CardinalSplineShape()
        {
        }

        #endregion

        #region Dependency Props


        public static readonly DependencyProperty PointsProperty =
            Polyline.PointsProperty.AddOwner(typeof(CardinalSplineShape),
                new FrameworkPropertyMetadata(null, OnMeasurePropertyChanged));

        public static readonly DependencyProperty TensionProperty =
            DependencyProperty.Register("Tension",
            typeof(double),
            typeof(CardinalSplineShape),
            new FrameworkPropertyMetadata(0.5, OnMeasurePropertyChanged));

        public static readonly DependencyProperty ClosedProperty =
            DependencyProperty.Register("Closed",
            typeof(bool),
            typeof(CardinalSplineShape),
            new FrameworkPropertyMetadata(false, OnMeasurePropertyChanged));


        static void OnMeasurePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as CardinalSplineShape).OnMeasurePropertyChanged(args);
        }

        static void OnRenderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as CardinalSplineShape).OnRenderPropertyChanged(args);
        }

        #endregion

        #region Event

        void OnMeasurePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Stream_geometry == null)
                Stream_geometry = new StreamGeometry();

            using (StreamGeometryContext sgc = Stream_geometry.Open())
            {
                // Get Bezier Spline Control Points.

                PointCollection pnts = CardinalSpline(Points, .5, Closed);

                sgc.BeginFigure(pnts[0], true, false);
                for (int i = 1; i < pnts.Count; i += 3)
                {
                    sgc.BezierTo(pnts[i], pnts[i + 1], pnts[i + 2], true, false);
                }
            }
            InvalidateMeasure();
            OnRenderPropertyChanged(args);
        }

        void OnRenderPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            InvalidateVisual();
        }

        #endregion

        #region Properties

        public PointCollection Points
        {
            set { SetValue(PointsProperty, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        public double Tension
        {
            set { SetValue(TensionProperty, value); }
            get { return (double)GetValue(TensionProperty); }
        }

        public bool Closed
        {
            set { SetValue(ClosedProperty, value); }
            get { return (bool)GetValue(ClosedProperty); }

        }

        #endregion

        #region DefiningGeometry

        protected override Geometry DefiningGeometry
        {
            get { return Stream_geometry; }
        }

        #endregion

        /*
         * 
         * This is what you are after!
         * Below:
         */

        #region Calculation of Spline

        private static void CalcCurve(Point[] pts, double tenstion, out Point p1, out Point p2)
        {
            double deltaX, deltaY;
            deltaX = pts[2].X - pts[0].X;
            deltaY = pts[2].Y - pts[0].Y;
            p1 = new Point((pts[1].X - tenstion * deltaX), (pts[1].Y - tenstion * deltaY));
            p2 = new Point((pts[1].X + tenstion * deltaX), (pts[1].Y + tenstion * deltaY));
        }

        private static void CalcCurveEnd(Point end, Point adj, double tension, out Point p1)
        {            
            p1 = new Point(((tension * (adj.X - end.X) + end.X)), ((tension * (adj.Y - end.Y) + end.Y)));
        }

        private static PointCollection CardinalSpline(PointCollection pts, double t, bool closed)
        {
            int i, nrRetPts;
            Point p1, p2;
            double tension = t * (1d / 3d); //we are calculating contolpoints.

            if (closed)
                nrRetPts = (pts.Count + 1) * 3 - 2;
            else
                nrRetPts = pts.Count * 3 - 2;

            Point[] retPnt = new Point[nrRetPts];
            for (i = 0; i < nrRetPts; i++)
                retPnt[i] = new Point();

            if (!closed)
            {
                CalcCurveEnd(pts[0], pts[1], tension, out p1);
                retPnt[0] = pts[0];
                retPnt[1] = p1;
            }
            for (i = 0; i < pts.Count - (closed ? 1 : 2); i++)
            {
                CalcCurve(new Point[] { pts[i], pts[i + 1], pts[(i + 2) % pts.Count] }, tension, out  p1, out p2);
                retPnt[3 * i + 2] = p1;
                retPnt[3 * i + 3] = pts[i + 1];
                retPnt[3 * i + 4] = p2;
            }
            if (closed)
            {
                CalcCurve(new Point[] { pts[pts.Count - 1], pts[0], pts[1] }, tension, out p1, out p2);
                retPnt[nrRetPts - 2] = p1;
                retPnt[0] = pts[0];
                retPnt[1] = p2;
                retPnt[nrRetPts - 1] = retPnt[0];
            }
            else
            {
                CalcCurveEnd(pts[pts.Count - 1], pts[pts.Count - 2], tension, out p1);
                retPnt[nrRetPts - 2] = p1;
                retPnt[nrRetPts - 1] = pts[pts.Count - 1];
            }
            return new PointCollection(retPnt);
        }

        #endregion

    }
}
