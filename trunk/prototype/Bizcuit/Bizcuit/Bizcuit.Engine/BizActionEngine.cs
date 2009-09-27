using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	class BizActionEngine
	{



		public void Initialize()
		{
			//TODO: Fill the BizActionFlowDirectory
			// ./login => {login, }
		}

		//<actionflow name="login">
		//  <actions>
		//    <action class="LoginFormAction"/>
		//    <action class="RegisterFormAction"/>
		//  </actions>
		//
		//</actionflow>
		//
		public void ProcessAction(object /*TODO: Add class HttpRequest*/Request, object /*TODO: Add class HttpResponse*/Response)
		{

			BizActionFlowDirectory actionFlowDir = new BizActionFlowDirectory();


			BizActionFlowDigest digest = actionFlowDir.GetActionFlowDigest(Request);

			IBizActionFlow actionFlow = digest.CreateActionFlow();

			Run(actionFlow, digest.GetConfig());
		}


		public void Run(IBizActionFlow actionFlow, IBizActionFlowConfig config)
		{
			IBizActionFlowContainer actionFlowContainer = new BizActionFlowContainer();
			actionFlowContainer.SetActionFlow(actionFlow);
			actionFlowContainer.ApplyConfig(config);

			actionFlowContainer.Execute();
		}
	}
}
