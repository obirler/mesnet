using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.IO.Manifest
{
    public class Member
    {
        public int Id { get; set; }

        public int BeamId { get; set; }

        public string Name { get; set; }

        public Global.Direction Direction { get; set; }
    }
}
