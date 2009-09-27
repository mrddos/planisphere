using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry
{
	public interface ISystemElement
	{
		object GetAttributeByKey(string key);
	}
}
