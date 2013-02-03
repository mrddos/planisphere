﻿using Scada.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
    /// <summary>
    /// 
    /// </summary>
    public static class MainApplication
    {
        private static string Devices = "devices";

        public static MessageTimerCreator TimerCreator
        {
            set;
            get;
        }

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
