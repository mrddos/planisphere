
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
        private string tableName;

        public DBDataCommonListerner(string tableName)
        {
            this.tableName = tableName;
        }

        public override List<ColumnInfo> GetColumnsInfo()
        {
            List<ColumnInfo> r = new List<ColumnInfo>();
            if (tableName.ToLower() == "weather")
            {
                r.Add(new ColumnInfo() { Header = "温度", BindingName = "temp", Width = 100 });
                r.Add(new ColumnInfo() { Header = "气压", BindingName = "press", Width = 100 });
                r.Add(new ColumnInfo() { Header = "风速", BindingName = "wspeed", Width = 100 });
                r.Add(new ColumnInfo() { Header = "****", BindingName = "---", Width = 100 });
            }
            else if (tableName.ToLower() == "hipc")
            {
            }
            return r;
        }

    }
}
