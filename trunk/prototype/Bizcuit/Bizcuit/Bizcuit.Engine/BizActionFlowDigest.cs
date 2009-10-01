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

		private string entryActionName = null;

		private Dictionary<string, IBizActionDigest> actionDigestDict = new Dictionary<string, IBizActionDigest>();


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

			

			private string returnContentType = null;
			private string returnContentValue = null;
			private string returnContentSource = null;

			private Dictionary<string, string> nextActionDict = new Dictionary<string, string>();

			internal BizActionDigest(string actionName)
			{
				this.actionName = actionName;
			}

			internal BizActionDigest(string actionName, string actionClassName)
			{
				this.actionName = actionName;
				this.actionClassName = actionClassName;
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

			#region IBizActionDigest Members

			// Set, Get => change to Property...
			public void SetReturnContentType(string contentType)
			{
				this.returnContentType = contentType;
			}

			public void SetReturnContent(string contentValue)
			{
				this.returnContentValue = contentValue;
			}

			public void SetReturnContentSource(string contentSrc)
			{
				this.returnContentSource = contentSrc;
			}

			public string GetReturnContentType()
			{
				return returnContentType;
			}

			public string GetReturnContent()
			{
				return returnContentValue;
			}

			public string GetReturnContentSource()
			{
				return returnContentSource;
			}

			#endregion
		} // BizActionDigest end




		#region IBizActionFlowDigest Members

		IBizActionDigest IBizActionFlowDigest.AddActionDigest(string actionName, string actionClassName)
		{
			IBizActionDigest actionDigest = new BizActionDigest(actionName, actionClassName);
			if (!actionDigestDict.ContainsKey(actionName))
			{
				actionDigestDict.Add(actionName, actionDigest);
			}
			else
			{
				throw new Exception("Duplicate ation name.");
			}

			return actionDigest;
		}

		//或许应该把这个部分从Digest类中拿出去。Digest应该更多展示信息。
		IBizActionFlow IBizActionFlowDigest.CreateActionFlow()
		{
			string entryActionName = this.EntryActionName;
			IBizActionFlow actionFlow = new BizActionFlow(entryActionName, this);
			return actionFlow;
		}

		IBizActionDigest IBizActionFlowDigest.GetActionDigest(string actionName)
		{
			return actionDigestDict[actionName];
		}

		string IBizActionFlowDigest.GetActionClassName(string actionName)
		{
			return actionDigestDict[actionName].ClassName;/**/
		}

		public string EntryActionName
		{
			get { return entryActionName; }
			set { entryActionName = value; }
		}
		#endregion
	}
}
