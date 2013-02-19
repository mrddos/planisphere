
namespace Scada.MainVision
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Scada.Controls;
	using Scada.Controls.Data;
	using MySql.Data.MySqlClient;

	internal class DBDataCommonListerner : DataListener
	{

	}

	/// <summary>
	/// Each Device has a Listener.
	/// </summary>
	internal class DBDataProvider : DataProvider
	{
		private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

		private MySqlConnection conn = new MySqlConnection(ConnectionString);

		private MySqlCommand cmd = null;

		private List<string> tableNames = new List<string>();

        private Dictionary<string, DBDataCommonListerner> dataListeners = new Dictionary<string, DBDataCommonListerner>();

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
			this.cmd = this.conn.CreateCommand();
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
                DBDataCommonListerner listener = new DBDataCommonListerner();
                this.dataListeners.Add(tableName, listener);
                return listener;
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
                    // TODO:
                    this.cmd.CommandText = "";
                    MySqlDataReader reader = this.cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // reader.
                    }
                }
                
			}
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
