using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	class BizActionFlowDirectory
	{
		private Dictionary<string, BizActionFlowDigest> actionFlowDict = new Dictionary<string, BizActionFlowDigest>();







		public BizActionFlowDigest GetActionFlowDigest(object Request)
		{
			return actionFlowDict[Request.ToString()];
		}
	}
}
