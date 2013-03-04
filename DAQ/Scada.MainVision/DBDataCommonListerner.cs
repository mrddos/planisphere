
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

        private string tableName;

        public DBDataCommonListerner(string tableName)
        {
            this.tableName = tableName;
        }

        public override List<ColumnInfo> GetColumnsInfo()
        {
            string tableKey = tableName.ToLower();
            if (columnTable.ContainsKey(tableKey))
            {
                return columnTable[tableKey];
            }

            List<ColumnInfo> r = new List<ColumnInfo>();
            if (tableKey == "weather")
            {
                r.Add(new ColumnInfo() { Header = "温度", BindingName = "temp", Width = 100 });
                r.Add(new ColumnInfo() { Header = "气压", BindingName = "press", Width = 100 });
                r.Add(new ColumnInfo() { Header = "风速", BindingName = "wspeed", Width = 100 });
                r.Add(new ColumnInfo() { Header = "其他数据", BindingName = "temp2", Width = 100 });
            }
            else if (tableName.ToLower() == "hipc")
            {
            }

            // TODO: 
            columnTable.Add(tableKey, r);
            return r;
        }

    }
}
