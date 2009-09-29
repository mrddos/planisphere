using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{

	//TODO: Maybe need an interface named IBizActionRequest
	public class BizActionRequest : IBizActionRequest
	{

		private string actionCommand = null;

		string IBizActionRequest.ActionCommnad
		{
			get { return actionCommand; }
			set { actionCommand = value; }
		}
	}
}
