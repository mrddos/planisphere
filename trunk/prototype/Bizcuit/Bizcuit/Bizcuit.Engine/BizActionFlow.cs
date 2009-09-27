using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{
	public class BizActionFlow : IBizActionFlow
	{
		// Full name of a Action class.
		private string entryActionClassName = null;





		public string Id
		{
			get { return "action.flow:[" + this.GetHashCode() + "]"; }
		}

	}
}
