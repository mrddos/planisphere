using System;
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

        private const string DeviceMappingFile = "d2d.m";

        public static DeviceManager deviceManager = new DeviceManager();

        /// <summary>
        /// Retrieve device to device mapping.
        /// </summary>
        /// <returns>Dict with device to device mapping</returns>
        static Dictionary<string, string> LoadDeviceMapping()
        {
            Dictionary<string, string> mapping = null;
            string deviceMappingFile = string.Format("{0}\\{1}", MainApplication.InstallPath, DeviceMappingFile);
            if (File.Exists(deviceMappingFile))
            {
                using (ScadaReader sr = new ScadaReader(deviceMappingFile))
                {
                    SectionType secType = SectionType.None;
                    string line = null;
                    string key = null;
                    IValue value = null;
                    mapping = new Dictionary<string, string>();
                    while (sr.ReadLine(out secType, out line, out key, out value) == ReadLineResult.OK)
                    {
                        if (secType == SectionType.KeyWithStringValue)
                        {
                            mapping.Add(key, value.ToString());
                        }
                    }
                }
            }
            return mapping;
        }

		public static DeviceManager DeviceManager
		{
			get { return Program.deviceManager; }
			private set { Program.deviceManager = value; }
		}


        [STAThread]
        static void Main()
        {
            /*
            string p = @"D:\Projects\SVN\DAQ\Bin\Debug\devices\Scada.VirtualAgent\0.9\Scada.VirtualAgent.dll";
            Assembly asm = Assembly.LoadFile(p);
            Device o = asm.CreateInstance("Scada.VirtualAgent") as Device;
            string s = o.ToString();
            */

            LoadDeviceMapping();
            
            
            // StandardDevice sd = new StandardDevice("Device1");
            
            /*
			if (sd.Connect("COM4"))
			{
				sd.ReadData();
			}
            */

            // DBConnection c = new DBConnection();
            // c.Connect();

            
            deviceManager.Initialize();
            deviceManager.SelectDevice("Scada.RadiationDetector", "0.9", true);
            // deviceManager.Run();
            
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
