using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Scada.Declare
{
	public class StandardDevice : Device
	{
		private const int ComDataBits = 8;

        private DeviceEntry entry = null;

		private SerialPort serialPort = null;

		private int readTimeout = 12000;		//Receive timeout

		private int baudRate = 19200;

        private int dataBits = 8;

        private StopBits stopBits = StopBits.One;

        private Parity parity = Parity.None;

        private string lineBreak = "\n"; 

        private StringBuilder contentBuffer = new StringBuilder();



		public StandardDevice(DeviceEntry entry)
		{
            this.entry = entry;
            this.Name = entry[DeviceEntry.Name].ToString();
            this.Version = entry[DeviceEntry.Version].ToString();

            IValue baudRate = entry[DeviceEntry.BaudRate];
            if (baudRate != null)
            {
                this.baudRate = (StringValue)baudRate;
            }

            IValue readTimeout = entry[DeviceEntry.ReadTimeout];
            if (readTimeout != null)
            {
                this.readTimeout = (StringValue)readTimeout;
            }

            IValue dataBits = entry[DeviceEntry.DataBits];
			this.dataBits = (dataBits != null) ? (StringValue)dataBits : ComDataBits;

            StringValue stopBits = (StringValue)entry[DeviceEntry.StopBits];
			this.stopBits = (stopBits != null) ? (StopBits)(int)stopBits : StopBits.One;

            StringValue parity = (StringValue)entry[DeviceEntry.Parity];
            if (parity != null)
            {
                if (parity == "None")
                {
                    this.parity = Parity.None;
                }
                else if (parity == "Odd")
                {
                    this.parity = Parity.Odd;
                }
                else if (parity == "Even")
                {
                    this.parity = Parity.Even;
                }
            }

		}


		public bool Connect(string portName)
		{
			this.serialPort = new SerialPort(portName);

			try
			{
				this.serialPort.BaudRate = this.baudRate;

				this.serialPort.Parity = this.parity; //Parity none
                this.serialPort.StopBits = (StopBits)this.stopBits;    //StopBits 1
                this.serialPort.DataBits = this.dataBits;   // DataBits 8bit
                this.serialPort.ReadTimeout = this.readTimeout;
				
                this.serialPort.RtsEnable = true;
				this.serialPort.NewLine = "/r";
				this.serialPort.DataReceived += this.SerialPortDataReceived;
				this.serialPort.Open();

				


			}
			catch (IOException e)
			{
				string message = e.Message;
				// TODO: Log here.
				return false;
			}


			return true;
		}

		void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)  
		{
			int n = this.serialPort.BytesToRead;
			byte[] buffer = new byte[n];
			
			int r = this.serialPort.Read(buffer, 0, n);
            
			string line = Encoding.ASCII.GetString(buffer, 0, r);
			contentBuffer.Append(line);

            string content = contentBuffer.ToString();
            int p = content.IndexOf(lineBreak);
            string data = content.Substring(0, p);

            if (this.DataReceived != null)
            {
                this.DataReceived(sender, data);
            }

		}
		
		public bool ReadData()
		{
			//string line = this.serialPort.ReadLine();

			//Debug.WriteLine(line);
			return true;
		}

        public override void Run()
        {
            // TODO: call method Connect to connect the Serial-Port.
            Connect("COM4");
            // 
        }


		
	}
}
