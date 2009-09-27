using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Ferry.Notification
{

	/// <summary>
	/// A NotifySet contains one or more Notification object.
	/// </summary>
	internal partial class NotifySet : ISystemElement
	{

		private static InvokeProxy invokeProxy = null;

		private static InvokeAllProxy invokeAllProxy = null;


		private List<Notification> notifyList = new List<Notification>();


		private bool single = false;


		public bool Single
		{
			get
			{
				return single;
			}
		}

		private static object Proxy(NotifySet.Notification n, object[] objects)
		{
			object r = n.Notify(objects);
			return null;
		}

		/**
		 * NOTICE: this function would be called on the UI thread stack.
		 * So, InvokePolicy.DirectlyInvoke would be as the invoke policy.
		 * But Sync, and Async... ...
		 */
		private static object ProxyForAll(List<Notification> notifyList, object[] objects)
		{
			foreach (Notification n in notifyList)
			{
				INotifyConnection nc = n as INotifyConnection;

			}
			return null;
		}

		static NotifySet()
		{
			invokeProxy = new InvokeProxy(Proxy);
			invokeAllProxy = new InvokeAllProxy(ProxyForAll);
		}

		private NotifySet(bool single)
		{
			this.single = single;

		}

		public static NotifySet CreateNotifySet(bool single)
		{
			return new NotifySet(single);
		}

		internal Notification AddNotification(object target, MethodInfo mi)
		{
			Notification n = new Notification(target, mi);
			notifyList.Add(n);

			return n;
		}


		class MethodMatcher
		{
			private object target = null;

			private MethodInfo mi = null;

			public MethodMatcher(object target, MethodInfo mi)
			{
				this.target = target;
				this.mi = mi;
			}

			public bool Matches(Notification n)
			{
				return target.Equals(n.Target) && mi.Equals(n.Method);
			}
		}




		internal Notification FindNotification(object target, MethodInfo mi)
		{
			if (single)
			{
				return notifyList[0];
			}
			else
			{
				MethodMatcher m = new MethodMatcher(target, mi);
				Notification n = notifyList.Find(m.Matches);
				return n;
			}
		}

		internal bool RemoveNotification(object target, MethodInfo mi)
		{
			MethodMatcher m = new MethodMatcher(target, mi);
			int index = notifyList.FindIndex(m.Matches);
			if (index >= 0)
			{
				notifyList.RemoveAt(index);
				return true;
			}
			return false;
			
		}


		internal void NotifyAll(Invoke syncInvoke, Invoke asynInvoke, params object[] objects)
		{
			bool? switchOnce = NotifySystem.GetThreadPolicy("switch.once");
			if (switchOnce == false)
			{
				foreach (Notification n in notifyList)
				{
					INotifyConnection nc = n as INotifyConnection;
					switch (nc.InvokePolicy)
					{
						case InvokePolicy.DirectlyInvoke:
							n.Notify(objects);
							break;
						case InvokePolicy.SynchronizedInvoke:
							syncInvoke(invokeProxy, n, objects);
							break;
						case InvokePolicy.AsynchronousInvoke:
							asynInvoke(invokeProxy, n, objects);
							break;
					}

				}
			}
			else if (switchOnce == true)
			{
				// the code should be codereview!!!
				bool hasSyncPolicy = false;
				bool hasAsynPolicy = false;
				foreach (Notification n in notifyList)
				{
					INotifyConnection nc = n as INotifyConnection;
					if (nc.InvokePolicy == InvokePolicy.DirectlyInvoke)
					{
						n.Notify(objects);
					}
					else if (nc.InvokePolicy == InvokePolicy.AsynchronousInvoke)
					{
						hasAsynPolicy = true;
					}
					else if (nc.InvokePolicy == InvokePolicy.SynchronizedInvoke)
					{
						hasSyncPolicy = true;
					}
				}

				if (hasSyncPolicy)
				{
					syncInvoke(invokeAllProxy, notifyList, objects);
				}

				if (hasAsynPolicy)
				{
					asynInvoke(invokeAllProxy, notifyList, objects);
				}

			}
		}


		#region ISystemElement Members

		object ISystemElement.GetAttributeByKey(string key)
		{
			switch (key)
			{
				case "list":
					return "";
			}
			return null;

		}

		#endregion


	}

	internal partial class NotifySet
	{
		internal class Notification : INotifyConnection
		{

			private object target = null;


			private MethodInfo mi = null;


			private InvokePolicy InvokePolicy = InvokePolicy.DirectlyInvoke;


			private bool available = false;



			public Notification(object target, MethodInfo mi)
			{
				this.target = target;
				this.mi = mi;
			}

			internal object Notify(object[] objects)
			{
				if (available)
				{
					return mi.Invoke(target, objects);
				}
				return null;
			}


			public object Target
			{
				get
				{
					return target;
				}
			}


			public MethodInfo Method
			{
				get
				{
					return mi;
				}
			}



			#region INotifyConnection Members
			InvokePolicy INotifyConnection.InvokePolicy
			{
				get
				{
					return InvokePolicy;
				}
				set
				{
					InvokePolicy = value;
				}
			}

			bool INotifyConnection.Available
			{
				get
				{
					return available;
				}
				set
				{
					available = value;
				}
			}

			#endregion
		}
	}
}
