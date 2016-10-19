using System.Drawing;

namespace Mesnet.Classes.Math
{
    public class Triangle : TPolygon
    {
        public Triangle(PointF p0, PointF p1, PointF p2)
        {
            Points = new PointF[] { p0, p1, p2 };
        }
    }
}
