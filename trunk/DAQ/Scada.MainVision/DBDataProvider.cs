
namespace Scada.MainVision
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Scada.Controls;
	using Scada.Controls.Data;
	using MySql.Data.MySqlClient;


	/// <summary>
	/// Each Device has a Listener.
	/// </summary>
	internal class DBDataProvider : DataProvider
	{
		private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

        private const int MaxCountFetchRecent = 10;

        private const string Id = "Id";

		private MySqlConnection conn = new MySqlConnection(ConnectionString);

		private MySqlCommand cmd = null;

		private List<string> deviceKeyList = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners;

        private Dictionary<string, object> filters = new Dictionary<string, object>();

        // private List<Dictionary<string, object>> dataPool = new List<Dictionary<string, object>>();

        private Dictionary<string, object> dataCache = new Dictionary<string, object>();

		/// <summary>
		/// 
		/// </summary>
		static DBDataProvider()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		public DBDataProvider()
		{

            this.dataListeners = new Dictionary<string, DBDataCommonListerner>();
            if (this.conn != null)
            {
                try
                {
                    this.conn.Open();
                    this.cmd = this.conn.CreateCommand();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
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

        public override void RemoveDataListener(string deviceKey)
        {
            deviceKey = deviceKey.ToLower();
            if (this.deviceKeyList.Contains(deviceKey))
            {
                this.deviceKeyList.Remove(deviceKey);
            }
        }

        public override void RemoveFilters()
        {
            this.filters.Clear();
        }

        public override void SetFilter(string key, object value)
        {
            if (!this.filters.ContainsKey(key))
            {
                this.filters.Add(key, value);
            }
        }

        /// <summary>
        /// If no listener, provider would not query the database.
        /// 
        /// </summary>
		public override void Refresh()
		{
			foreach (var item in this.deviceKeyList)
			{
                string deviceKey = item.ToLower();
                if (!this.dataListeners.ContainsKey(deviceKey))
                {
                    continue;
                }

                this.Refresh(deviceKey);
			}
		}

        public override void Refresh(string deviceKey)
        {
            DBDataCommonListerner listener = this.dataListeners[deviceKey];
            if (listener != null)
            {
                int count = 4;// MaxCountFetchRecent;
                Config cfg = Config.Instance();
                ConfigEntry entry = cfg[deviceKey];
                this.cmd.CommandText = this.GetSelectStatement(entry.TableName, count);
                using (MySqlDataReader reader = this.cmd.ExecuteReader())
                {
                    listener.OnDataArrivalBegin();

                    int index = 0;
                    while (reader.Read())
                    {
                        string id = reader.GetString(Id);
                        if (string.IsNullOrEmpty(id))
                        {
                            continue;
                        }
                        id = id.Trim();
                        if (this.dataCache.ContainsKey(id))
                        {
                            continue;
                        }
                        Dictionary<string, object> data = new Dictionary<string, object>(10);
                        data.Add("Id", id);
                        foreach (var i in entry.ConfigItems)
                        {
                            string v = reader.GetString(i.Key);
                            data.Add(i.Key, v);
                        }
                        this.dataCache.Add(id, data);
                        listener.OnDataArrival(data);
                        index++;
                    }

                    listener.OnDataArrivalEnd();
                }

            }

        }

        private string GetSelectStatement(string tableName, int count)
        {
            string format = "select * from {0} order by Time DESC limit {1}";
            return string.Format(format, tableName, count);
        }

		private DataListener GetDataListenerByTableName(string tableName)
		{
            if (!this.dataListeners.ContainsKey(tableName))
            {
                return null;
            }
            return this.dataListeners[tableName];
		}
	}
}
