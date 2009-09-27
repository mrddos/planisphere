using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionDigest
	{

		string Name { get; }

		string ClassName { get; set; }

		void SetNextActionOnCondition(string condition, string nextActionName);
	}
}
