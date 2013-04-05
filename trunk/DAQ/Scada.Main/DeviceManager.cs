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

    class DeviceRunContext
    {
        public DeviceRunContext(string deviceName, string version)
        {
            this.DeviceName = deviceName;
            this.Version = version;
        }

        public string DeviceName
        {
            get;
            private set;
        }

        public string Version
        {
            get;
            private set;
        }

        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }

        public SendOrPostCallback Callback
        {
            get;
            set;
        }

        public Device Device
        {
            get;
            set;
        }
    }

	class DeviceManager
	{
        private const string DeviceConfigFile = @"device.cfg";

        private const string DeviceMappingFile = @"d2d.m";

        private Dictionary<string, DevicesInfo> dict = new Dictionary<string, DevicesInfo>();

        /// <summary>
        /// 
        /// At most has 10 SerailPort device.
        /// </summary>
        private Dictionary<string, string> d2d = new Dictionary<string, string>(10);


        private Dictionary<string, DeviceRunContext> selectedDevices = new Dictionary<string, DeviceRunContext>();


        private Dictionary<string, long> lastUpdateDict = new Dictionary<string, long>();


        /// <summary>
        /// Running devices;
        /// </summary>
        // private List<Device> devices = new List<Device>();

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
				if (deviceName.StartsWith("!") || deviceName.StartsWith("."))
				{
					continue;
				}
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

            // TODO: Load d2d
            string d2dFile = MainApplication.InstallPath + "\\" + DeviceMappingFile;
            if (File.Exists(d2dFile))
            {
                using (StreamReader sr = new StreamReader(d2dFile))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] kv = line.Split(':');
                        if (kv.Length == 2)
                        {
                            string device = kv[0].Trim();
                            string address = kv[1].Trim();
                            this.d2d.Add(device.ToLower(), address); 
                        }
                    }
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
            string deviceKey = deviceName.ToLower();
            if (selected)
            {
                selectedDevices.Add(deviceKey, new DeviceRunContext(deviceName, version));
            }
            else
            {
                selectedDevices.Remove(deviceKey);
            }
        }

        public static DeviceEntry LoadFromConfig(string deviceName, string configFile)
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
				entry[DeviceEntry.Identity] = new StringValue(deviceName);

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
			else if (typeof(WebFileDevice).ToString() == className)
			{
				return new WebFileDevice(entry);
			}
            else if (typeof(FormProxyDevice).ToString() == className)
            {
                return new FormProxyDevice(entry);
            }

			// Other Device defined in some Assemblies.
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

        private static DeviceEntry ReadConfigFile(string deviceName, string configFile)
        {
			return LoadFromConfig(deviceName, configFile);
        }

        public bool Run(SynchronizationContext syncCtx, SendOrPostCallback callback)
        {
            foreach (string deviceName in selectedDevices.Keys)
            {
                DeviceRunContext context = this.selectedDevices[deviceName];
                context.SynchronizationContext = syncCtx;
                context.Callback = callback;

                this.RunDevice(context);

            }
            return true;
        }

        private bool RunDevice(DeviceRunContext context)
        {
            string path = GetDevicePath(context.DeviceName, context.Version);
            if (Directory.Exists(path))
            {
                string deviceCfgFile = string.Format("{0}\\{1}", path, DeviceConfigFile);
                // TODO: Config file reading
                if (deviceCfgFile != null)
                {
                    DeviceEntry entry = ReadConfigFile(context.DeviceName, deviceCfgFile);

                    Device device = Load(entry);
                    if (device != null)
                    {
                        context.Device = device;

                        // Set thread-sync-context
                        device.SynchronizationContext = context.SynchronizationContext;
                        // Set data-received callback
                        device.DataReceived += context.Callback;

                        // Load the address from the d2d.m
                        string address = this.GetCOMPort(context.DeviceName);

                        string deviceLoadedStr = string.Format("Device: '{0}' Loaded @ '{1}'", entry[DeviceEntry.Identity], address);
                        RecordManager.DoSystemEventRecord(device, deviceLoadedStr);

                        device.Start(address);
                        return true;
                    }
                }
            }

            return false;
        }

		public void Initialize()
		{
            // TODO: Remove the param;
            this.LoadDevicesInfo(MainApplication.InstallPath);
		}

		public void ShutdownDeviceConnection()
		{
            /*
			foreach (Device device in this.devices)
			{
				device.Stop();
			}
             * */
		}

        private string GetCOMPort(string deviceName)
        {
            string deviceKey = deviceName.ToLower();
            if (this.d2d.ContainsKey(deviceKey))
            {
                return this.d2d[deviceKey];
            }
            return string.Empty;
        }

        /// <summary>
        /// Update the device last modify time.
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <param name="p"></param>
        internal void UpdateLastModifyTime(string deviceKey, long p)
        {
            if (this.lastUpdateDict.ContainsKey(deviceKey))
            {
                this.lastUpdateDict[deviceKey] = p;
            }
            else
            {
                this.lastUpdateDict.Add(deviceKey, p);
            }
        }

        // [Notice]; The the device no data arrived at the very beginning.
        // TODO: Should alert at all!
        internal void CheckLastModifyTime()
        {
            // At the very beginning, a device has NO data received, that case Not in rescue case.
            // So, the this.lastUpdateDict would NOT contain the device's last modify info.
            long now = DateTime.Now.Ticks;
            foreach (string deviceKey in this.lastUpdateDict.Keys)
            {
                long lastModifyTime = this.lastUpdateDict[deviceKey];
                long diffInSec = (now - lastModifyTime) / 10000000;
                if (diffInSec > 60 * 5)
                {
                    this.RescueDevice(deviceKey);
                }
            }
        }

        private void RescueDevice(string deviceKey)
        {
            DeviceRunContext context = this.selectedDevices[deviceKey];

            if (context != null)
            {
                Device badDevice = context.Device;
                const string DeviceWillRestart = "The device will restart now.";
                RecordManager.DoSystemEventRecord(badDevice, DeviceWillRestart);
                if (badDevice != null)
                {
                    badDevice.Stop();
                }
                this.RunDevice(context);
            }
        }
    }
}
