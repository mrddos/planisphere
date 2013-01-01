using Scada.Declare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.XCase
{
	class Program
	{
		static void Main(string[] args)
		{
			DataParser dp = new DataParser();
			dp.Pattern = "{Single} {Single}  {Single} {Single} {String}";

			string[] a =  dp.Search("11111.2 00.000 0.12121 3.4 5.6");
		}
	}
}
