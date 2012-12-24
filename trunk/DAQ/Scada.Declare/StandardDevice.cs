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
        private DeviceEntry entry = null;

		private SerialPort serialPort = null;

		private int readTimeout = 12000;		//Receive timeout

		private int baudRate = 19200;

        private int dataBits = 8;

        private StopBits stopBits = StopBits.One;

        private Parity parity = Parity.None;

		private StringBuilder content = new StringBuilder();



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
            if (dataBits != null)
            {
                this.dataBits = (StringValue)dataBits;
            }

            StringValue stopBits = (StringValue)entry[DeviceEntry.StopBits];
            if (stopBits != null)
            {
                this.stopBits = (StopBits)int.Parse(stopBits);
            }

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
			int n = this.serialPort.BytesToRead; //先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
			byte[] buffer = new byte[n];//声明一个临时数组存储当前来的串口数据  
			//received_count += n;//增加接收计数  
			this.serialPort.Read(buffer, 0, n);//读取缓冲数据  
			string line = Encoding.ASCII.GetString(buffer);
			content.Append(line);
			// Debug.WriteLine(line);
			// Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
			

			// TODO: Deal-with the Data.
			// If we have a completed


            if (this.DataReceived != null)
            {
                string data = string.Empty;
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
