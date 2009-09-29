using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	public class BizActionFlowDigest : IBizActionFlowDigest
	{
		private string actionFlowName = null;


		public BizActionFlowDigest(string actionFlowName)
		{
			this.actionFlowName = actionFlowName;
		}

		public string ActionFlowName
		{
			get { return actionFlowName; }
		}









		public class BizActionDigest : IBizActionDigest
		{
			private string actionName = null;
			private string actionClassName = null;

			private Dictionary<string, string> nextActionDict = new Dictionary<string,string>();

			internal BizActionDigest(string actionName)
			{
				this.actionName = actionName;
			}

			#region IBizActionDigest Members

			string IBizActionDigest.Name
			{
				get { return actionName; }
			}

			string IBizActionDigest.ClassName
			{
				get
				{
					return actionClassName;
				}
				set
				{
					actionClassName = value;
				}
			}

			void IBizActionDigest.SetNextActionOnCondition(string condition, string nextActionName)
			{
				nextActionDict.Add(condition, nextActionName);
			}

			#endregion
		} // BizActionDigest end




		#region IBizActionFlowDigest Members

		IBizActionDigest IBizActionFlowDigest.AddActionDigest(string actionName)
		{
			IBizActionDigest actionDigest = new BizActionDigest(actionName);


			return actionDigest;
		}


		IBizActionFlow IBizActionFlowDigest.CreateActionFlow()
		{
			throw new NotImplementedException();
		}

		IBizActionFlowConfig IBizActionFlowDigest.GetConfig()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
