using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Diagnostics;
using MySql.Data.Types;

namespace Scada.Declare
{
    class DBConnectionForSid
    {

        private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

        private MySqlConnection conn = new MySqlConnection(ConnectionString);

        private MySqlCommand cmd = null;

        private const string Localhost = "127.0.0.1";

        private const string Root = "root";

        private const string DAQDatabase = "scada";

        public int GetCurrentSid(string tableName)
        {
            try
            {
                conn.Open();
                this.cmd = conn.CreateCommand();

                string query = string.Format("select max(Sid) from {0} ", tableName);
                this.cmd.CommandText = query;

                using (var reader = this.cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int sid = reader.GetInt32(0);
                        return sid;
                    }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return 1;
        }

    }
}
