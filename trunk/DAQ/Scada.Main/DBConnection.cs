using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Scada.Main
{
	class DBConnection
	{
        /// <summary>
        /// No Need Port 3306.
        /// 
        /// </summary>
        private const string ConnectionString = "datasource=127.0.0.1;username=root;database=daq2";

        private MySqlConnection conn = new MySqlConnection(ConnectionString);

        private const string Localhost = "127.0.0.1";

        private const string Root = "root";

        private const string DAQDatabase = "daq";

        private string database = null;

        public string Database
        {
            get { return this.database; }
            set { this.database = value; }
        }



        public void Connect()
        {
            // conn.Open();

            
        }
	}
}
