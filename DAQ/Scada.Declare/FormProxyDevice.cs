using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Scada.Declare
{
	public class FormProxyDevice : Device
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport("user32.dll")]
		public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		public FormProxyDevice(DeviceEntry entry)
		{

			this.Initialize(entry);
		}

		~FormProxyDevice()
        {
        }

		private bool Initialize(DeviceEntry entry)
		{
			return true;
		}

		public override void Start(string address)
		{
			throw new NotImplementedException();
		}

		public override void Stop()
		{
			throw new NotImplementedException();
		}

		public override void Send(byte[] action)
		{
			throw new NotImplementedException();
		}
	}
}
