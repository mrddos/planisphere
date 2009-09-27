using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry.Notification.Attributes
{
	/// <summary>
	/// 
	/// domain
	/// notify
	/// synchronized?
	/// single
	/// 
	/// </summary>
    [AttributeUsage(AttributeTargets.Method)]
	public class NotifyRegisterAttribute : Attribute
	{


		public NotifyRegisterAttribute()
		{

		}


	}
}
