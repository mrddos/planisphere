
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


                for (int i = 0; i < 15; ++i)
                {
                    Dictionary<string, object> d = new Dictionary<string, object>(10);
                    d["temp"] = "213.5";
                    d["press"] = "4.33";
                    d["wspeed"] = "0.33";
                    d["temp2"] = "45.5";
    
                    
                    this.dataListener.OnDataArrival(d);
                }


				this.dataListener.OnDataArrivalEnd();
			}
		}

		public override DataListener GetDataListener(string tableName)
		{
			if (tableName.ToLower() == "weather")
			{
                this.dataListener = new DBDataCommonListerner(tableName);
				return this.dataListener;
			}
			return null;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override void RemoveDataListener(string tableName)
        {
            // Do nothing.
        }
	}
}
