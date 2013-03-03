
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

		private List<string> tableNames = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners;

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

		public override DataListener GetDataListener(string tableName)
		{
			this.tableNames.Add(tableName);
            if (this.dataListeners.ContainsKey(tableName))
            {
                return this.dataListeners[tableName];
            }
            else
            {
                DBDataCommonListerner listener = new DBDataCommonListerner(tableName);
                this.dataListeners.Add(tableName, listener);
                return listener;
            }

		}

        public override void RemoveDataListener(string tableName)
        {
            if (this.tableNames.Contains(tableName))
            {
                this.tableNames.Remove(tableName);
            }
        }

        /// <summary>
        /// If no listener, provider would not query the database.
        /// 
        /// </summary>
		public override void Refresh()
		{
			foreach (string tableName in this.tableNames)
			{
                if (!this.dataListeners.ContainsKey(tableName))
                {
                    continue;
                }

                DBDataCommonListerner listener = this.dataListeners[tableName];
                if (listener != null)
                {

                    int count = MaxCountFetchRecent;
                    this.cmd.CommandText = this.GetSelectStatement(tableName, count);
                    MySqlDataReader reader = this.cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // TODO: Mashal the data into Dict;
                        string a = reader.GetString(2);
                        
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
