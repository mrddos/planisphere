using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Controls.Data
{
	public delegate void OnDataArrivalBegin();

	public delegate void OnDataArrival(Dictionary<string, object> data);

	public delegate void OnDataArrivalEnd();

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

		public OnDataArrivalBegin OnDataArrivalBegin
		{ 
			get;
			set;
		}

		public OnDataArrivalEnd OnDataArrivalEnd
		{
			get;
			set;
		}
	}
}
