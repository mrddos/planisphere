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
			return data;
		}
	}

	
}
