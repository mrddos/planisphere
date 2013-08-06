﻿using Scada.Declare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Scada.DataCenterAgent
{
    /// <summary>
    /// 
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// 
        /// </summary>
        /// !
        /// 
        public const string DeviceKey_Hpic = "scada.hpic";

        public const string DeviceKey_Weather = "scada.weather";

        public const string DeviceKey_HvSampler = "scada.hvsampler";

        public const string DeviceKey_ISampler = "scada.isampler";

        public const string DeviceKey_Shelter = "scada.shelter";

        public const string DeviceKey_Dwd = "scada.dwd";

        public const string DeviceKey_NaI = "scada.naidevice";


        public string[] DeviceKeys = {
                                DeviceKey_Hpic, 
                                DeviceKey_Weather, 
                                DeviceKey_HvSampler, 
                                DeviceKey_ISampler, 
                                DeviceKey_Shelter,
                                DeviceKey_Dwd,  
                                DeviceKey_NaI
                                     };

        public static Settings Instance = new Settings();

        /// <summary>
        /// 
        /// </summary>
        public class DataCenter
        {
            public string Ip { get; set; }
            public string WirelessIp { get; set; }

            public int Port { get; set; }
            public int WirelessPort { get; set; }

        }

        /// <summary>
        /// 
        /// </summary>
        public class DeviceCode
        {
            public string Code
            {
                get;
                set;
            }

            public string Field
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public class Device
        {
            public string TableName
            {
                get;
                set;
            }

            private List<DeviceCode> codes = new List<DeviceCode>();


            public void AddCode(string code, string field)
            {
                this.codes.Add(new DeviceCode() { Code = code, Field = field });
            }


            public string Key { get; set; }

            public string EquipNumber { get; set; }

            internal List<DeviceCode> GetCodes()
            {
                return this.codes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private XmlDocument doc = new XmlDocument();

        private List<DataCenter> dataCenters = new List<DataCenter>();

        private List<Device> devices = new List<Device>();

        public Settings()
        {
            string settingFileName = "agent.settings";
            if (File.Exists(settingFileName))
            {
                doc.Load(settingFileName);
            }

            // Data Center
            var datacenters = doc.SelectNodes("//datacenter");
            foreach (XmlNode dcn in datacenters)
            {
                DataCenter dc = new DataCenter();
                
                dc.Ip = this.GetAttribute(dcn, "ip");
                dc.Port = int.Parse(this.GetAttribute(dcn, "port", "0"));
                dc.WirelessIp = this.GetAttribute(dcn, "wirelessip");
                dc.WirelessPort = int.Parse(this.GetAttribute(dcn, "wirelessport", "0"));

                dataCenters.Add(dc);
            }

            // Site
            var siteNode = doc.SelectNodes("//site")[0];
            this.SysName = this.GetAttribute(siteNode, "sysname");
            this.SysSt = this.GetAttribute(siteNode, "sysst");
            this.Mn = this.GetAttribute(siteNode, "mn");
            this.Sno = this.GetAttribute(siteNode, "sno");

            this.LoadPassword();
            
            // Devices
            var devices = doc.SelectNodes("//devices/device");
            foreach (XmlNode deviceNode in devices)
            {
                Device device = this.ParseDeviceNode(deviceNode);
                this.devices.Add(device);
                var codes = deviceNode.SelectNodes("code");
                foreach (XmlNode codeNode in codes)
                {
                    string code = codeNode.InnerText;
                    XmlNode fieldNode = codeNode.Attributes.GetNamedItem("field");
                    if (fieldNode != null)
                    {
                        device.AddCode(code, fieldNode.Value);
                    }
                }
            }


            // Load NaI device config.
            // TODO:
            this.NaIFilePath = string.Format("{0}\\devices\\Scada.NaIDevice\\0.9", Environment.CurrentDirectory);
            string path = string.Format("{0}\\device.cfg", this.NaIFilePath);

            DeviceEntry entry = LoadFromConfig("Scada.NaIDevice", path);

            this.NaIDeviceSn = (StringValue)entry["DeviceSn"];
            this.MinuteAdjust = (StringValue)entry["MinuteAdjust"];
        }

        private Device ParseDeviceNode(XmlNode deviceNode)
        {
            var tableNameNode = deviceNode.Attributes.GetNamedItem("table");
            string tableName = string.Empty;
            if (tableNameNode != null)
            {
                tableName = tableNameNode.Value;
            }

            var idNode = deviceNode.Attributes.GetNamedItem("id");
            string deviceKey = string.Empty;
            if (idNode != null)
            {
                deviceKey = idNode.Value;
            }

            var equipNode = deviceNode.Attributes.GetNamedItem("eno");
            string equipNumber = string.Empty;
            if (equipNode != null)
            {
                equipNumber = equipNode.Value;
            }

            Device device = new Device();
            device.TableName = tableName;
            device.Key = deviceKey;
            device.EquipNumber = equipNumber;
            return device;
        }

        public List<DataCenter> DataCenters
        {
            get
            {
                return this.dataCenters;
            }
        }

        internal string GetTableName(string deviceKey)
        {
            Device device = devices.Find((d) => { return deviceKey.Equals(d.Key, StringComparison.OrdinalIgnoreCase)  ; });
            if (device != null)
            {
                return device.TableName;
            }
            return string.Empty;
        }

        internal string GetEquipNumber(string deviceKey)
        {
            Device device = devices.Find((d) => { return deviceKey.Equals(d.Key, StringComparison.OrdinalIgnoreCase); });
            if (device != null)
            {
                return device.EquipNumber;
            }
            return string.Empty;
        }

        internal List<DeviceCode> GetCodes(string deviceKey)
        {
            Device device = devices.Find((d) => { return deviceKey.Equals(d.Key, StringComparison.OrdinalIgnoreCase); });
            if (device != null)
            {
                return device.GetCodes();
            }
            return new List<DeviceCode>();
        }

        private string GetAttribute(XmlNode node, string attr)
        {
            var xmlAttr = node.Attributes.GetNamedItem(attr);
            return xmlAttr.Value; 
        }

        private string GetAttribute(XmlNode node, string attr, string defaultValue = "")
        {
            var xmlAttr = node.Attributes.GetNamedItem(attr);
            if (xmlAttr != null)
            {
                if (!string.IsNullOrEmpty(xmlAttr.Value))
                {
                    return xmlAttr.Value;
                }
            }
            return defaultValue;
        }

        private string password = string.Empty;

        public string Password
        {
            get
            {
                if (this.password == string.Empty)
                {
                    this.LoadPassword();
                }
                return password;
            }

            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.UpdatePassword(value);
                }
            }

        }

        private void LoadPassword()
        {
            using (StreamReader sr = new StreamReader("password"))
            {
                this.password = sr.ReadLine();
            }
        }

        private void UpdatePassword(string password)
        {
            using (StreamWriter sw = new StreamWriter("password"))
            {
                sw.WriteLine(password);
            }
        }

        public string SysName
        {
            get;
            private set;
        }

        public string SysSt
        {
            get;
            private set;
        }

        public string Mn
        {
            get;
            set;
        }

        public string Sno
        {
            get;
            private set;
        }


        public DateTime CurrentTime
        {
            set { }
            get
            {
                return DateTime.Now;
            }
        }

        internal string GetDeviceKeyByEno(string eno)
        {
            Device device = devices.Find((d) => { return eno.Equals(d.EquipNumber, StringComparison.OrdinalIgnoreCase); });
            if (device != null)
            {
                return device.Key;
            }
            return string.Empty;
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

        public string NaIDeviceSn { get; set; }

        public int MinuteAdjust { get; set; }

        public string NaIFilePath { get; set; }
    }
}
