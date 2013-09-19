using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private string device;

        public DataBaseInsertion(string device)
        {
            this.device = device;
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
                        if (this.device != "nai")
                        {
                            t = t.AddSeconds(this.Interval);
                        }
                        else
                        {
                            t = t.AddSeconds(60 * 5);
                        }
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
            if (this.device.ToLower() == "hpic")
            {
                cmd.CommandText = "insert into HPIC_rec(time, doserate, highvoltage, battery, temperature, alarm) values(@1, @2, @3, 123, 24, 1)";
                cmd.Parameters.AddWithValue("@1", t);

                v = (v + 1) % 5;
                double d = double.Parse("1." + v);
                cmd.Parameters.AddWithValue("@2", d);

                double h = double.Parse("410." + v);
                cmd.Parameters.AddWithValue("@3", h);


                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            else if (this.device.ToLower() == "weather")
            {
                cmd.CommandText = "insert into weather(time, Windspeed, Direction, Temperature, Humidity, Pressure, Raingauge,Dewpoint,IfRain) " 
                                    + "values(@1, 0, 360, @2, @3, 1000.1, 1, 1, 0)";
                cmd.Parameters.AddWithValue("@1", t);

                v = (v + 1) % 5;
                double d = double.Parse("1." + v);
                cmd.Parameters.AddWithValue("@2", d);

                double h = double.Parse("20." + v);
                cmd.Parameters.AddWithValue("@3", h);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            else if (this.device.ToLower() == "shelter")
            {
                // Debug.Assert(false);
                cmd.CommandText = "insert into environment_rec(time, Temperature, Humidity, IfMainPowerOff, BatteryHours, IfSmoke, IfWater, IfDoorOpen, Alarm)" 
                                    + " values(@1, @2, @3, 0, 4, 0, 0, 0, 0)";
                cmd.Parameters.AddWithValue("@1", t);

                v = (v + 1) % 5;
                double d = double.Parse("2" + v);
                cmd.Parameters.AddWithValue("@2", d);

                double h = double.Parse("3" + v);
                cmd.Parameters.AddWithValue("@3", h);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            else if (this.device.ToLower() == "nai")
            {
                // TODO: 
                t = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, 0);
                cmd.CommandText = "insert into nai_rec(time, StartTime, EndTime, Coefficients, ChannelData, DoseRate, Temperature,HighVoltage,NuclideFound,EnergyFromPosition) values(@1, @2, @3, @4, '1 1 1 1 1 1 1', @5, 24, 400.1, 1, 1460.83)";
                cmd.Parameters.AddWithValue("@1", t);
                cmd.Parameters.AddWithValue("@2", t.AddMinutes(-5));
                cmd.Parameters.AddWithValue("@3", t);

                cmd.Parameters.AddWithValue("@4", "-4.94022E+00 1.37924E+00 9.68201E-05");
                cmd.Parameters.AddWithValue("@5", "0.04");

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            else if (this.device.ToLower() == "mds")
            {

            }
            else if (this.device.ToLower() == "ais")
            {

            }
            else if (this.device.ToLower() == "dwd")
            {

            }

        }

        
    }
}
