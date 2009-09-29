using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionFlowConfig
	{


		IBizActionFlowDigest AddActionFlowDigest(string actionFlowName);

		IBizActionFlowDigest GetActionFlowDigest(string actionFlowName);
	}
}
