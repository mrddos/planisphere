using Scada.Declare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private SynchronizationContext synchronizationContext = null;

        private SendOrPostCallback dataReceived;
        

        public DeviceManager Instance()
        {
            return null;
        }

        public DeviceManager()
		{

		}

        public SynchronizationContext SynchronizationContext
        {
            get { return this.synchronizationContext; }
            set { this.synchronizationContext = value; }
        }

        public SendOrPostCallback DataReceived
        {
            set { this.dataReceived = value; }
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
            return string.Format("{0}\\{1}\\{2}", MainApplication.DevicesRootPath, deviceName, version);
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

        public DeviceEntry LoadFromConfig(string configFile)
        {
            if (!File.Exists(configFile))
                return null;

            using (ScadaReader sr = new ScadaReader(configFile))
            {
                SectionType secType = SectionType.None;
                string line = null;
                string key = null;
                IValue value = null;
                ReadLineResult result = sr.ReadLine(out secType, out line, out key, out value);
                // Dictionary<string, string> config = new Dictionary<string, string>();
                DeviceEntry entry = new DeviceEntry();
                while (result == ReadLineResult.OK)
                {
                    result = sr.ReadLine(out secType, out line, out key, out value);

                    if (secType == SectionType.KeyWithStringValue)
                    {
                        entry[key] = value;
                    }
                }

                return entry;
            }
        }

        private Device Load(DeviceEntry entry)
        {
			if (entry == null)
				return null;

            StringValue className = (StringValue)entry[DeviceEntry.ClassName];
            if (typeof(StandardDevice).ToString() == className)
            {
                return new StandardDevice(entry);
            }

            if (entry[DeviceEntry.Assembly] != null)
            {
                Assembly assembly = Assembly.Load((StringValue)entry[DeviceEntry.Assembly]);
                Type deviceClass = assembly.GetType((StringValue)entry[DeviceEntry.ClassName]);
                if (deviceClass != null)
                {
                    object device = Activator.CreateInstance(deviceClass, new object[] { });
                    return device as Device;
                }
            }

            return (Device)null;
        }

        private DeviceEntry ReadConfigFile(string configFile)
        {
            return this.LoadFromConfig(configFile);
        }

        private bool OnDataReceived(object sender, string data)
        {
            this.SynchronizationContext.Send(this.dataReceived, data);
            return true;
        }

        public bool Run()
        {
            foreach (string deviceName in selectedDevices.Keys)
            {
                string version = selectedDevices[deviceName];
                string path = GetDevicePath(deviceName, version);
                if (Directory.Exists(path))
                {
                    string deviceCfgFile = string.Format("{0}\\{1}", path, DeviceConfigFile);
                    // TODO: Config file reading
					if (deviceCfgFile != null)
					{
						DeviceEntry entry = this.ReadConfigFile(deviceCfgFile);

						Device device = Load(entry);
						if (device != null)
						{
                            device.DataReceived += this.OnDataReceived;
							device.Run();
						}
					}

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
