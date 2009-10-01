using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using System.Reflection;

namespace Bizcuit.Engine
{
	public class BizActionFlow : IBizActionFlow
	{
		// Full name of a Action class.
		private string entryActionName = null;

		private string currentActionName = null;

		private string nextActionName = null;

		private IBizActionFlowDigest digest = null;

		public BizActionFlow(string entryActionName, IBizActionFlowDigest digest)
		{
			this.entryActionName = entryActionName;
			this.digest = digest;
		}

		public string Id
		{
			get { return "action.flow:[" + this.GetHashCode() + "]"; }
		}


		public void Run(IBizActionRequest request, IBizActionResponse response)
		{
			if (currentActionName == null)
			{
				currentActionName = entryActionName;
			}

			IBizActionNode actionNode = CreateActionNode(currentActionName);
			actionNode.ApplyDigest(digest.GetActionDigest(currentActionName));
			actionNode.Run(request, response);

		}

		private IBizActionNode CreateActionNode(string currentActionName)
		{
			string actionClassName = digest.GetActionClassName(currentActionName);
			return new BizActionNode(currentActionName, actionClassName);
		}


	}
}