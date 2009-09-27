using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry.Notification
{
	/// <summary>
	/// 
	/// 1. Change Invoke Policy
	/// 2. 
	/// </summary>
	public interface INotifyConnection
	{

		/// <summary>
		/// 
		/// </summary>
		InvokePolicy InvokePolicy { set; get; }

		/// <summary>
		/// If available is true, the notify connection is connected.
		/// </summary>
		bool Available { set; get; }
	}
}
