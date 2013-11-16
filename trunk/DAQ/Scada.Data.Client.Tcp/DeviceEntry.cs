using Scada.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Data.Client.Tcp
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

        public const string Pattern = "Pattern";

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


    }
}
