using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesnet.Classes.Tools
{
    using System.Runtime;
    using System.Runtime.InteropServices;

    public static class InternetAvailability
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
