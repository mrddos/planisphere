using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine.Pesudo
{
	public class BizSession
	{
		private IBizActionFlow actionFlow = null;

		public IBizActionFlow GetActionFlow()
		{

			return actionFlow;
		}

		public void SetActionFlow(IBizActionFlow actionFlow)
		{

			this.actionFlow = actionFlow;
		}
	}
}
