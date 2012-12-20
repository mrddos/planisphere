using Scada.Declare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scada.Main
{
	class DeviceManager
	{
        private const string DeviceConfigFile = @"device.cfg";

        private Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

        private Dictionary<string, string> selectedDevices = new Dictionary<string, string>();

        private List<Device> devices = new List<Device>();

        public DeviceManager Instance()
        {
            return null;
        }

        public DeviceManager()
		{

		}

        public string[] DeviceNames
        {
            get { return dict.Keys.ToArray<string>(); }
        }

        public List<string> GetVersions(string deviceName)
        {
            return dict[deviceName];
        }

        static string DirectoryName(string dir)
        {
            int lastBackSlash = dir.LastIndexOf('\\');
            string name = dir.Substring(lastBackSlash + 1);
            return name;
        }
		
        private void LoadDevicesInfo(string installPath)
        {
            string[] devicePaths = Directory.GetDirectories(installPath);
            foreach (string devicePath in devicePaths)
            {
                string deviceName = DirectoryName(devicePath);

                string[] versions = Directory.GetDirectories(devicePath);
                foreach (string version in versions)
                {
                    this.AddDeviceVersion(deviceName, DirectoryName(version));
                }
            }
        }

        private void AddDeviceVersion(string deviceName, string version)
        {
            deviceName = deviceName.ToLower();
            if (dict.Keys.Contains(deviceName))
            {
                dict[deviceName].Add(version);
            }
            else
            {
                List<string> versions = new List<string>();
                versions.Add(version);
                dict[deviceName] = versions;
            }
        }

        public static string GetDevicePath(string deviceName, string version)
        {
            return string.Format("{0}\\{1}\\{2}", MainApplication.InstallPath, deviceName, version);
        }

        /// <summary>
        /// Add a device to run.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="version"></param>
        public void SelectDevice(string deviceName, string version, bool selected)
        {
            if (selected)
            {
                selectedDevices.Add(deviceName, version);
            }
            else
            {
                selectedDevices.Remove(deviceName);
            }
        }

        public bool Run()
        {
            foreach (string deviceName in selectedDevices.Keys)
            {
                string version = selectedDevices[deviceName];
                string path = GetDevicePath(deviceName, version);
                if (Directory.Exists(path))
                {
                    string cfg = string.Format("{0}\\{1}", path, DeviceConfigFile);
                    // TODO: Config file reading
                }
            }
            return true;
        }

		public void Initialize()
		{
            this.LoadDevicesInfo(MainApplication.InstallPath);
		}

        
	}
}
