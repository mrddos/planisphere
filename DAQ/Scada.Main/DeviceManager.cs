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

    class DevicesInfo
    {
        public List<string> Versions
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }
    }

	class DeviceManager
	{
        private const string DeviceConfigFile = @"device.cfg";

        private Dictionary<string, DevicesInfo> dict = new Dictionary<string, DevicesInfo>();

        private Dictionary<string, string> selectedDevices = new Dictionary<string, string>();

        /// <summary>
        /// Running devices;
        /// </summary>
        private List<Device> devices = new List<Device>();

        private SendOrPostCallback dataReceived;

		private Dictionary<string, FileRecord> records = null;

        public string[] Args
        {
            get;
            set;
        }

        public DeviceManager Instance()
        {
            return null;
        }

        public DeviceManager()
		{

		}

        public SendOrPostCallback DataReceived
        {
            set { this.dataReceived = value; }
        }

        public string[] DeviceNames
        {
            get 
            {
                List<string> deviceNames = new List<string>();
                foreach (var deviceInfo in dict.Values)
                {
                    deviceNames.Add(deviceInfo.Name);
                }
                return deviceNames.ToArray();
            }
        }

        // TODO:
        public string GetDeviceDisplayName(string deviceName)
        {
            return dict[deviceName.ToLower()].DisplayName;
        }

        public List<string> GetVersions(string deviceName)
        {
            return dict[deviceName].Versions;
        }

        static string DirectoryName(string dir)
        {
            int lastBackSlash = dir.LastIndexOf('\\');
            string name = dir.Substring(lastBackSlash + 1);
            return name;
        }
		
        private void LoadDevicesInfo(string installPath)
        {
            string[] devicePaths = Directory.GetDirectories(MainApplication.DevicesRootPath);
            foreach (string devicePath in devicePaths)
            {
                string deviceName = DirectoryName(devicePath);
                string deviceKey = deviceName.ToLower();

                DevicesInfo di = null;
                if (!dict.ContainsKey(deviceKey))
                {
                    di = new DevicesInfo() { Name = deviceName };
                    di.Versions = new List<string>();
                    dict.Add(deviceKey, di);
                }
                else
                {
                    di = dict[deviceKey];
                }

                string displayConfig = devicePath + "\\display.cfg";
                if (File.Exists(displayConfig))
                {
                    using (ScadaReader sr = new ScadaReader(displayConfig))
                    {
                        SectionType secType = SectionType.None;
                        string line = null;
                        string key = null;
                        IValue value = null;
                        ReadLineResult result = sr.ReadLine(out secType, out line, out key, out value);

                        while (result == ReadLineResult.OK)
                        {
                            if (key.ToLower() == "name")
                            {
                                di.DisplayName = value.ToString();
                            }
                            result = sr.ReadLine(out secType, out line, out key, out value);
                        }
                    }
                }

                string[] versionPaths = Directory.GetDirectories(devicePath);
                foreach (string versionPath in versionPaths)
                {
                    string version = DirectoryName(versionPath);
                    di.Versions.Add(version);
                }
            }
        }


        public static string GetDevicePath(string deviceName)
        {
            return string.Format("{0}\\{1}", MainApplication.DevicesRootPath, deviceName);
        }

        public static string GetDevicePath(string deviceName, string version)
        {
            return string.Format("{0}\\{1}\\{2}", MainApplication.DevicesRootPath, deviceName, version);
        }

		public bool RegisterRecordModule(string module, FileRecord fileRecord)
		{
			if (!this.records.ContainsKey(module))
			{
				this.records.Add(module, new FileRecord("TODO:"));
				return true;
			}
			return false;
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

        public static DeviceEntry LoadFromConfig(string configFile)
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
                DirectoryInfo di = Directory.GetParent(configFile);
                string devicePath = di.FullName;
                // Path
                entry[DeviceEntry.Path] = new StringValue(devicePath);
                // Virtual 

                if (File.Exists(devicePath + "\\virtual-device"))
                {
                    entry[DeviceEntry.Virtual] = new StringValue("true");
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

        private static DeviceEntry ReadConfigFile(string configFile)
        {
            return LoadFromConfig(configFile);
        }

        public bool Run(SynchronizationContext syncCtx, SendOrPostCallback callback)
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
						DeviceEntry entry = ReadConfigFile(deviceCfgFile);

						Device device = Load(entry);
						if (device != null)
						{
							// Set thread-sync-context
                            device.SynchronizationContext = syncCtx;
							// Set data-received callback
                            device.DataReceived += callback;
							// Set file-record
							
							FileRecord fr = new FileRecord("");

                            // To hold the object.
                            this.devices.Add(device);

							// Load the address from the d2d.cfg
							string address = "COM3";
                            
							device.Start(address);
						}
					}

                }
            }
            return true;
        }

		public void Initialize()
		{
            // TODO: Remove the param;
            this.LoadDevicesInfo(MainApplication.InstallPath);
		}

		public void ShutdownDeviceConnection()
		{
			foreach (Device device in this.devices)
			{
				device.Stop();
			}
		}
	}
}
