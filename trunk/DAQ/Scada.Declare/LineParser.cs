using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public class LineParser
	{
		private byte[] lineBreak = { (byte)'\r', (byte)'\n' };

		List<byte> list = new List<byte>();

		public LineParser()
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
				if (list[i] == this.LineBreak[1])
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

					}
				}
			}
			return index;
		}

		public virtual byte[] ContinueWith(byte[] data)
		{
			byte[] line = null;
			for (int i = 0; i < data.Length; ++i)
			{
				list.Add(data[i]);
			}

			int p = this.IndexLineBreak();


			return line;
			/*
			string s = this.sb.ToString();
			int p = s.IndexOf(this.LineBreak);
			if (p > 0)
			{
				string line = s.Substring(0, p);
				this.sb.Remove(0, p + this.LineBreak.Length);
				return line;
			}
			else if (p == 0)
			{
				this.sb.Remove(0, this.LineBreak.Length);
				s = this.sb.ToString();
				int n = s.IndexOf(this.LineBreak);
				string line = s.Substring(0, n + LineBreak.Length);
				return line;
			}
			else
			{
				return string.Empty;
			}
			*/
		}
	}
}
