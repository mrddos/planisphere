using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Scada.DataCenterAgent
{
    public class Settings
    {

        public const string DeviceKey_Hipc = "scada.hipc";

        public const string DeviceKey_Weather = "scada.weather";

        public const string DeviceKey_HvSampler = "scada.hvsampler";

        public const string DeviceKey_ISampler = "scada.isampler";

        public const string DeviceKey_Shelter = "scada.shelter";

        public const string DeviceKey_Dwd = "scada.dwd";

        public const string DeviceKey_NaI = "scada.naidevice";


        public string[] DeviceKeys = {
                                DeviceKey_Hipc, 
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
            doc.Load("agent.settings");

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
            this.Password = this.GetAttribute(siteNode, "password");
            
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

            Device device = new Device();
            device.TableName = tableName;
            device.Key = deviceKey;

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
            Device device = devices.Find((d) => { return d.Key == deviceKey; });
            if (device != null)
            {
                return device.TableName;
            }
            return string.Empty;
        }

        internal List<DeviceCode> GetCodes(string deviceKey)
        {
            Device device = devices.Find((d) => { return d.Key == deviceKey; });
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


        public string Password
        {
            get;
            private set;
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
    }
}
