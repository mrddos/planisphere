﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

using Scada.Declare;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Scada.Main
{

    
	/// <summary>
	/// 
	/// </summary>
    static class Program
    {

        [STAThread]
        static void Main()
        {
            /*
            string p = @"D:\Projects\SVN\DAQ\Bin\Debug\devices\Scada.VirtualAgent\0.9\Scada.VirtualAgent.dll";
            Assembly asm = Assembly.LoadFile(p);
            Device o = asm.CreateInstance("Scada.VirtualAgent") as Device;
            string s = o.ToString();
            */

            
            /*
            StandardDevice sd = new StandardDevice("Device1");

			if (sd.Connect("COM4"))
			{
				sd.ReadData();
			}
            */
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
