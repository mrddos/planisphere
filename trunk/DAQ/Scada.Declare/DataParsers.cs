using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public abstract class DataParser
	{
		public static readonly byte[] EmptyByteArray = new byte[0];

		protected LineParser lineParser;

		public LineParser GetLineParser()
		{
			return lineParser;
		}

		public abstract byte[] GetLineBytes(byte[] data);

		public abstract string[] Search(byte[] data);
	}
}
