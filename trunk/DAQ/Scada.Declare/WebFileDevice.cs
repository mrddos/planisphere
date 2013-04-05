using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scada.Declare
{
	public class WebFileDevice : Device
	{
		private bool isVirtual = false;

		private DeviceEntry entry = null;


		private string addr = "127.0.0.1";

		private int port = 23;

		public WebFileDevice(DeviceEntry entry)
		{
            this.entry = entry;
			this.Initialize(entry);
		}

        ~WebFileDevice()
        {
        }

		// Initialize the device
		private void Initialize(DeviceEntry entry)
		{
			this.Name = entry[DeviceEntry.Name].ToString();
			this.Path = entry[DeviceEntry.Path].ToString();
			this.Version = entry[DeviceEntry.Version].ToString();

			this.port = (StringValue)entry[DeviceEntry.IPPort];
			this.addr = (StringValue)entry[DeviceEntry.IPAddress];
		}

		public bool IsVirtual
		{
			get { return this.isVirtual; }
		}

		private bool IsOpen
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// 
		/// Ignore the address parameter
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private bool Connect(string address)
		{
			bool connected = true;
			Thread t = new Thread(new ThreadStart(() => 
			{
				
			}));
			t.Start();
			// t.Join(1000);
			
			return connected;
		}


		public override void Start(string address)
		{
			this.Connect(address);
		}

		public override void Stop()
		{
			//throw new NotImplementedException();
		}

		public override void Send(byte[] action)
		{

		}
	}
}
