using Scada.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class WeatherDataParser : DataParser
	{
		public WeatherDataParser()
		{
            this.lineParser = new LineParser();
		}

		public override string[] Search(byte[] data)
		{
			// >"11/29/12","00:58", 10.0, 55,  1.3,1018.4,360,  0.0,   0.0,2,!195
			string line = Encoding.ASCII.GetString(data);
            
			int p = line.IndexOf('>');
			line = line.Substring(p + 1);
			string[] items = line.Split(',');
			for (int i = 0; i < items.Length; ++i)
			{
				items[i] = items[i].Trim();
			}
			return items;
		}

		public override byte[] GetLineBytes(byte[] data)
		{
            int len = data.Length;
            if (data[len - 2] == (byte)0x0d && data[len - 1] == (byte)0x0a)
            {
                return data;
            }
            else if (this.lineParser != null)
            {
                return this.lineParser.ContinueWith(data);
            }
            
            return data;
		}
	}


	public class HPICDataParser : DataParser
	{
		public HPICDataParser()
		{
			this.lineParser = new LineParser();
		}

		public override string[] Search(byte[] data)
		{
			// .0000   .0000   .0000   .0000   .5564   383.0   6.136   28.40   .0000 
			string line = Encoding.ASCII.GetString(data);
			int p = line.IndexOf('>');
			line = line.Substring(p + 1);
			string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            double d = 0.0;
            if (double.TryParse(items[4], out d))
            {
                double f = 1.0;
                if (this.Factors.Count > 0)
                {
                    f = this.Factors[0];
                }

                // uGy/h (*1000)==> nGy/h
                items[4] = (f * d * 1000).ToString();
            }
			return items;
		}

		public override byte[] GetLineBytes(byte[] data)
		{
			if (this.lineParser != null)
			{
				return this.lineParser.ContinueWith(data);
			}
			return data;
		}
	}


    public class ShelterDataParser : DataParser
    {
        public ShelterDataParser()
		{
			// this.lineParser = new LineParser();
		}

		public override string[] Search(byte[] data)
		{
            // [10:28:09] 2013-5-19 10:28:09;; 1827 2470 2698 1953 4095 4095 4095 4095 
			string line = Encoding.ASCII.GetString(data);
			int p = line.IndexOf('#');
			line = line.Substring(p + 1);
			string[] items = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Temperature, Humidity, IfMainPowerOff, BatteryHours, IfSmoke, IfWater, IfDoorOpen, Alarm
            string[] ret = new string[8];
           
            string item = string.Empty;

            item = items[1];    // 温度, 第二个是温度
            if (!string.IsNullOrEmpty(item))
            {
                double v = 0.0;
                if (double.TryParse(item, out v))
                {
                    double f = 0.0122;
                    if (this.Factors.Count >= 2)
                    {
                        f = this.Factors[1];
                    }

                    int temp = (int)(v * f);
                    ret[0] = temp.ToString();
                }
            }

            item = items[0];    // 湿度, 第一个是湿度
            if (!string.IsNullOrEmpty(item))
            {
                double v = 0.0;
                if (double.TryParse(item, out v))
                {
                    double f = 0.0224;
                    if (this.Factors.Count >= 1)
                    {
                        f = this.Factors[0];
                    }
                    int humidity = (int)(v * f);
                    ret[1] = humidity.ToString();
                }
            }

            item = items[11];    // 备电状态
            if (!string.IsNullOrEmpty(item))
            {
                bool isMainPowerOff = item.Trim() == "1";
                ret[2] = isMainPowerOff ? "1" : "0";
            }

            item = items[2];    // 备电时间Hour, 第三个是电压
            if (!string.IsNullOrEmpty(item))
            {
                double v = 0.0;
                if (double.TryParse(item, out v))
                {
                    double f = 0.00488 * 600 * 0.8 / 80;
                    if (this.Factors.Count >= 3)
                    {
                        f = this.Factors[2];
                    }

                    double hour = f * v;// *0.00488 * 0.8 / 80;
                    ret[3] = ((int)hour).ToString();
                }
            }

            item = items[10];    // 烟
            if (!string.IsNullOrEmpty(item))
            {
                bool ifSmoke = (item.Trim() == "1");
                ret[4] = ifSmoke ? "1" : "0";
            }

            item = items[9];    // 水
            if (!string.IsNullOrEmpty(item))
            {
                bool ifWater = (item.Trim() == "1");
                ret[5] = ifWater ? "1" : "0";
            }

            item = items[8];    // 门
            if (!string.IsNullOrEmpty(item))
            {
                bool ifOpen = (item.Trim() == "0");
                ret[6] = ifOpen ? "1" : "0";
            }

            ret[7] = "";

            return ret;
		}

		public override byte[] GetLineBytes(byte[] data)
		{
            // Data in One Frame! And I don't known whether the line-parser is valid. 
			//if (this.lineParser != null)
			//{
			//return this.lineParser.ContinueWith(data);
			//}
			return data;
		}
    }


	public class WebFileDataParser : DataParser
	{
		public WebFileDataParser()
		{		
		}

		public override string[] Search(byte[] data)
		{


			return new string[0] { };
		}

		public override byte[] GetLineBytes(byte[] data)
		{
			return data;
		}
	}
}
