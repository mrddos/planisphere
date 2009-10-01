using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using System.Reflection;

namespace Bizcuit.Engine
{
	class BizActionNode : IBizActionNode
	{

		private IBizAction action = null;

		private IBizActionDigest digest = null;

		private string actionName = null;

		private string actionClassName = null;

		// For lazy loading...
		public BizActionNode(string actionName, string actionClassName)
		{
			this.actionName = actionName;
			this.actionClassName = actionClassName;
		}

		public BizActionNode(IBizAction action)
		{
			this.action = action;
			
			// actionClassName = action.GetType().FullName;

		}


		public void Run(IBizActionRequest request, IBizActionResponse response)
		{
			if (action == null)
			{
				if (actionClassName != null)
				{
					Type actionType = Type.GetType(actionClassName);
					if (actionType != null)
					{
						ConstructorInfo ci = actionType.GetConstructor(new Type[] { typeof(IBizActionDigest) });
						action = (IBizAction)ci.Invoke(new object[] { this.digest });
					}
				}
			}
			action.Perform(request, response);
		}


		public IBizAction Action
		{
			get { return action; }
		}

		public void ApplyDigest(IBizActionDigest digest)
		{
			this.digest = digest;
		}


		public IBizActionNode GetNextActionNodeOnCondition(IBizCondition condition)
		{
			throw new NotImplementedException();
		}

		public void SetNextActionNodeOnCondition(IBizCondition condition, IBizActionNode action)
		{
			throw new NotImplementedException();
		}



	}
}
