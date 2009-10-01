using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionFlowDigest
	{
		IBizActionDigest AddActionDigest(string actionName, string actionClassName);

		IBizActionFlow CreateActionFlow();

		string GetActionClassName(string actionName);

		IBizActionDigest GetActionDigest(string actionName);

		string EntryActionName { get; set; }
	}
}
