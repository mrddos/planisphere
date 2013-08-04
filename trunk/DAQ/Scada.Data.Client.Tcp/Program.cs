using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            AgentWindow agentWindow = new AgentWindow();
            if (args.Length > 0 && args[0] == "--start")
            {
                Thread.Sleep(2000);
                agentWindow.StartState = true;
            }
            Application.Run(agentWindow);
        }
    }
}
