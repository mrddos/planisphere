using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	class BizActionFlowDirectory
	{
		private Dictionary<string, string> actionFlowDict = new Dictionary<string, string>();



		public BizActionFlowDirectory()
		{
			// TODO: Add a command => ActionFlow using C# code for test.
			// TODO: Complete the config part.
			

			// Later, some url should be check permission?
			// 
			actionFlowDict.Add("login", "access.default.login.form");
		}


		// TODO: Add default and wrong action command handler.
		public string GetActionFlowDigest(string actionCommand)
		{
			return actionFlowDict[actionCommand];
		}
	}
}
