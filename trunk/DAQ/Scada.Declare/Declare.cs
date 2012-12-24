using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{

    public delegate bool OnDataReceived(object sender, string data);

    /// <summary>
    /// 
    /// </summary>
    public abstract class Device
    {
		private string name;

		private string version;

        private bool running = false;

        private OnDataReceived dataReceived;

        /// <summary>
        /// 
        /// </summary>
        public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        public string Version
		{
            get { return this.version; }
			set { this.version = value; }
		}

        public bool Running
        {
            get { return this.running; }
        }

        public OnDataReceived DataReceived
        {
            get { return this.dataReceived; }
            set { this.dataReceived = value; }
        }

        public abstract void Run();


    }

    /// <summary>
    /// 
    /// </summary>
    public class DeviceEntry
    {
        public const string Name = "Name";

        public const string Version = "Version";

        public const string ClassName = "ClassName";

        public const string Assembly = "Assembly";

        public const string BaudRate = "BaudRate";

        public const string DataBits = "DataBits";

        public const string StopBits = "StopBits";

        public const string ReadTimeout = "ReadTimeout";

        public const string Parity = "Parity";


        private Dictionary<string, IValue> dict = new Dictionary<string, IValue>();

        public DeviceEntry()
        {
            dict[ClassName] = new StringValue("Scada.Declare.StandardDevice");
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
        
        
    }

}
