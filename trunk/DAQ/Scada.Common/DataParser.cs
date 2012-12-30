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
			get { return this.pattern; }
			set { this.pattern = value; }
		}

		public List<string> Search(string data)
		{
			Scanner scanner = new Scanner();

			object[] r = scanner.Scan("0.00 1.2 3.4  5.0", this.Pattern);

			return null;
		}

	}
}
