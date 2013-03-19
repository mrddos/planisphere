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


		public bool AddRecordData(string commandText, params object[] items)
		{
			try
			{
				if (this.cmd != null)
				{
					// "insert into HIPCrec(time, doserate, highvoltage ...) values(@1, @2, @3 ... )"
					cmd.CommandText = commandText;

					for (int i = 0; i < items.Length; ++i)
					{
						string at = string.Format("@{0}", i + 1);
						cmd.Parameters.AddWithValue(at, items[i]);
					}
					cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
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
			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="doseRate"></param>
		/// <param name="highVoltage"></param>
		/// <param name="battery"></param>
		/// <param name="temperature"></param>
		/// <param name="alarm"></param>
		/// <returns></returns>
		public bool AddHIPCRecordData(string doseRate, string highVoltage, string battery, string temperature, short alarm)
		{
			try
			{
				if (this.cmd != null)
				{
					cmd.CommandText = "insert into HIPCrec(time, doserate, highvoltage, battery, temperature, alarm) values(@1, @2, @3, @4, @5, @6)";

					cmd.Parameters.AddWithValue("@1", DateTime.Now);
					cmd.Parameters.AddWithValue("@2", doseRate);
					cmd.Parameters.AddWithValue("@3", highVoltage);
					cmd.Parameters.AddWithValue("@4", battery);
					cmd.Parameters.AddWithValue("@5", temperature);
					cmd.Parameters.AddWithValue("@6", alarm);

					cmd.ExecuteNonQuery();
				}
				else
				{
					return false;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}
			return true;
		}

	}
}
