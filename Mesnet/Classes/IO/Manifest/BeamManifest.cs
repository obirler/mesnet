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

using Mesnet.Classes.Math;

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

        public KeyValueCollection ConcentratedLoads { get; set; }

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
