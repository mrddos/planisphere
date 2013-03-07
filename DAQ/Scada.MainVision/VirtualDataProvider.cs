
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


                Random rd = new Random();
                for (int i = 0; i < 5; ++i)
                {
                    Dictionary<string, object> d = new Dictionary<string, object>(10);
                    d["temp"] = (183.5 * rd.Next(1, 10)/10).ToString();
                    d["press"] = (120.0 * rd.Next(1, 10)/10).ToString();
                    d["wspeed"] = (98.5 * rd.Next(1, 10)/10).ToString();
                    d["temp2"] = (70.5 * rd.Next(1, 10)/10).ToString();
    
                    
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
