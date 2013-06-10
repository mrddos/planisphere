﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Scada.Common;
using System.Reflection;
using System.Globalization;

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

		private bool actionSendInHex = false;

		private string actionCondition = string.Empty;

		private byte[] actionSend = null;

		private int actionDelay = 0;

        private int actionInterval = 0;

		private string linePattern = string.Empty;

		private string insertIntoCommand = string.Empty;

		//private string fieldsConfig = string.Empty;

		private FieldConfig[] fieldsConfig = null;

		private DataParser dataParser = null;
		

        private IMessageTimer senderTimer = null;

		private Timer timer = null;

		private bool handled = true;

        private string exampleLine;

		private List<byte> exampleBuffer = new List<byte>();


		private string error = "No Error";

		private const string ScadaDeclare = "Scada.Declare.";

        private int MaxDelay = 10;

        private DateTime currentActionTime = default(DateTime);


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
            this.Id = entry[DeviceEntry.Identity].ToString();
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

			
			// Virtual On
			string isVirtual = (StringValue)entry[DeviceEntry.Virtual];
			if (isVirtual != null && isVirtual.ToLower() == "true")
			{
				this.isVirtual = true;
			}

			this.actionCondition = (StringValue)entry[DeviceEntry.ActionCondition];
			string actionSendInHex = (StringValue)entry[DeviceEntry.ActionSendInHex];
			if (actionSendInHex != "true")
			{
				string actionSend = (StringValue)entry[DeviceEntry.ActionSend];
                if (actionSend != null)
                {
                    actionSend = actionSend.Replace("\\r", "\r");
                    this.actionSend = Encoding.ASCII.GetBytes(actionSend);
                }
			}
			else
			{
				this.actionSendInHex = true;
				string hexes = (StringValue)entry[DeviceEntry.ActionSend];
                hexes = hexes.Trim();
				this.actionSend = ParseHex(hexes);
			}

			this.actionDelay = (StringValue)entry[DeviceEntry.ActionDelay];


            var interval = entry[DeviceEntry.ActionInterval];
            if (interval != null)
            {
                this.actionInterval = (StringValue)interval;
            }

			// Set DataParser;
			this.SetDataParser();

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
            List<FieldConfig> fieldConfigList = ParseDataFieldConfig(fieldsConfigStr);
			this.fieldsConfig = fieldConfigList.ToArray<FieldConfig>();

			if (this.IsVirtual)
			{
				string el = (StringValue)entry[DeviceEntry.ExampleLine];
				el = el.Replace("\\r", "\r");
				el = el.Replace("\\n", "\n");

				this.exampleLine = el;
			}
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

                this.serialPort.Parity = this.parity;       //Parity none
                this.serialPort.StopBits = StopBits.One;    //(StopBits)this.stopBits;    //StopBits 1
                this.serialPort.DataBits = 8;               // this.dataBits;   // DataBits 8bit
                this.serialPort.ReadTimeout = 10000;        // this.readTimeout;

                this.serialPort.RtsEnable = true;
                this.serialPort.NewLine = "/r/n";	        //?
                this.serialPort.DataReceived += this.SerialPortDataReceived;
                // Real Devie begins here.
				if (!this.IsVirtual)
				{
					this.serialPort.Open();

					if (this.actionInterval > 0)
					{
						this.StartSenderTimer(this.actionInterval);
					}
					else
					{
                        if (this.actionSend != null && this.actionSend.Length > 0)
                        {
                            this.Send(this.actionSend, default(DateTime));
                        }
					}
                    SetStartStatus();


                    /* TODO: Remove after test.
                    if (this.actionCondition == null || this.actionCondition.Length == 0)
                    {
                        this.Send(this.actionSend);
                    }
                    */
				}
				else
				{
                    RecordManager.DoSystemEventRecord(this, "Notice, Virtual Device Started");
                    this.StartVirtualDevice();
				}

            }
            catch (IOException e)
            {
                string message = "IO: " + e.Message;
                RecordManager.DoSystemEventRecord(this, message);
                return false;
            }
            catch (Exception e)
            {
                string message = "Other: " + e.Message;
                RecordManager.DoSystemEventRecord(this, message);
            }

			return true;
		}

        private void StartSenderTimer(int interval)
        {
            if (MainApplication.TimerCreator != null)
            {
                // Trigger every 1s. 
                this.senderTimer = MainApplication.TimerCreator.CreateTimer(1);
                this.senderTimer.Start(() => 
                {
                    DateTime now = DateTime.Now;
                    DateTime rightTime = default(DateTime);
                    if (!this.IsRightTime(now, out rightTime))
                    {
                        return;
                    }

                    if (rightTime == this.currentActionTime)
                    {
                        return;
                    }

                    this.Send(this.actionSend, rightTime);
                });

            }
        }

        // Virtual-Device.
        private void StartVirtualDevice()
        {
            if (this.actionInterval > 0)
            {
                this.StartSenderTimer(this.actionInterval);
            }
            else if (this.actionInterval == 0)
            {
                this.OnSendDataToVirtualDevice(this.actionSend);
            }
            /*
            else if (!string.IsNullOrEmpty(this.actionCondition))
            {
                this.Send(this.actionSend);
            }
            */
        }

		private byte[] ReadData()
		{
			if (!this.isVirtual)
			{
                // Try to Sleep 100ms and make one time callback.
                // 200 ms is enough for BoudRate 9600
                Thread.Sleep(200);

				int n = this.serialPort.BytesToRead;
				byte[] buffer = new byte[n];

				int r = this.serialPort.Read(buffer, 0, n);

				return buffer;
			}
			else // Virtual Device~!
			{
				if (this.actionInterval > 1)
				{
					// 假设: 应答式的数据，都是完整的帧.
					return this.GetExampleLine();
				}
				else
				{
					if (this.actionSendInHex)
					{
						return this.GetExampleLine();
					}
					// 不完整帧模拟
					byte[] bytes = this.GetExampleLine();
					int len = bytes.Length;
					foreach (byte b in bytes)
					{
						exampleBuffer.Add(b);
					}

					int c = new Random().Next(len - 5, len + 5);
					int count = Math.Min(c, len);
					byte[] ret = new byte[count];

					for (int i = 0; i < count; ++i)
					{
						ret[i] = exampleBuffer[i];
					}
					exampleBuffer.RemoveRange(0, count);

					return ret;
				}
			}
		}

		private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs evt)  
		{
			Debug.Assert(this.DataReceived != null);
			try
			{
				handled = false;
				byte[] buffer = this.ReadData();

				byte[] line = this.dataParser.GetLineBytes(buffer);

				if (line.Length > 0)
				{
                    // Defect: HPIC need check the time.
                    if (this.actionInterval == 0)
                    {
                        DateTime now = DateTime.Now;
                        DateTime rightTime = default(DateTime);
                        if (!this.IsRightTime(now, out rightTime))
                        {
                            return;
                        }

                        if (this.currentActionTime == rightTime)
                        {
                            return;
                        }

                        this.currentActionTime = rightTime;
                    }

					DeviceData dd;
					bool got = this.GetDeviceData(line, this.currentActionTime, out dd);
					if (got)
					{
						this.SynchronizationContext.Post(this.DataReceived, dd);
					}
				}
			}
			catch (InvalidOperationException e)
			{
                RecordManager.DoSystemEventRecord(this, e.Message);
			}
			finally
			{
				handled = true;
			}
		}

        private void SetStartStatus()
        {
            DeviceData dd = new DeviceData(this, null);
            
            this.SynchronizationContext.Post(this.DataReceived, dd);
        }


		private bool GetDeviceData(byte[] line, DateTime time, out DeviceData dd)
		{
            if (time == default(DateTime))
            {
                time = DateTime.Now;
            }

			string[] data = this.dataParser.Search(line);
			dd = default(DeviceData);
			if (data == null || data.Length == 0)
			{
				return false;
			}
            dd.Time = time;
            object[] fields = Device.GetFieldsData(data, time, this.fieldsConfig);
			dd = new DeviceData(this, fields);
			dd.InsertIntoCommand = this.insertIntoCommand;
			// deviceData.FieldsConfig = this.fieldsConfig;
			return true;
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
            if (!this.Connect(address))
            {
                RecordManager.DoSystemEventRecord(this, "Connection Failure");
            }
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

		public override void Send(byte[] action, DateTime time)
		{
			if (this.serialPort != null && this.IsOpen)
			{
				if (!this.IsVirtual)
				{
                    // RecordManager.DoSystemEventRecord(this, Encoding.ASCII.GetString(action));
                    this.currentActionTime = time;
					this.serialPort.Write(action, 0, action.Length);
				}
                // Virtual-Device~!
				else
				{
                    this.currentActionTime = time;
                    this.OnSendDataToVirtualDevice(action);					
				}
			}

		}

        // Virtual !!
		private void OnSendDataToVirtualDevice(byte[] action)
        {
            if (Bytes.Equals(action, this.actionSend))
            {
                if (this.actionInterval > 0)
                {
					this.SerialPortDataReceived("virtual-device", null);
                }
                else
                {
                    this.timer = new Timer(new TimerCallback((object state) =>
                    {
						if (handled)
						{
							this.SerialPortDataReceived("virtual-device", null);
						}
					}), null, 2000, 5000);
                }
            }	
        }

        private byte[] GetExampleLine(int rand = 0)
        {
			if (actionSendInHex)
			{
				return ParseHex(this.exampleLine);
			}
			else
			{
				return Encoding.ASCII.GetBytes(this.exampleLine);
			}
        }

		private static byte[] ParseHex(string line)
		{
			string[] hexArray = line.Split(' ');
			List<byte> bs = new List<byte>();
			foreach (string hex in hexArray)
			{
				byte b = (byte)int.Parse(hex, NumberStyles.AllowHexSpecifier);
				bs.Add(b);
			}
			return bs.ToArray<byte>();
		}

		private void SetDataParser()
		{
			string dataParserClz = (StringValue)entry[DeviceEntry.DataParser];
			if (!dataParserClz.Contains('.'))
			{
				dataParserClz = ScadaDeclare + dataParserClz;
			}
			Assembly assembly = Assembly.GetAssembly(typeof(StandardDevice));
			Type deviceClass = assembly.GetType(dataParserClz);
			if (deviceClass != null)
			{
				object dataParser = Activator.CreateInstance(deviceClass, new object[] { });
				this.dataParser = (DataParser)dataParser;

                int i = 0;
                double v = 0.0;
                while (Device.GetFactor(entry, ++i, out v))
                {
                    this.dataParser.Factors.Add(v);
                }

			}
		}

        // VB form data every 30 sec.
        // Verify the time (second) is the right time.
        private bool IsRightTime(DateTime now, out DateTime rightTime)
        {
            int second = (now.Second < 30) ? 0 : 30;
            rightTime = default(DateTime);
            if (now.Second >= 0 && now.Second <= MaxDelay)
            {
                rightTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second);
                return true;
            }
            else if (now.Second >= 30 && now.Second <= (30 + MaxDelay))
            {
                rightTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second);
                return true;
            }
            return false;
        }


		
	}
}
