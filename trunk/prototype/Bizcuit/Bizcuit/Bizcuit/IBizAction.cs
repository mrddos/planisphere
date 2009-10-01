using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizAction
	{


		void Perform(IBizActionRequest request, IBizActionResponse response);
	}
}
