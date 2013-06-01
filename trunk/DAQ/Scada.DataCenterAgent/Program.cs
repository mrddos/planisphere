using System;
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
        static void Main()
        {
            // Settings settings = new Settings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AgentWindow());
        }
    }
}
