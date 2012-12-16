using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Declare;
using System.Threading;

namespace Scada.Main
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceLoader
    {
		private string deviceName;

		private Thread workThread;

		public DeviceLoader(string deviceName)
        {
			this.deviceName = deviceName;
        }

        public Device Load()
        {
			Device device = null;
			StartDeviceOnWorkThread(device);
			return device;
        }

		public void Unload()
		{
			// 1.

			// 2.
		}

		private void StartDeviceOnWorkThread(Device device)
		{
			this.workThread = new Thread(new ParameterizedThreadStart(this.OnThreadStart));
			this.workThread.Start(device);
		}

		private void OnThreadStart(object arg)
		{
			Device device = arg as Device;
		}
    }
}
