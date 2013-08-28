﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Scada.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceEntry
    {
        public const string Name = "Name";

        public const string Path = "path";

        public const string Identity = "id";

        public const string Version = "Version";

        public const string ClassName = "ClassName";

        public const string Assembly = "Assembly";

        public const string BaudRate = "BaudRate";

        public const string DataBits = "DataBits";

        public const string StopBits = "StopBits";

        public const string SerialPort = "SerialPort";

        public const string IPAddress = "IPAddress";

        public const string IPPort = "IPPort";

        public const string DeviceSn = "DeviceSn";


        public const string ReadTimeout = "ReadTimeout";

        public const string Parity = "Parity";

        public const string LineBreak = "LineBreak";

        public const string Virtual = "Virtual";

        public const string ActionCondition = "ActionCondition";

        public const string ActionSendInHex = "ActionSendInHex";

        public const string ActionSend = "ActionSend";

        public const string ActionDelay = "ActionDelay";

        public const string ActionInterval = "ActionInterval";

        public const string RecordInterval = "RecordInterval";

        public const string Pattern = "Pattern";

        public const string Sensitive = "Sensitive";

        public const string DataParser = "DataParser";

        public const string TableName = "TableName";

        public const string TableFields = "TableFields";

        public const string FieldsConfig = "FieldsConfig";

        public const string ExampleLine = "ExampleLine";


        private Dictionary<string, IValue> dict = new Dictionary<string, IValue>();

        public DeviceEntry()
        {
        }

        public IValue this[string name]
        {
            get
            {
                return dict.ContainsKey(name) ? dict[name] : null;
            }

            set
            {
                dict.Add(name, value);
            }
        }

        public bool Contains(string name)
        {
            return dict.ContainsKey(name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static DeviceEntry ReadConfigFile(string deviceName, string configFile)
        {
            return DeviceEntry.LoadFromConfig(deviceName, configFile);
        }

        private static DeviceEntry LoadFromConfig(string deviceName, string configFile)
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
                string virtualDeviceFlagFile = devicePath + "\\virtual-device";
                if (File.Exists(virtualDeviceFlagFile))
                {
                    /*
                    string caption = "连接虚拟设备提示";
                    string message = string.Format("是否要连接 '{0}' 的虚拟设备，连接虚拟设备点击‘是’，\n连接真实设备点击‘否’", deviceName);
                    DialogResult dr = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        entry[DeviceEntry.Virtual] = new StringValue("true");
                    }
                    else
                    {
                        entry[DeviceEntry.Virtual] = new StringValue("false");

                        string deleteVirtualFileMsg = string.Format("是否要删除 '{0}' 的虚拟设备标志文件？", deviceName);
                        DialogResult del = MessageBox.Show(deleteVirtualFileMsg, caption, MessageBoxButtons.YesNo);
                        if (del == DialogResult.Yes)
                        {
                            File.Delete(virtualDeviceFlagFile);
                        }

                    }
                    */
                }
                return entry;
            }
        }

        public static byte[] ParseHex(string line)
        {
            string[] hexArray = line.Split(' ');
            List<byte> bs = new List<byte>();
            foreach (string hex in hexArray)
            {
                byte b = (byte)int.Parse(hex, NumberStyles.AllowHexSpecifier);
                bs.Add(b);
            }
            return bs.ToArray<byte>();
        }

    }

}
