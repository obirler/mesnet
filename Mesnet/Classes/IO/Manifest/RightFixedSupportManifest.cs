using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.IO.Manifest
{
    public class RightFixedSupportManifest
    {
        public int Id { get; set; }

        public int SupportId { get; set; }

        public string Name { get; set; }

        public double LeftPosition { get; set; }

        public double TopPosition { get; set; }

        public double Angle { get; set; }

        public Member Member { get; set; }
    }
}
