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

		private string actionClassName = null;

		private IBizAction action = null;

		private IBizActionConfig config = null;

		// For lazy loading...
		public BizActionNode(string actionClassName)
		{
			this.actionClassName = actionClassName;
		}

		public BizActionNode(IBizAction action)
		{
			this.action = action;
			
			// actionClassName = action.GetType().FullName;

		}


		public void Run()
		{
			if (action == null)
			{
				if (actionClassName != null)
				{
					Type actionType = Type.GetType(actionClassName);
					if (actionType != null)
					{
						ConstructorInfo ci = actionType.GetConstructor(new Type[] { });
						action = (IBizAction)ci.Invoke(new object[] { });
					}
				}
			}
			action.Perform();
		}


		public IBizAction Action
		{
			get { return action; }
		}

		public void ApplyConfig(IBizActionConfig config)
		{
			this.config = config;
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
