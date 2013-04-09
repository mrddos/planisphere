
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Controls.Data;
using System.IO;

namespace Scada.MainVision
{
	internal class VirtualDataProvider : DataProvider
	{
		private DBDataCommonListerner dataListener;

        private List<string> deviceKeyList = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners = new Dictionary<string, DBDataCommonListerner>();

        private string dataProviderFile = "HIPC.log";

        private List<Dictionary<string, object>> dataPool = new List<Dictionary<string, object>>();

        private List<string> lists = new List<string>();

		public VirtualDataProvider()
		{
            deviceKeyList.Add("scada.hipc");

            using (FileStream fileStream = File.OpenRead(dataProviderFile))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string line = sr.ReadLine();
                    int index = 0;
                    while (line != null)
                    {
                        line = line.Trim();
                        if (line.Length > 0)
                        {
                            lists.Add(line);
                            index++;
                        }

                        if (index > 90)
                        {
                            break;
                        }
                        // Next line;
                        line = sr.ReadLine();
                    }
                }
            }

            for (int i = 0; i < 100; i++)
            {
                dataPool.Add(new Dictionary<string, object>(10));
            }
            // this.dataListeners.Add("scada.hipc", new DBDataCommonListerner("scada.hipc"));
		}

		public override void Refresh()
		{
            foreach (var item in this.deviceKeyList)
            {
                string deviceKey = item.ToLower();
                if (!this.dataListeners.ContainsKey(deviceKey))
                {
                    continue;
                }

                DBDataCommonListerner listener = this.dataListeners[deviceKey];
                if (listener != null)
                {

                    // int count = MaxCountFetchRecent;
                    Config cfg = Config.Instance();
                    ConfigEntry entry = cfg[deviceKey];
                    
                    listener.OnDataArrivalBegin();
                    
                    int index = 0;
                    foreach (string line in lists)
                    {
                        if (line.Length > 0)
                        {
                            Dictionary<string, object> data = this.dataPool[index];
                            data.Clear();
                            ParseLine(line, entry, data);
                            listener.OnDataArrival(data);
                            index++;
                        }

                    }

                    listener.OnDataArrivalEnd();

                }

            }

		}

        private void ParseLine(string line, ConfigEntry entry, Dictionary<string, object> data)
        {
            string[] a = line.Split(' ');
            foreach (var i in entry.ConfigItems)
            {
                if (i.FieldIndex > 1)
                {
                    string v = a[i.FieldIndex + 1];
                    data.Add(i.Key, v);
                }
                else if (i.FieldIndex == 1)
                {
                    string v = a[1] + " " + a[2];
                    data.Add(i.Key, v);
                }

            }

        }

        public override DataListener GetDataListener(string deviceKey)
		{
            deviceKey = deviceKey.ToLower();
            this.deviceKeyList.Add(deviceKey);
            if (this.dataListeners.ContainsKey(deviceKey))
            {
                return this.dataListeners[deviceKey];
            }
            else
            {
                DBDataCommonListerner listener = new DBDataCommonListerner(deviceKey);
                this.dataListeners.Add(deviceKey, listener);
                return listener;
            }
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
