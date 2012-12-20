using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
    public static class MainApplication
    {
        private static string Devices = "devices";

        public static string InstallPath
        {
            get { return System.Environment.CurrentDirectory; }
        }

        public static string DevicesRootPath
        {
            get { return string.Format("{0}\\{1}", InstallPath, Devices); }
        }

    }
}
