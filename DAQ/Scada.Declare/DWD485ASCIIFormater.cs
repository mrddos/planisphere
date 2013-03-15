using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class DWD485ASCIIFormater : DataParser
	{
		public DWD485ASCIIFormater()
		{ 
		}

		public override string[] Search(string data)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(data);
			if (bytes[0] == 0)
			{

			}
			int len = bytes[4];
			int open = bytes[5];

			//?
			int a = bytes[6];
			int b = bytes[7];
			int c = bytes[8];

			bool rain = bytes[9] == 0x30;
			bool full = bytes[10] == 0x33;
			string time = Encoding.ASCII.GetString(bytes, 11, 5);	// in minutes;

			return new string[] { };
		}
	}

	public class DWD485ASCIILineParser : LineParser
	{
		private string remain = string.Empty;

		public override string ContinueWith(string data)
		{
			this.remain += data;
			byte[] bytes = Encoding.ASCII.GetBytes(remain);

			int start = 0;
			int end = 0;
			for (int i = 0; i < bytes.Length; ++i)
			{
				if (bytes[i] == '\0')
				{
					start = i;
					continue;
				}

				if (bytes[i] == 0x01)
				{
					end = i;
					break;
				}
			}

			remain = Encoding.ASCII.GetString(bytes, end + 1, bytes.Length - end);

			string line = Encoding.ASCII.GetString(bytes, start, end);
			line.IndexOf('\0');
			return line;
		}

	}
}
