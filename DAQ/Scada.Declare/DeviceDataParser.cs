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
			//this.lineParser = new LineParser();
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
			return data;
		}
	}


	public class HIPCDataParser : DataParser
	{
		public HIPCDataParser()
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

}
