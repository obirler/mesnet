using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.Tools
{
    public class MesnetVersion
    {
        public MesnetVersion()
        {
        }


        public MesnetVersion(string version, string url = "")
        {
            Version = version;
            Url = url;
        }

        public string Version;

        public string Url;
    }
}
