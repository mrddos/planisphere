using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using com.sampullara;

namespace Bizcuit.Engine.Server
{
	class Program
	{
		static void LoadConfig(string[] args)
		{
			IBizActionFlowConfig config = new BizActionFlowConfig();
			
			BizActionFlowConfig.Load("config/access.default.xml", config);

			ListConfig(config);


			HttpServer hs = new HttpServer("web/root", 80);
		}


		static void ListConfig(IBizActionFlowConfig config)
		{
			// TODO: List the config file...
			// 这个例子也展示了如何去使用Config对象，去创建需要的Action，ActionFlow等关键对象。

			// config.GetActionFlowDigests();

		}
	}
}
