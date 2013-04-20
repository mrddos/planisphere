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
			this.lineParser = new DWD485ASCIILineParser();
		}

		public override string[] Search(byte[] bytes)
		{
			// 00 
			// 32 CD A0 
			// 3B 
			// 32 
			// 30 30 31 30 33 30 30 30 30 30 02 51 01 1A 
			
			// Skip 0 123 4
			
			bool open = (bytes[5] == 0x31);
            string opens = string.Format("{0}", open ? 1 : 0);

			// Skip
			int a = bytes[6];
			int b = bytes[7];
			int c = bytes[8];

			bool rain = bytes[9] == 0x30;
            string rains = string.Format("{0}", rain ? 1 : 0);

			bool full = bytes[10] == 0x33;
            string fulls = string.Format("{0}", full ? 1 : 0);
			string time = Encoding.ASCII.GetString(bytes, 11, 5);	// in minutes;

			return new string[] { opens, rains, fulls, time };
		}

		public override byte[] GetLineBytes(byte[] data)
		{
			return this.lineParser.ContinueWith(data);;
		}
	}


	class DWD485ASCIILineParser : LineParser
	{
		private byte[] lineBreak = { (byte)0x01 };

		List<byte> list = new List<byte>();

		public DWD485ASCIILineParser()
		{
		}

		public byte[] LineBreak
		{
			get { return this.lineBreak; }
			set { this.lineBreak = value; }
		}

		private int IndexLineBreak()
		{
			int index = -1;
			int count = list.Count;
			for (int i = 0; i < count; ++i)
			{
				if (list[i] == this.LineBreak[0])
				{
					bool find = true;
					for (int j = 1; j < this.LineBreak.Length && (i + j < count); ++j)
					{
						if (list[i + j] != this.LineBreak[j])
						{
							find = false;
							break;
						}
					}
					if (find)
					{
						return i;
					}
				}
			}
			return index;
		}

		public override byte[] ContinueWith(byte[] data)
		{
			byte[] line = DataParser.EmptyByteArray;
			for (int i = 0; i < data.Length; ++i)
			{
				list.Add(data[i]);
			}

			int p = this.IndexLineBreak();

			if (p > 0)
			{
				byte[] ret = new byte[p];
				list.CopyTo(0, ret, 0, p);
				list.RemoveRange(0, p);
				return ret;
			}
			else if (p == 0)
			{
				list.RemoveRange(0, this.LineBreak.Length);
				return line;
			}

			return line;
		}
	}
	

	
}
