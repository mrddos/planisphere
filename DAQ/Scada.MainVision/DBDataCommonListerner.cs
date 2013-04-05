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

    public class DBDataCommonListerner : DataListener
    {
        static Dictionary<string, List<ColumnInfo>> columnTable = new Dictionary<string, List<ColumnInfo>>();

        private string deviceKey;

		public DBDataCommonListerner(string deviceKey)
        {
			this.deviceKey = deviceKey;
        }

        public override List<ColumnInfo> GetColumnsInfo()
        {
			string deviceKey = this.deviceKey.ToLower();
			if (columnTable.ContainsKey(deviceKey))
            {
				return columnTable[deviceKey];
            }

			Config cfg = Config.Instance();
			ConfigEntry entry = cfg[deviceKey];
			List<ColumnInfo> colInfoList = this.ToColumnInfoList(entry);

            // Cache
			columnTable.Add(deviceKey, colInfoList);
			return colInfoList;
        }

		private List<ColumnInfo> ToColumnInfoList(ConfigEntry entry)
		{
			List<ColumnInfo> ret = new List<ColumnInfo>();
			foreach (ConfigItem ci in entry.ConfigItems)
			{
				ret.Add(new ColumnInfo() { Header = ci.ColumnName, BindingName = ci.Key, Width = 100 });
			}
			return ret;
		}

    }
}
