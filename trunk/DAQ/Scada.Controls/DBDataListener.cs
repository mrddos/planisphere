using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Controls.Data
{
	public delegate void OnDataArrival(params object[] data);

	public abstract class DataListener
	{
		public DataListener()
		{

		}

		public OnDataArrival OnDataArrival
		{
			get;
			set;
		}
	}
}
