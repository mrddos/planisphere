using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class LineParser
	{

		private string lineBreak = "\r";

		public StringBuilder sb = null;

		public LineParser()
		{
			this.sb = new StringBuilder();
		}

		public string LineBreak
		{
			get { return this.lineBreak; }
			set { this.lineBreak = value; }
		}

		public virtual string ContinueWith(string data)
		{
			this.sb.Append(data);
			
			string s = this.sb.ToString();
			int p = s.IndexOf(this.LineBreak);
			if (p >= 0)
			{
				string line = s.Substring(0, p);
				this.sb.Remove(0, p + this.LineBreak.Length);
				return line;
			}
			else if (p == 0)
			{
				// Skip the Empty line.
				this.sb.Remove(0, this.LineBreak.Length);
				return string.Empty;
			}
			else
			{
				return string.Empty;
			}
			
		}
	}
}
