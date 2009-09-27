using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry.Notification.Attributes
{

	[AttributeUsage(AttributeTargets.Method)]
	public class NotifyAutoRegisterAttribute : Attribute
	{
		private string domain = null;


		private string notify = null;

		public NotifyAutoRegisterAttribute(string domain, string notify)
		{
			this.domain = domain;
			this.notify = notify;
		}

		public string DomainName
		{
			get
			{
				return domain;
			}
		}

		public string NotifyName
		{
			get
			{
				return notify;
			}
		}

	}
}
