using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry
{
	internal class ThreadInvoke : ISystemElement
	{
		private Invoke syncInvoke = null;

		private Invoke asynInvoke = null;

		public ThreadInvoke(Invoke syncInvoke, Invoke asynInvoke)
		{
			this.syncInvoke = syncInvoke;
			this.asynInvoke = asynInvoke;
		}


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


		#region ISystemElement Members

		object ISystemElement.GetAttributeByKey(string key)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
