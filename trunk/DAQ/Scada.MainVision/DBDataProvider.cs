
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

		private MySqlConnection conn = new MySqlConnection(ConnectionString);

		private MySqlCommand cmd = null;

		private List<string> deviceKeyList = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners;


        private List<Dictionary<string, object>> dataPool = new List<Dictionary<string, object>>();

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
            for (int i = 0; i < 50; i++)
            {
                dataPool.Add(new Dictionary<string, object>(10));
            }


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

                DBDataCommonListerner listener = this.dataListeners[deviceKey];
                if (listener != null)
                {

                    int count = MaxCountFetchRecent;
                    Config cfg = Config.Instance();
                    ConfigEntry entry = cfg[deviceKey];
                    this.cmd.CommandText = this.GetSelectStatement(entry.TableName, count);
                    using (MySqlDataReader reader = this.cmd.ExecuteReader())
                    {
                        listener.OnDataArrivalBegin();

                        int index = 0;
                        while (reader.Read())
                        {
                            Dictionary<string, object> data = this.dataPool[index];
                            data.Clear();
                            foreach (var i in entry.ConfigItems)
                            {
                                string v = reader.GetString(i.FieldIndex);
                                data.Add(i.Key, v);
                            }

                            listener.OnDataArrival(data);
                            index++;
                        }

                        listener.OnDataArrivalEnd();
                    }

                }
                
			}
		}

        private string GetSelectStatement(string tableName, int count)
        {
            string format = "select * from {0} order by Id DESC limit {1}";
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
