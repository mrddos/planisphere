using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionNode
	{

		IBizAction Action { get; }
		void Run(IBizActionRequest request, IBizActionResponse response);

		void ApplyDigest(IBizActionDigest digest);


		IBizActionNode GetNextActionNodeOnCondition(IBizCondition condition);

		void SetNextActionNodeOnCondition(IBizCondition condition, IBizActionNode actionNode);
	
	}
}
