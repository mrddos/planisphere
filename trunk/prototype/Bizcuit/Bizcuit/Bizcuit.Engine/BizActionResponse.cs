using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;

namespace Bizcuit.Engine
{

	//TODO: Maybe need an interface named IBizActionResponse
	public class BizActionResponse : IBizActionResponse
	{
		private string content;

		public string Content
		{
			get { return content; }
			set { content = value; }
		}
	}
}
