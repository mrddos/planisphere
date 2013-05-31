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
            private string TableName
            {
                get;
                set;
            }

            private List<DeviceCode> codes = new List<DeviceCode>();


            public void AddCode(string code, string field)
            {
                this.codes.Add(new DeviceCode() { Code = code, Field = field });
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

            var datacenters = doc.SelectNodes("//datacenter");
            foreach (var dc in datacenters)
            {

            }


            var devices = doc.SelectNodes("//devices/device");
            foreach (XmlNode deviceNode in devices)
            {
                Device device = new Device();
                this.devices.Add(device);
                var codes = deviceNode.SelectNodes("code");
                foreach (XmlNode codeNode in codes)
                {
                    string code = codeNode.InnerText;
                    XmlNode fieldNode = codeNode.Attributes.GetNamedItem("field");
                    device.AddCode(code, fieldNode.Value);
                }
            }
        }

        public List<DataCenter> DataCenters
        {
            get
            {
                return this.dataCenters;
            }
        }
    }
}
