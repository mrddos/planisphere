using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
    /// <summary>
    /// 
    /// </summary>
    public enum ErrorCode
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Device
    {
		private string name;

		private string version;

        /// <summary>
        /// 
        /// </summary>
        public string Name
		{ 
			get { return this.name;}
			set { this.name = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        public string Version
		{ 
			get { return this.version;}
			set { this.version = value; }
		}

    }


}
