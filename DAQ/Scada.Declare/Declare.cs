using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scada.Declare
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="deviceName"></param>
	/// <param name="data"></param>
	/// <returns></returns>
    public delegate bool OnDataReceived(object sender, string deviceName, string data);


	public struct DeviceData
	{
		private string line;

		private Device device;

		private int delay;

		private Action action;

		public DeviceData(Device device, string line)
		{
			this.device = device;
			this.line = line;
			this.delay = 0;
			this.action = null;
		}

		public string Line
		{
			get { return this.line; }
		}

		public Device Device
		{
			get { return this.device; }
		}

		public int Delay
		{
			get { return this.delay; }
			set { this.delay = value; }
		}

		public Action Action
		{
			get { return this.action; }
			set { this.action = value; }
		}
	}

    /// <summary>
    /// 
    /// </summary>
    public abstract class Device
    {
		private string name;

		private string version;

        private bool running = false;

        private SynchronizationContext synchronizationContext;

        private SendOrPostCallback dataReceived;

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

		

        public SendOrPostCallback DataReceived
        {
            get { return this.dataReceived; }
            set { this.dataReceived = value; }
        }

        public SynchronizationContext SynchronizationContext
        {
            get { return this.synchronizationContext; }
            set { this.synchronizationContext = value; }
        }

        public abstract void Start(string address);

		public abstract void Stop();

		public abstract void Send(string data);

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

		public const string LineBreak = "LineBreak";

		public const string Virtual = "Virtual";

		public const string ActionCondition = "ActionCondition";

		public const string ActionSend = "ActionSend";

		public const string ActionDelay = "ActionDelay";


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
