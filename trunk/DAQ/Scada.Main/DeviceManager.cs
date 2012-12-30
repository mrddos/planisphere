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

        private SendOrPostCallback dataReceived;

		private Dictionary<string, FileRecord> records = null;
        

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
							string address = "COM5";
							device.Start(address);
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

		public void ShutdownDeviceConnection()
		{
			foreach (Device device in this.devices)
			{
				device.Stop();
			}
		}
	}
}
