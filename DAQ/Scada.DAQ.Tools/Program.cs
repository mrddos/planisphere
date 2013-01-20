using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Scada.DAQ.Tools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Initialize the DataBase");
            if (args.Length == 0)
            {
                UIMain(args);
            }
        }

        static void UIMain(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }


    }
}
