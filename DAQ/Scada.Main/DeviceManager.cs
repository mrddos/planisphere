using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scada.Main
{
	class DeviceManager
	{
		public DeviceManager()
		{

		}

		private void OnThreadStart(object args)
		{


		}

		public void Initialize()
		{
			// ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnThreadStart), null);	
		}
	}
}
