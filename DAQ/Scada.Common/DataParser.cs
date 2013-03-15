using Scada.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public abstract class DataParser
	{
		public abstract string[] Search(string data);
	}

	/// <summary>
	/// 
	/// </summary>
	public class PatternDataParser : DataParser
	{
		public PatternDataParser(string pattern)
		{
			this.pattern = pattern;
		}
		private string pattern;

        Scanner scanner = new Scanner();


		public override string[] Search(string data)
		{
            string[] ret = null;
            try
            {
                ret = scanner.Scan(data, this.pattern);
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
