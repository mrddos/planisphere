using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionNode
	{

		IBizAction Action { get; }

		void ApplyConfig(IBizActionConfig config);


		IBizActionNode GetNextActionNodeOnCondition(IBizCondition condition);

		void SetNextActionNodeOnCondition(IBizCondition condition, IBizActionNode actionNode);
	
	}
}
