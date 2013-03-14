using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Scada.Common;

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

        private int actionInterval = 0;

		private string linePattern = string.Empty;

		private string insertIntoCommand = string.Empty;

		//private string fieldsConfig = string.Empty;

		private FieldConfig[] fieldsConfig = null;

		private DataParser dataParser = null;
		// 
		private LineParser lineParser = null;

        private IMessageTimer senderTimer = null;

		private Timer timer = null;

        private string exampleLine;

		private string error = "No Error";


		public StandardDevice(DeviceEntry entry)
		{
            this.entry = entry;
			if (!this.Initialize(entry))
			{
				string initFailedEvent = string.Format("Device '{0}' initialized failed. Error is {1}.", entry[DeviceEntry.Identity], error);
				RecordManager.DoSystemEventRecord(this, initFailedEvent);
			}

		}

        ~StandardDevice()
        {
        }

		private bool Initialize(DeviceEntry entry)
		{
			this.Name = entry[DeviceEntry.Name].ToString();
            this.Path = entry[DeviceEntry.Path].ToString();
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
			this.parity = SerialPorts.ParseParity(parity);

			// Line-Data Parser and LineBreak confing. 
			this.lineParser = new LineParser();
            string lineBreak = (StringValue)entry[DeviceEntry.LineBreak];
            lineBreak = lineBreak.Replace("\\r", "\r");
            lineBreak = lineBreak.Replace("\\n", "\n");
            this.lineParser.LineBreak = lineBreak;

			this.dataParser = new DataParser();
			// Virtual On
			string isVirtual = (StringValue)entry[DeviceEntry.Virtual];
			if (isVirtual != null && isVirtual.ToLower() == "true")
			{
				this.isVirtual = true;
			}

			this.actionCondition = (StringValue)entry[DeviceEntry.ActionCondition];
			this.actionSend = (StringValue)entry[DeviceEntry.ActionSend];
			this.actionDelay = (StringValue)entry[DeviceEntry.ActionDelay];

            var interval = entry[DeviceEntry.ActionInterval];
            if (interval != null)
            {
                this.actionInterval = (StringValue)interval;
            }

			this.linePattern = (StringValue)entry[DeviceEntry.Pattern];
			this.dataParser.Pattern = this.linePattern;

			string tableName = (StringValue)entry[DeviceEntry.TableName];
			string tableFields = (StringValue)entry[DeviceEntry.TableFields];

			string[] fields = tableFields.Split(',');
			string atList = string.Empty;
			for (int i = 0; i < fields.Length; ++i)
			{
				string at = string.Format("@{0}, ", i + 1);
				atList += at;
			}
			atList = atList.TrimEnd(',', ' ');

			string cmd = string.Format("insert into {0}({1}) values({2})", tableName, tableFields, atList);
			this.insertIntoCommand = cmd;

			string fieldsConfigStr = (StringValue)entry[DeviceEntry.FieldsConfig];
			string[] fieldsConfig = fieldsConfigStr.Split(',');
			List<FieldConfig> fieldConfigList = new List<FieldConfig>();
			for (int i = 0; i < fieldsConfig.Length; ++i)
			{
				string config = fieldsConfig[i];
				config = config.Trim();
				if (config == "Now")
				{
					FieldConfig fc = new FieldConfig(FieldType.TimeNow);
					fc.type = FieldType.TimeNow;
					fieldConfigList.Add(fc);
				}
				else if (config.StartsWith("#"))
				{
					FieldConfig fc = new FieldConfig(FieldType.String);
					fc.type = FieldType.String;
					fc.index = int.Parse(config.Substring(1));
					fieldConfigList.Add(fc);
				}
				else if (config == "int")
				{
					FieldConfig fc = new FieldConfig(FieldType.Int);
					fc.type = FieldType.Int;
					fieldConfigList.Add(fc);
				}

			}
			this.fieldsConfig = fieldConfigList.ToArray<FieldConfig>();
            this.exampleLine = (StringValue)entry[DeviceEntry.ExampleLine];
			return true;
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
                this.serialPort.NewLine = "/r/n";	//?
                this.serialPort.DataReceived += this.SerialPortDataReceived;
				if (!this.IsVirtual)
				{
					this.serialPort.Open();

                    if (this.actionInterval > 0)
                    {
                        this.StartSenderTimer(this.actionInterval);
                    }

                    /* TODO: Remove after test.
                    if (this.actionCondition == null || this.actionCondition.Length == 0)
                    {
                        this.Send(this.actionSend);
                    }
                    */
				}
				else
				{
                    this.StartVirtualDevice();
				}

            }
            catch (IOException e)
            {
                string message = e.Message;

                // TODO: Do something when can NOT open the port.

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

        private void StartSenderTimer(int interval)
        {
            if (MainApplication.TimerCreator != null)
            {
                this.senderTimer = MainApplication.TimerCreator.CreateTimer(interval);
                this.senderTimer.Start(() => 
                {
                    this.Send(this.actionSend);
                });

            }
        }

        private void StartVirtualDevice()
        {
            if (this.actionInterval > 0)
            {
                this.StartSenderTimer(this.actionInterval);
            }
            else if (!string.IsNullOrEmpty(this.actionCondition))
            {
                this.Send(this.actionSend);
            }
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
                if (!string.IsNullOrEmpty(line))
                {
                    DeviceData dd = this.GetDeviceData(line);

                    this.SynchronizationContext.Post(this.DataReceived, dd);
                }
				
			}
			catch (InvalidOperationException e)
			{
				Debug.WriteLine(e.Message);
				// !
			}
		}

		private object[] GetFieldsData(string[] data, FieldConfig[] fieldsConfig)
		{
			int count = fieldsConfig.Length;
			object[] ret = new object[count];
			for (int i = 0; i < count; ++i)
			{
				if (fieldsConfig[i].type == FieldType.TimeNow)
				{
					ret[i] = DateTime.Now;
				}
				else if (fieldsConfig[i].index >= 0)
				{
                    int index = fieldsConfig[i].index;
                    if (index > data.Length)
                        return null;
                    ret[i] = data[index];
				}
			}
			return ret;
		}

		private DeviceData GetDeviceData(string line)
		{
			string[] data = this.dataParser.Search(line);
            if (data == null || data.Length == 0)
                return default(DeviceData);
			object[] fields = GetFieldsData(data, this.fieldsConfig);
			DeviceData deviceData = new DeviceData(this, fields);
			if (IsActionCondition(line))
			{
				deviceData.Delay = this.actionDelay;
				deviceData.Action = () =>
				{
					this.Send(this.actionSend);
				};
			}
			deviceData.InsertIntoCommand = this.insertIntoCommand;
			//deviceData.FieldsConfig = this.fieldsConfig;
			return deviceData;
		}

		private bool IsActionCondition(string line)
		{
            if (this.actionCondition == null || this.actionCondition == string.Empty)
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
            if (this.senderTimer != null)
            {
                this.senderTimer.Close();
            }

            if (this.serialPort != null && this.IsOpen)
            {
                this.serialPort.Close();
            }
			// 
		}

		public override void Send(string action)
		{
			if (this.serialPort != null && this.IsOpen)
			{
				if (this.IsVirtual == false)
				{
					byte[] buffer = Encoding.ASCII.GetBytes(action);
					this.serialPort.Write(buffer, 0, buffer.Length);
				}
				else
				{
                    this.OnSendDataToVirtualDevice(action);					
				}
			}

		}

        private void OnSendDataToVirtualDevice(string action)
        {
            if (action == this.actionSend)
            {
                if (this.actionInterval > 0)
                {
                    // Depends on the WinFormTimer.
                    string line = this.GetExampleLine();
                    DeviceData dd = this.GetDeviceData(line);

                    this.SynchronizationContext.Post(this.DataReceived, dd);
                }
                else
                {
                    this.timer = new Timer(new TimerCallback((object state) =>
                    {
                        string line = this.GetExampleLine();
                        DeviceData dd = this.GetDeviceData(line);

                        this.SynchronizationContext.Post(this.DataReceived, dd);
                    }), null, 1000, 2000);
                }
            }	
        }

        private string GetExampleLine(int rand = 0)
        {
            // TODO: Maybe need random example lines.
            return this.exampleLine;
        }


		
	}
}
