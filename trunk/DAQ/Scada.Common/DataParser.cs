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

        Scanner scanner = new Scanner();

		public string Pattern
		{
			private get { return this.pattern; }
			set { this.pattern = value; }
		}

		public string[] Search(string data)
		{
            string[] ret = null;
            try
            {
                ret = scanner.Scan(data, this.Pattern);
                return ret;
            }
            catch (ScannerExeption se)
            {
				ret = new string[] { se.Message };
				return ret;
            }
		}

	}
}
