using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Declare;
using System.Reflection;

namespace Scada
{
    public class VirtualAgent : Device
    {
		
		public VirtualAgent()
		{
			this.Name = "virtual-agent";
		}

        /// <summary>
        /// 
        /// </summary>
        public override void Start(string address)
        {
            // Start this device and return data...
            return;
        }

		public override void Stop()
		{
			// Start this device and return data...
			return;
		}
    }
}
