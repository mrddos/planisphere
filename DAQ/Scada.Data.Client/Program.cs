﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Scada.Data.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = new AgentWindow();
            Application.Run(form);
        }

        public static string GetInstallPath()
        {
            string p = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(p);
        }

        public static string GetDatePath(DateTime time)
        {
            return string.Format("{0}-{1:D2}", time.Year, time.Month);
        }

        public static string GetLogPath(string deviceKey)
        {
            string p = string.Format("{0}\\logs\\{1}\\{2}", GetInstallPath(), deviceKey, GetDatePath(DateTime.Now));
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            return p;
        }

        public const string System = "system";

    }
}
