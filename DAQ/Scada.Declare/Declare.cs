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

	public enum FieldType
	{
		Null,
		String,
		Int,
		Time,
		TimeNow,
	}

	public class FieldConfig
	{
		public FieldType type;

		public int index;

		public FieldConfig(FieldType type)
		{
			this.type = type;
			this.index = -1;
		}
	}


	public struct DeviceData
	{
		private object[] data;

		private Device device;

		private int delay;

		private Action action;

		private string insertIntoCommand;

		// private FieldConfig[] fieldsConfig;

		public DeviceData(Device device, object[] data)
		{
			this.device = device;
			this.data = data;
			this.delay = 0;
			this.action = null;
			this.insertIntoCommand = string.Empty;
			// this.fieldsConfig = null;
		}

		public object[] Data
		{
			get { return this.data; }
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

		public string InsertIntoCommand
		{
			get { return this.insertIntoCommand; }
			set { this.insertIntoCommand = value; }
		}
		/*
		public FieldConfig[] FieldsConfig
		{
			get { return this.fieldsConfig; }
			set { this.fieldsConfig = value; }
		}
		*/
	}

    /// <summary>
    /// 
    /// </summary>
    public abstract class Device
    {
		private string name;

        private string path;

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

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
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

		public abstract void Send(byte[] action);

	}

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
        
        
    }

}
