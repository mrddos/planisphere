
namespace Scada.DataCenterAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;
    using System.Data.SqlTypes;
    using System.IO;


    /// <summary>
    /// Each Device has a Listener.
    /// </summary>
    internal class DBDataSource
    {
        private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

        private const int MaxCountFetchRecent = 10;

        private const string Id = "Id";

        private const string Time = "time";

        private MySqlConnection conn = null;

        private MySqlCommand cmd = null;


        private List<string> tables = new List<string>();

        private List<string> deviceKeyList = new List<string>();


      

        // <DeviceKey, dict[data]>
        private Dictionary<string, object> latestData = new Dictionary<string, object>();


        /// <summary>
        /// 
        /// </summary>
        public DBDataSource()
        {
            this.conn = new MySqlConnection(ConnectionString);

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


        DataPacket GetDataPacket(string deviceKey, DateTime time)
        {
            return default(DataPacket);
        }
       
        /*
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
        */

        // Get time-range data,
        // Notify with all the result.
        public List<Dictionary<string, object>> RefreshTimeRange(string deviceKey, DateTime fromTime, DateTime toTime)
        {
            try
            {
                // DBDataCommonListerner listener = this.dataListeners[deviceKey];

                var result = this.Refresh(deviceKey, false, -1, fromTime, toTime);
                result.Reverse();
                return result;
            }
            catch (Exception e)
            {

            }

            return new List<Dictionary<string, object>>();
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

            Settings cfg = Settings.Instance;
            SettingEntries entry = cfg[deviceKey];
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

                    foreach (var i in entry.Items)
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


                }
            }

            return ret;
        }

        private List<Dictionary<string, object>> Refresh(string deviceKey, bool current, int count, DateTime fromTime, DateTime toTime)
        {
            if (this.cmd == null)
            {
                return null;
            }
            // Return values
            var ret = new List<Dictionary<string, object>>();

            Settings settings = Settings.Instance;
            SettingEntries entry = settings[deviceKey];
            if (current)
            {
                this.cmd.CommandText = this.GetSelectStatement(entry.TableName, count);
            }
            else
            {
                this.cmd.CommandText = this.GetSelectStatement(entry.TableName, fromTime, toTime);
            }
            using (MySqlDataReader reader = this.cmd.ExecuteReader())
            {
                int index = 0;
                while (reader.Read())
                {
                    // Must Has an Id.
                    string id = reader.GetString(Id);
                    id = id.Trim();

                    // if (string.IsNullOrEmpty(id) || this.dataCache.ContainsKey(id)) { continue; }
                    
                    Dictionary<string, object> data = new Dictionary<string, object>(10);
                    data.Add("Id", id);

                    foreach (var i in entry.Items)
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

        private string GetSelectStatement(string tableName, DateTime fromTime, DateTime toTime)
        {
            // Get the recent <count> entries.
            string format = "select * from {0}  where time<'{1}' and time>'{2}' order by Id DESC";
            string sql = string.Format(format, tableName, toTime, fromTime);
            return sql;
        }

        public Dictionary<string, object> GetLatestData(string deviceKey)
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
