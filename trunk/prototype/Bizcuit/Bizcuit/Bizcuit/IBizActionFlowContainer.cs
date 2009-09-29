using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{

	public enum BizActionFlowContainerStatus : int
	{
		Unready,
		Ready,
		Running,
		Completed
	}


	public interface IBizActionFlowContainer
	{

		void ApplyConfig(IBizActionFlowConfig config);


		BizActionFlowContainerStatus Status { get; }


		void Activate();

		void SetActionFlow(IBizActionFlow actionFlow);
		void SetActionFlowDigest(IBizActionFlowDigest digest);
		void Execute(IBizActionRequest request, IBizActionResponse response);
	}
}
