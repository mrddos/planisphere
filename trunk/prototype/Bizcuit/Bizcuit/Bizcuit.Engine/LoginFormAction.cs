using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizcuit.Common;
using Bizcuit.Engine.WebSupport;

namespace Bizcuit.Engine.Access
{
	public class LoginFormAction : IBizRenderAction
	{
		private WebRender webRender = new WebRender();

		private IBizActionDigest digest = null;

		public LoginFormAction(IBizActionDigest digest)
		{
			this.digest = digest;
		}
		#region IBizAction Members

		public void Perform(IBizActionRequest request, IBizActionResponse response)
		{

			int i = 0;

			if (digest != null)
			{
				if (digest.GetReturnContentType() == "text/html")
				{
					if (digest.GetReturnContent() == null)
					{
						response.Content = webRender.LoadFromSource(digest.GetReturnContentSource());
					}
				}
			}

		}

		#endregion
	}
}
