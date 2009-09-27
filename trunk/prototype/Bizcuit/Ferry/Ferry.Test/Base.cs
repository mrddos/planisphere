using System;
using System.Collections.Generic;
using System.Text;
using Ferry.Notification.Attributes;
using System.Threading;

namespace Ferry.Test
{
	class Base
	{

		[NotifyRegister]
		public void OnSomeNotify(string a, int b)
		{
			// NotifySystem ns = NotifySystem.GetNotifySystem("net");
			// INotifyConnection conn = ns.GetConnection("some", this, "OnSomeNotify", new Type[] { typeof(int), typeof(int) });

			int id = Thread.CurrentThread.ManagedThreadId;
			// Need UI Thread Invoke.
			//textBox1.Text = "message: " + a + b;
		}
	}
}
