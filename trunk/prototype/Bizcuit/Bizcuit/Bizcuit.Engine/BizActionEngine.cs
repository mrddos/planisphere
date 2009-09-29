using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	public class BizActionEngine
	{
		private BizActionFlowDirectory actionFlowDir = new BizActionFlowDirectory();

		private IBizActionFlowConfig config = new BizActionFlowConfig();

		public void Initialize()
		{
			//TODO: Fill the BizActionFlowDirectory
			// ./login => {login, }

			BizActionFlowConfig.Load("config/access.default.xml", config);
		}

		//<actionflow name="login">
		//  <actions>
		//    <action class="LoginFormAction"/>
		//    <action class="RegisterFormAction"/>
		//  </actions>
		//
		//</actionflow>
		//
		public void ProcessAction(IBizActionRequest request, IBizActionResponse response)
		{

			string actionFlowName = actionFlowDir.GetActionFlowDigest(request);
			IBizActionFlowDigest digest = config.GetActionFlowDigest(actionFlowName);

			// TODO: Create a new ActionFlow, or get from the session?
			IBizActionFlow actionFlow = digest.CreateActionFlow();

			Run(actionFlow, digest.GetConfig(), request, response);
		}


		public void Run(IBizActionFlow actionFlow, IBizActionFlowConfig config, IBizActionRequest request, IBizActionResponse response)
		{
			IBizActionFlowContainer actionFlowContainer = new BizActionFlowContainer();
			actionFlowContainer.SetActionFlow(actionFlow);
			actionFlowContainer.ApplyConfig(config);	// TODO, 换成ApplyDigest更好.


			string actionFlowName = actionFlowDir.GetActionFlowDigest(request);
			actionFlowContainer.SetActionFlowDigest(config.GetActionFlowDigest(actionFlowName));

			// ApplyConfig and SetActionFlowDigest seem to be so same... ...
			actionFlowContainer.Execute(request, response);
		}
	}
}
