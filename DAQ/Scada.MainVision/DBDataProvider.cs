
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
	/// 
	/// </summary>
	internal class DBDataProvider : DataProvider
	{
		private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

		private MySqlConnection conn = new MySqlConnection(ConnectionString);

		private MySqlCommand cmd = null;

		private List<string> tableNames = new List<string>();

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
			return new DBDataCommonListerner();
		}

		public override void Refresh()
		{
			foreach (string tableName in this.tableNames)
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

		private DataListener GetDataListenerByTableName(string tableName)
		{
			return null;
		}
	}
}
