using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			IBizActionFlowConfig config = new BizActionFlowConfig();
			
			config = BizActionFlowConfig.Load("config/access.default.xml", config);

		}
	}
}
