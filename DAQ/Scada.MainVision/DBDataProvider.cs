
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Controls;

namespace Scada.MainVision
{

	internal class DBDataCommonListerner : DBDataListener
	{

	}

	/// <summary>
	/// 
	/// </summary>
	internal class DBDataProvider
	{
		private List<string> tableNames = new List<string>(30);

		/// <summary>
		/// 
		/// </summary>
		public DBDataProvider()
		{
		}

		internal DBDataListener GetListener(string table)
		{
			this.tableNames.Add(table);
			return new DBDataCommonListerner();
		}

		internal void Refresh()
		{
			foreach (string tableName in this.tableNames)
			{
				// TODO: 
			}
		}
	}
}
