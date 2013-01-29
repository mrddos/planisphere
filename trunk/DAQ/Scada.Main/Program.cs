using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

using Scada.Declare;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Scada.Common;

namespace Scada.Main
{

    
	/// <summary>
	/// 
	/// </summary>
    static class Program
    {
		private const int WM_KEEPALIVE = 0x006A;
		/// <summary>
		/// 
		/// </summary>
		private static IntPtr watchFormHandle;

        private const string DeviceMappingFile = "d2d.m";

		private const string WatchExeFileName = "scada.watch";

        public static DeviceManager deviceManager = new DeviceManager();

		// private static CopyDataStruct cds = new CopyDataStruct() { cbData=10, lpData = "Hello" };

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

		public static IntPtr WatchFormHandle
		{
			get { return Program.watchFormHandle; }
		}

		public static DeviceManager DeviceManager
		{
			get { return Program.deviceManager; }
			private set { Program.deviceManager = value; }
		}

		public static bool IsWatchRunning()
		{
			Process[] procs = Process.GetProcesses();
			foreach (Process proc in procs)
			{
				string processName = proc.ProcessName.ToLower();
				if (processName.IndexOf(WatchExeFileName) >= 0)
				{
					try
					{
						Program.watchFormHandle = proc.MainWindowHandle;
					}
					catch (Exception e)
					{
						Debug.WriteLine(e.Message);
						Error error = Errors.UnknownError;
					}
					return true;
				}
			}
			return false;
		}

		public static bool SendKeepAlive()
		{
			bool ret = Defines.PostMessage(Program.WatchFormHandle, Defines.WM_KEEPALIVE, Defines.KeepAlive, 232);
			return true;
		}

		public static void StartWatchProcess()
		{
			// TODO: Start Watch Process
		}

		/// <summary>
		/// 
		/// </summary>
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


			if (!IsWatchRunning())
			{
				StartWatchProcess();
			}

			



            // StandardDevice sd = new StandardDevice("Device1");
            
            /*
			if (sd.Connect("COM4"))
			{
				sd.ReadData();
			}
            */


            //DBConnection c = new DBConnection();
            // c.Connect();
            
            deviceManager.Initialize();
            deviceManager.SelectDevice("Scada.HIPC", "0.9", true);
            deviceManager.SelectDevice("Scada.Weather", "0.9", true);
            // deviceManager.Run();

            //Thread.Sleep(1000);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
