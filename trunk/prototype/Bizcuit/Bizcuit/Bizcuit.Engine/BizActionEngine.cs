using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using Bizcuit.Engine.Pesudo;

namespace Bizcuit.Engine
{
	public class BizActionEngine
	{
		private BizActionFlowDirectory actionFlowDir = new BizActionFlowDirectory();

		private IBizActionFlowConfig config = new BizActionFlowConfig();

		// TODO:! use a real session, this session just for one person.
		// 我们需要一个真的Session，这个只是为了程序能跑通概念流程。
		private BizSession session = new BizSession();

		public void Initialize()
		{
			//TODO: Fill the BizActionFlowDirectory
			// ./login => {login, }

			BizActionFlowConfig.Load("config/access.default.xml", config);
		}


		public bool CanProcess(string actionCommand)
		{
			string actionFlowName = actionFlowDir.GetActionFlowDigest(actionCommand);

			return (actionFlowName != null);
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



			Run(request, response);
		}




		public void Run(IBizActionRequest request, IBizActionResponse response)
		{
			string actionFlowName = actionFlowDir.GetActionFlowDigest(request.ActionCommnad);
			IBizActionFlowDigest digest = config.GetActionFlowDigest(actionFlowName);


			// TODO: Create a new ActionFlow, or get from the session?
			// TODO: 需要处理可中断的ActionFlow。
			IBizActionFlow actionFlow = session.GetActionFlow();
			if (actionFlow == null)
			{
				actionFlow = digest.CreateActionFlow();
				session.SetActionFlow(actionFlow);
			}

			IBizActionFlowContainer actionFlowContainer = new BizActionFlowContainer();
			actionFlowContainer.SetActionFlow(actionFlow);
			actionFlowContainer.ApplyConfig(config);	// TODO, 换成ApplyDigest更好.
			actionFlowContainer.ApplyDigest(digest);

			actionFlowContainer.SetActionFlowDigest(config.GetActionFlowDigest(actionFlowName));

			// ApplyConfig and SetActionFlowDigest seem to be so same... ...
			actionFlowContainer.Execute(request, response);
		}
	}
}
