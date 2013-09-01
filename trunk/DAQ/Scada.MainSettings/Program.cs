using Scada.Config;
using Scada.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Scada.MainSettings
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            /*
             * Test Code:
            string a = GetDeviceConfigFile("scada.hpic");
            ScadaWriter sw = new ScadaWriter(a);

            sw.WriteLine("factor1", new StringValue("2"));
            sw.Commit();
            */

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SettingsForm());
		}

        public static string GetDevicesPath()
        {
            string aep = Application.ExecutablePath;
            return Directory.GetParent(aep).FullName + "\\devices";
        }

        public static string GetDevicePath(string deviceKey)
        {
            return string.Format("{0}\\{1}\\{2}", GetDevicesPath(), deviceKey, "0.9");
        }

        public static string GetDeviceConfigFile(string deviceKey)
        {
            return string.Format("{0}\\{1}", GetDevicePath(deviceKey), "device.cfg");
        }

	}
}
