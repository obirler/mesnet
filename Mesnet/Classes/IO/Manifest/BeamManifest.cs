using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.IO.Manifest
{
    public class BeamManifest
    {
        public double Length { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public int BeamId { get; set; }

        public double IZero { get; set; }

        public double Elasticity { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public bool PerformStressAnalysis { get; set; }

        public double MaxAllowableStress { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double Angle { get; set; }

        public System.Windows.Point TopLeft { get; set; }

        public System.Windows.Point TopRight { get; set; }

        public System.Windows.Point BottomLeft { get; set; }

        public System.Windows.Point BottomRight { get; set; }

        public Math.PiecewisePoly Inertias { get; set; }

        public Math.PiecewisePoly DistributedLoads { get; set; }

        public List<KeyValuePair<double, double>> ConcentratedLoads { get; set; }

        public Math.PiecewisePoly EPolies { get; set; }

        public Math.PiecewisePoly DPolies { get; set; }

        public Connections Connections { get; set; }
    }

    public class Connections
    {
        public LeftSide LeftSide { get; set; }

        public RightSide RightSide { get; set; }
    }

    public class LeftSide
    {
        public string Type { get; set; }

        public int Id { get; set; }

        public int SupportId { get; set; }
    }

    public class RightSide
    {
        public string Type { get; set; }

        public int Id { get; set; }

        public int SupportId { get; set; }
    }
}
