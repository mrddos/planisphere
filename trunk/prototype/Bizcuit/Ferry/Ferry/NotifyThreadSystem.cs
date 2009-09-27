using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Ferry.Notification
{

	internal class NotifyThreadSystem : ISystemElement
	{

		private int threadId = 0;


		


		private Invoke syncInvoke = null;

		private Invoke asynInvoke = null;

		/*
		public ThreadInvoke(Invoke syncInvoke, Invoke asynInvoke)
		{
			this.syncInvoke = syncInvoke;
			this.asynInvoke = asynInvoke;
		}
		*/

		




		public Invoke SyncInvoke
		{
			get
			{
				return this.syncInvoke;
			}
			set
			{
				this.syncInvoke = value;
			}
		}

		public Invoke AsynInvoke
		{
			get
			{
				return this.asynInvoke;
			}
			set
			{
				this.asynInvoke = value;
			}
		}



		private NotifyThreadSystem(int threadId)
		{
			this.threadId = threadId;


		}

		public static NotifyThreadSystem CreateNotifyThreadSystem(int threadId)
		{
			return new NotifyThreadSystem(threadId);
		}



		public void Notify(string domain, string notify, params object[] objects)
		{

			NotifySet ns = SystemDictionary.GetNotifySet(domain, notify, threadId);
			ns.NotifyAll(syncInvoke, asynInvoke, objects);
		}

		internal NotifySet.Notification AddNotification(string domain, string notify, object target, MethodInfo mi, bool single)
		{
			NotifySet.Notification n = null;
			n = SystemDictionary.AddNotification(domain, notify, threadId, target, mi, single);

			return n;
		}

		#region ISystemElement Members

		object ISystemElement.GetAttributeByKey(string key)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
