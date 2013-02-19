
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Controls.Data;

namespace Scada.MainVision
{
	internal class VirtualDataProvider : DataProvider
	{
		private DBDataCommonListerner dataListener;

		public VirtualDataProvider()
		{

		}

		public override void Refresh()
		{
			if (this.dataListener != null)
			{
				this.dataListener.OnDataArrivalBegin();
				Dictionary<string, object> d = new Dictionary<string, object>(10);
				
				
				this.dataListener.OnDataArrival(d);
                Dictionary<string, object> d2 = new Dictionary<string, object>(10);


                this.dataListener.OnDataArrival(d2);
				/*
				this.dataListener.OnDataArrival("1A", "1B", "1C", ".33");
				this.dataListener.OnDataArrival("2A", "2B", "2C", ".4454");
				this.dataListener.OnDataArrival("3A", "3B", "3C", ".1444");
				this.dataListener.OnDataArrival("4A", "4B", "4C", ".4544");
				this.dataListener.OnDataArrival("5A", "5B", "5C", ".8448");
				this.dataListener.OnDataArrival("6A", "6B", "6C", ".4144");
				 */
				this.dataListener.OnDataArrivalEnd();
			}
		}

		public override DataListener GetDataListener(string tableName)
		{
			if (tableName.ToLower() == "weather")
			{
				this.dataListener = new DBDataCommonListerner();
				return this.dataListener;
			}
			return null;
		}
	}
}
