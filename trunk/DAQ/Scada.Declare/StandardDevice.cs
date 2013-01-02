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

		private bool isVirtual = false;

		private string actionCondition = string.Empty;

		private string actionSend = string.Empty;

		private int actionDelay = 0;
       
		// 
		private LineParser lineParser = null;


		public StandardDevice(DeviceEntry entry)
		{
            this.entry = entry;
			this.Initialize(entry);
		}

        ~StandardDevice()
        {
        }

		private void Initialize(DeviceEntry entry)
		{
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
				else if (parity == "Mark")
				{
					this.parity = Parity.Mark;
				}
				else if (parity == "Space")
				{
					this.parity = Parity.Space;
				}
			}

			// Line-Data Parser and LineBreak confing. 
			this.lineParser = new LineParser();
			this.lineParser.LineBreak = (StringValue)entry[DeviceEntry.LineBreak];

			// Virtual On
			string isVirtual = (StringValue)entry[DeviceEntry.Virtual];
			if (isVirtual != null && isVirtual.ToLower() == "true")
			{
				this.isVirtual = true;
			}

			this.actionCondition = (StringValue)entry[DeviceEntry.ActionCondition];
			this.actionSend = (StringValue)entry[DeviceEntry.ActionSend];
			this.actionDelay = (StringValue)entry[DeviceEntry.ActionDelay];
		}

		public bool IsVirtual
		{
			get { return this.isVirtual; }
		}

		private bool IsOpen
		{
			get
			{
				return this.IsVirtual ? true : this.serialPort.IsOpen;
			}
		}

		public bool Connect(string portName)
		{
            try
            {
                this.serialPort = new SerialPort(portName);

                this.serialPort.BaudRate = this.baudRate;

                this.serialPort.Parity = this.parity; //Parity none
                this.serialPort.StopBits = StopBits.One; //(StopBits)this.stopBits;    //StopBits 1
                this.serialPort.DataBits = 8;// this.dataBits;   // DataBits 8bit
                this.serialPort.ReadTimeout = 10000;// this.readTimeout;

                this.serialPort.RtsEnable = true;
                this.serialPort.NewLine = "/r/n";
                this.serialPort.DataReceived += this.SerialPortDataReceived;
				if (!this.IsVirtual)
				{
					this.serialPort.Open();
				}
				else
				{
					this.Send(this.actionSend);
				}

            }
            catch (IOException e)
            {
                string message = e.Message;

                // SerialDataReceivedEventArgs events = null;
                // this.SerialPortDataReceived(null, events);

                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


			return true;
		}

		private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs evt)  
		{
			Debug.Assert(this.DataReceived != null);

			try
			{
				int n = this.serialPort.BytesToRead;
				byte[] buffer = new byte[n];
			
				int r = this.serialPort.Read(buffer, 0, n);
				string data = Encoding.ASCII.GetString(buffer, 0, r);

				string line = this.lineParser.ContinueWith(data);
				DeviceData dd = this.GetDeviceData(line);

				this.SynchronizationContext.Post(this.DataReceived, dd);
				
			}
			catch (InvalidOperationException e)
			{
				Debug.WriteLine(e.Message);
				// !
			}
		}

		private DeviceData GetDeviceData(string line)
		{
			DeviceData deviceData = new DeviceData(this, line);
			if (IsActionCondition(line))
			{
				deviceData.Delay = this.actionDelay;
				deviceData.Action = () =>
				{
					this.Send(this.actionSend);
				};
			}
			return deviceData;
		}

		private bool IsActionCondition(string line)
		{
			if (this.actionCondition == string.Empty)
			{
				return true;
			}
			if (line.IndexOf(this.actionCondition) >= 0)
			{
				return true;
			}
			return false;
		}

		public override void Start(string address)
        {
			// address = "COM5";
            // TODO: call method Connect to connect the Serial-Port.
			this.Connect(address);
            // 
        }


		public override void Stop()
		{

			// 
		}

		public override void Send(string data)
		{
			if (this.serialPort != null && this.IsOpen)
			{
				if (this.IsVirtual == false)
				{
					byte[] buffer = Encoding.ASCII.GetBytes(data);
					this.serialPort.Write(buffer, 0, buffer.Length);
				}
				else
				{
					if (data == this.actionSend)
					{
						Timer timer = new Timer(new TimerCallback((object state) =>
						{
							string line = @".0000   .0000   .0000   .0000   .5564   383.0   6.136   28.40   .0000 ";
							DeviceData dd = this.GetDeviceData(line);

							this.SynchronizationContext.Post(this.DataReceived, dd);
						}), null, 1000, 2000);
					}					
				}
			}

		}


		
	}
}
