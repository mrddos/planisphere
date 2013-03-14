using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class TelnetDevice : Device
	{
		private bool isVirtual = false;

		private DeviceEntry entry = null;

		public TelnetDevice(DeviceEntry entry)
		{
            this.entry = entry;
			this.Initialize(entry);
		}

        ~TelnetDevice()
        {
        }

		// Initialize the device
		private void Initialize(DeviceEntry entry)
		{
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

		private bool Connect(string address)
		{
			// TODO: 
			return true;
		}


		public override void Start(string address)
		{
			this.Connect(address);
		}

		public override void Stop()
		{
			//throw new NotImplementedException();
		}

		public override void Send(string data)
		{
			//throw new NotImplementedException();
		}
	}
}
