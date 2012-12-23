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

        private int baudRate = 9600;

        private string name;

        private string version;

        private string className = "Scada.Declare.StandardDevice";

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        public int BaudRate
        {
            get { return this.baudRate; }
            set { this.baudRate = value; }
        }

        public string ClassName
        {
            get { return this.className; }
            set { this.className = value; }
        }

    }

}
