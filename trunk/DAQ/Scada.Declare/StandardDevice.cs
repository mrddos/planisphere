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

       
		// 
		private LineParser lineParser = null;


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
				else if (parity == "Mark")
				{
					this.parity = Parity.Mark;
				}
				else if (parity == "Space")
				{
					this.parity = Parity.Space;
				}
            }

			this.lineParser = new LineParser();

		}

        ~StandardDevice()
        {
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
                this.serialPort.Open();
                // this.serialPort.Close();
                // this.serialPort.Open();

                bool isOpen = this.serialPort.IsOpen;

                int tid = Thread.CurrentThread.ManagedThreadId;


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
			try
			{

				int n = this.serialPort.BytesToRead;
				byte[] buffer = new byte[n];
			
				int r = this.serialPort.Read(buffer, 0, n);
            
				string data = Encoding.ASCII.GetString(buffer, 0, r);


				if (this.DataReceived != null)
				{
					string line = this.lineParser.ContinueWith(data);
					DeviceData dd = new DeviceData(this, line);
					

                    this.SynchronizationContext.Post(this.DataReceived, dd);
				}
			}
			catch (InvalidOperationException e)
			{
				Debug.WriteLine(e.Message);
				if (this.DataReceived != null)
				{
                    this.SynchronizationContext.Post(this.DataReceived, null);
				}
			}
		}
		
		public bool ReadData()
		{
			//string line = this.serialPort.ReadLine();

			//Debug.WriteLine(line);
			return true;
		}

		public override void Start(string address)
        {
			address = "COM5";
            // TODO: call method Connect to connect the Serial-Port.
			this.Connect(address);
            // 
        }


		public override void Stop()
		{

			// 
		}


		
	}
}
