using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionFlowDigest
	{
		IBizActionDigest AddActionDigest(string actionName);

		IBizActionFlow CreateActionFlow();

		IBizActionFlowConfig GetConfig();
	}
}
