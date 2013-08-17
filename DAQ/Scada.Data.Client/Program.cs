using System;
using System.Collections.Generic;
using System.Linq;
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
            Form form = null;
            if (args.Length > 0 && string.Compare(args[0], "--f", true) == 0)
            {
                form = new Form1();
            }
            else
            {
                form = new MainForm();
            }
            Application.Run(form);
        }
    }
}
