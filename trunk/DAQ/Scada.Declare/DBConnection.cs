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
	class DBConnection
	{
        /// <summary>
        /// No Need Port 3306.
        /// 
        /// </summary>
        private const string ConnectionString = "datasource=127.0.0.1;username=root;database=scada";

        private MySqlConnection conn = new MySqlConnection(ConnectionString);

		private MySqlCommand cmd = null;

        private const string Localhost = "127.0.0.1";

        private const string Root = "root";

		private const string DAQDatabase = "scada";

        private string database = null;

        public string Database
        {
            get { return this.database; }
            set { this.database = value; }
        }

        public void Connect()
        {
			try
			{
				conn.Open();
				this.cmd = conn.CreateCommand();

				// AddHIPCRecordData("243.5", "2.3", "5.88", "24.0000", 2);
				// AddHIPCRecordData("243.5", "2.3", "5.88", "24.0000", 2);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
        }


		public bool AddRecordData(string commandText, DateTime time, params object[] items)
		{
            try
            {
                if (this.cmd != null)
                {
                    cmd.CommandText = commandText;

                    for (int i = 0; i < items.Length; ++i)
                    {
                        string at = string.Format("@{0}", i + 1);
                        cmd.Parameters.AddWithValue(at, items[i]);
                    }
                    cmd.ExecuteNonQuery();
                    // If exception, the params would NOT clear.
                    // cmd.Parameters.Clear();

                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                }
            }
			return true;
		}


	}
}
