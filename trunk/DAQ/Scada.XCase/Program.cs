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

			StringBuilder sb = new StringBuilder("Hello\rWorld");
			string s = sb.ToString();
			int p = s.IndexOf('\r');
			sb.Remove(0, p);

			string ass = sb.ToString();

			DataParser dp = new DataParser();
            //dp.Pattern = "D,{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String},\\s*{String}";//,{String},{String},{String},{String},{String},{String},{String},{String},!{String}";
            dp.Pattern = "D,{String} ,\\s* {String}";
			string[] a =  dp.Search("D,\"11/29/12\", \"00:01\"");//, -3.6, 44,-14.2,1024.7,327,  0.0,   0.0,!028");
		}
	}
}
