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
		private SerialPort serialPort = null;

		private const int ComDataBits = 8;			//Port data-bits

		private int readTimeout = 12000;		//Receive timeout

		private int baudRate = 19200;

		private StringBuilder content = new StringBuilder();

		public StandardDevice(string deviceName)
		{
			this.Name = deviceName;
		}


		public int BaudRate
		{
			get { return this.baudRate; }
			set { this.baudRate = value; }
		}

		public int ReadTimeout
		{
			get { return this.readTimeout; }
			set { this.readTimeout = value; }
		}

		public bool Connect(string portName)
		{
			this.serialPort = new SerialPort(portName);

			try
			{
				this.serialPort.BaudRate = this.BaudRate;

				this.serialPort.Parity = Parity.None; //Parity none
				this.serialPort.StopBits = StopBits.One; //StopBits 1
				this.serialPort.DataBits = ComDataBits;        // DataBits 8bit
				this.serialPort.ReadTimeout = -1;
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

            // 
        }


		
	}
}
