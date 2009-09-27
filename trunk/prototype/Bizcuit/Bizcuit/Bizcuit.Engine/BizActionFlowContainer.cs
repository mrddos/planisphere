using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	class BizActionFlowContainer : IBizActionFlowContainer
	{
		private IBizActionFlow actionFlow = null;

		private IBizActionFlowConfig config = null;

		private BizActionFlowContainerStatus status = BizActionFlowContainerStatus.Unready;



		public void ApplyConfig(IBizActionFlowConfig config)
		{
			this.config = config;
		}

		public BizActionFlowContainerStatus Status
		{
			get { return status; }
		}



		public void Activate()
		{
			status = BizActionFlowContainerStatus.Ready;
		}


		public void SetActionFlow(IBizActionFlow actionFlow)
		{
			this.actionFlow = actionFlow;
		}

		public void Execute()
		{
		}

	}
}
