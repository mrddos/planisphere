
namespace Scada.MainVision
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Scada.Controls;
    using Scada.Controls.Data;
    using MySql.Data.MySqlClient;
    using System.Data.SqlTypes;


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


        private List<string> allDeviceKeys = new List<string>();

        private List<string> deviceKeyList = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners;

        private Dictionary<string, object> filters = new Dictionary<string, object>();

        // private List<Dictionary<string, object>> dataPool = new List<Dictionary<string, object>>();

        private Dictionary<string, object> dataCache = new Dictionary<string, object>();

        private Dictionary<string, object> latestData = new Dictionary<string, object>();

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
            this.allDeviceKeys.Add(DeviceKey_Hipc);
            this.allDeviceKeys.Add(DeviceKey_Dwd);
            this.allDeviceKeys.Add(DeviceKey_HvSampler);
            this.allDeviceKeys.Add(DeviceKey_ISampler);
            this.allDeviceKeys.Add(DeviceKey_NaI);
            this.allDeviceKeys.Add(DeviceKey_Shelter);
            this.allDeviceKeys.Add(DeviceKey_Weather);

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

        // For Panels.
        // Use this to get the latest data
        public override void RefreshCurrentData()
        {
            this.latestData.Clear();
            foreach (var item in this.allDeviceKeys)
            {
                string deviceKey = item.ToLower();
                // Would use listener to notify, panel would get the lastest data.
                var dataList = this.RefreshCurrentData(deviceKey, true, 1);
                if (dataList != null && dataList.Count > 0)
                {
                    this.latestData.Add(deviceKey, dataList[0]);
                }
            }
        }

        public override void Refresh(string deviceKey)
        {
            DBDataCommonListerner listener = this.dataListeners[deviceKey];

            var result = this.Refresh(deviceKey, true, 10, DateTime.MinValue, DateTime.MinValue);
            if (result == null)
            {
                return;
            }
            listener.OnDataArrivalBegin();
            foreach (var data in result)
            {
                listener.OnDataArrival(data);
            }
            listener.OnDataArrivalEnd();
        }

        public override void RefreshTimeRange(string deviceKey, string from, string to)
        {
            try
            {
                DateTime fromTime = DateTime.Parse(from);
                DateTime toTime = DateTime.Parse(to);

                DBDataCommonListerner listener = this.dataListeners[deviceKey];

                var result = this.Refresh(deviceKey, false, -1, fromTime, toTime);
                listener.OnDataArrivalBegin();
                foreach (var data in result)
                {
                    listener.OnDataArrival(data);
                }
                listener.OnDataArrivalEnd();
            }
            catch (Exception)
            {
            }

        }

        private List<Dictionary<string, object>> RefreshCurrentData(string deviceKey, bool current, int count)
        {
            if (this.cmd == null)
            {
                return null;
            }
            // Return values
            var ret = new List<Dictionary<string, object>>();

            Config cfg = Config.Instance();
            ConfigEntry entry = cfg[deviceKey];
            this.cmd.CommandText = this.GetSelectStatement(entry.TableName, count);
            using (MySqlDataReader reader = this.cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Must Has an Id.
                    string id = reader.GetString(Id);
                    id = id.Trim();

                    if (string.IsNullOrEmpty(id)) { continue; }

                    Dictionary<string, object> data = new Dictionary<string, object>(10);
                    data.Add("Id", id);

                    foreach (var i in entry.ConfigItems)
                    {
                        string key = i.Key.ToLower();
                        try
                        {
                            string v = reader.GetString(key);
                            data.Add(key, v);
                        }
                        catch (SqlNullValueException)
                        {
                            // TODO: Has Null Value
                            data.Add(key, null);
                        }
                        catch (Exception)
                        {
                            // No this field.
                        }
                    }

                    if (entry.DataFilter != null)
                    {
                        entry.DataFilter.Fill(data);
                    }
                    ret.Add(data);
                }
            }

            return ret;
        }


        /// <summary>
        /// Implements 
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <param name="current"></param>
        /// <param name="count"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private List<Dictionary<string, object>> Refresh(string deviceKey, bool current, int count, DateTime from, DateTime to)
        {
            if (this.cmd == null)
            {
                return null;
            }
            // Return values
            var ret = new List<Dictionary<string, object>>();

            Config cfg = Config.Instance();
            ConfigEntry entry = cfg[deviceKey];
            this.cmd.CommandText = this.GetSelectStatement(entry.TableName, count);
            using (MySqlDataReader reader = this.cmd.ExecuteReader())
            {
                int index = 0;
                while (reader.Read())
                {
                    // Must Has an Id.
                    string id = reader.GetString(Id);
                    id = id.Trim();

                    if (string.IsNullOrEmpty(id) || this.dataCache.ContainsKey(id)) { continue; }
                    
                    Dictionary<string, object> data = new Dictionary<string, object>(10);
                    data.Add("Id", id);

                    foreach (var i in entry.ConfigItems)
                    {
                        string key = i.Key.ToLower();
                        try
                        {
                            string v = reader.GetString(key);
                            data.Add(key, v);
                        }
                        catch (SqlNullValueException)
                        {
                            // TODO: Has Null Value
                            data.Add(key, null);
                        }
                        catch (Exception)
                        {
                            // No this field.
                        }
                    }

                    if (entry.DataFilter != null)
                    {
                        entry.DataFilter.Fill(data);
                    }
                    ret.Add(data);
                    // this.dataCache.Add(id, data);

                    index++;
                }
            }

            return ret;
        }

        private string GetSelectStatement(string tableName, int count)
        {
            // Get the recent <count> entries.
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

        public override Dictionary<string, object> GetLatestData(string deviceKey)
        {
            if (this.latestData.ContainsKey(deviceKey))
            {
                return (Dictionary<string, object>)this.latestData[deviceKey];
            }
            return null;
        }
    }
}
