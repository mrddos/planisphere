using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scada.DAQ.Installer
{
    class DataBaseInsertion
    {

        private MySqlConnection conn = null;

        private string connectionString = "datasource=127.0.0.1;username=root;database=scada";

        private int v = 0;



        public DataBaseInsertion()
        {
            
        }

        private int Interval
        {
            get { return 30; }
        }


        internal void Execute()
        {
            using (var connToMySql = new MySqlConnection(connectionString))
            {
                connToMySql.Open();

                using (MySqlCommand cmd = connToMySql.CreateCommand())
                {
                    DateTime t = GetBaseTime(DateTime.Now);
                    while (true)
                    {
                        t = t.AddSeconds(this.Interval);
                        ExecuteSQL(cmd, t);
                        Thread.Sleep(8 * 1000);
                    }
                }
            }
        }

        private DateTime GetBaseTime(DateTime startTime)
        {
            // 目前只支持30秒 和 5分钟两种间隔
            // Debug.Assert(this.Interval == 30 || this.Interval == 60 * 5 || this.Interval == 0);

            DateTime baseTime = default(DateTime);
            if (this.Interval == 30)
            {
                int second = startTime.Second / 30 * 30;
                baseTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, second);
            }
            else if (this.Interval == 60 * 5)
            {
                int min = startTime.Minute / 5 * 5;
                baseTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, min, 0);
            }
            return baseTime;
        }

        internal void Execute(string content)
        {
            
        }

        internal void ExecuteSQL(MySqlCommand cmd, DateTime t)
        {

            cmd.CommandText = "insert into HIPC_rec(time, doserate, highvoltage, battery, temperature, alarm) values(@1, @2, @3, 123, 24, 1)";
            cmd.Parameters.AddWithValue("@1", t);

            v = (v + 1) % 5;
            double d = double.Parse("1." + v);
            cmd.Parameters.AddWithValue("@2", d);

            double h = double.Parse("410." + v);
            cmd.Parameters.AddWithValue("@3", h);
            

            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        
    }
}
