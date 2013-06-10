﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Scada.DataCenterAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Settings settings = new Settings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AgentWindow aw = new AgentWindow();
            if (args.Length > 0 && args[0] == "--start")
            {
                aw.StartState = true;
            }
            Application.Run(aw);
        }
    }
}
