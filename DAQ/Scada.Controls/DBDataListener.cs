using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Controls
{
	public delegate void OnDataArrival(params object[] data);

	public abstract class DBDataListener
	{

		public OnDataArrival OnDataArrival
		{
			get;
			set;
		}
	}
}
