using Scada.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class DataParser
	{
		private string pattern;

		public string Pattern
		{
			private get { return this.pattern; }
			set { this.pattern = value; }
		}

		public string[] Search(string data)
		{
			Scanner scanner = new Scanner();

			string[] ret = scanner.Scan(data, this.Pattern);
		
			return ret;
		}

	}
}
