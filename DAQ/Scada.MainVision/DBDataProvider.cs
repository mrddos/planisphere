﻿
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

        private const string Time = "time";

        private MySqlConnection conn = new MySqlConnection(ConnectionString);

        private MySqlCommand cmd = null;

        // private bool isRealTime = true;


        private List<string> allDeviceKeys = new List<string>();

        private List<string> deviceKeyList = new List<string>();


        private Dictionary<string, DBDataCommonListerner> dataListeners;

        // ?? What's filter.
        private Dictionary<string, object> filters = new Dictionary<string, object>(10);


        // ?
        private Dictionary<string, object> dataCache = new Dictionary<string, object>();


        // <DeviceKey, dict[data]>
        private Dictionary<string, object> latestData = new Dictionary<string, object>();


        private List<Dictionary<string, object>> timelineSource;
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

            this.FetchCount = 20;

            this.dataListeners = new Dictionary<string, DBDataCommonListerner>(30);
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


            this.timelineSource = new List<Dictionary<string, object>>();
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
        // Get Latest data,
        // No Notify.
        public override void RefreshTimeNow()
        {
            this.latestData.Clear();
            foreach (var item in this.allDeviceKeys)
            {
                string deviceKey = item.ToLower();
                // Would use listener to notify, panel would get the lastest data.
                var data = this.RefreshTimeNow(deviceKey);
                if (data != null)
                {
                    this.latestData.Add(deviceKey, data);
                }
            }
        }

        public int FetchCount
        {
            get;
            set;
        }

        // Get Recent data
        // Notify the new 
        public override void RefreshTimeline(string deviceKey)
        {
            DBDataCommonListerner listener = this.dataListeners[deviceKey];
            if (listener == null)
            {
                return;
            }

            var result = this.Refresh(deviceKey, true, this.FetchCount, DateTime.MinValue, DateTime.MinValue);
            if (result == null)
            {
                return;
            }
            List<Dictionary<string, object>> recent = new List<Dictionary<string, object>>();
            DateTime latestDateTime = DateTime.MinValue;
            if (this.timelineSource.Count > 0)
            {
                var last = this.timelineSource.First();
                latestDateTime = DateTime.Parse((string)last["time"]);
            }
            foreach (var item in result)
            {
                DateTime dt = DateTime.Parse((string)item["time"]);
                if (dt > latestDateTime)
                {
                    recent.Add(item);
                }
            }

            if (recent.Count == 0)
            {
                return;
            }
            recent.Sort(DBDataProvider.DateTimeCompare);

            this.timelineSource.AddRange(recent);
            this.timelineSource.Sort(DBDataProvider.DateTimeCompare);


            listener.OnDataArrivalBegin(DataArrivalConfig.TimeRecent);
            foreach (var data in recent)
            {
                listener.OnDataArrival(data);
            }
            listener.OnDataArrivalEnd();
        }

        // Get time-range data,
        // Notify with all the result.
        public override void RefreshTimeRange(string deviceKey, string from, string to)
        {
            try
            {
                DateTime fromTime = DateTime.Parse(from);
                DateTime toTime = DateTime.Parse(to);

                DBDataCommonListerner listener = this.dataListeners[deviceKey];

                var result = this.Refresh(deviceKey, false, -1, fromTime, toTime);
                listener.OnDataArrivalBegin(DataArrivalConfig.TimeRange);
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

        private Dictionary<string, object> RefreshTimeNow(string deviceKey)
        {
            if (this.cmd == null)
            {
                return null;
            }
            // Return values
            const int MaxItemCount = 20;
            var ret = new Dictionary<string, object>(MaxItemCount);

            Config cfg = Config.Instance();
            ConfigEntry entry = cfg[deviceKey];
            this.cmd.CommandText = this.GetSelectStatement(entry.TableName, 1);
            using (MySqlDataReader reader = this.cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Must Has an Id.
                    string id = reader.GetString(Id);
                    id = id.Trim();

                    if (string.IsNullOrEmpty(id))
                    {
                        return null;
                    }

                    ret.Add(Id, id);

                    foreach (var i in entry.ConfigItems)
                    {
                        string key = i.Key.ToLower();
                        try
                        {
                            string v = reader.GetString(key);
                            ret.Add(key, v);
                        }
                        catch (SqlNullValueException)
                        {
                            // TODO: Has Null Value
                            ret.Add(key, null);
                        }
                        catch (Exception)
                        {
                            // No this field.
                        }
                    }

                    if (entry.DataFilter != null)
                    {
                        entry.DataFilter.Fill(ret);
                    }
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

        public static int DateTimeCompare(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            object t1 = a[Time];
            object t2 = b[Time];
            DateTime dt1 = DateTime.MinValue;
            DateTime dt2 = DateTime.MinValue;
            if (t1 != null)
            {
                dt1 = DateTime.Parse((string)t1);
            }
            if (t2 != null)
            {
                dt2 = DateTime.Parse((string)t2);
            }

            if (dt1 > dt2)
            {
                return -1;
            }
            return 1;
        }
    }
}
