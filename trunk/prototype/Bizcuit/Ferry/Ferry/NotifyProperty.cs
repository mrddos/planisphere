using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry.Notification
{

	internal enum PropertyFlag
	{
		SingleSet = 0x0001,
		PrivateSet = 0x0002,

	}

	internal class NotifyProperty : ISystemElement
	{
		private const string undefined = "undefined";

		private bool isSingle = false;

		private bool isPrivate = false;

		private int flag = 0;


		public NotifyProperty()
		{
		}


		public string Single
		{
			get
			{
				if ((flag & (int)PropertyFlag.SingleSet) == 0)
				{
					return undefined;
				}
				return this.isSingle.ToString();
			}
			set
			{
				this.isSingle = bool.Parse(value);
			}
		}


		public string Private
		{
			get
			{
				if ((flag & (int)PropertyFlag.PrivateSet) == 0)
				{
					return undefined;
				}
				return this.isPrivate.ToString();
			}

			set
			{
				this.isPrivate = bool.Parse(value);
			}
		}

		#region ISystemElement Members

		object ISystemElement.GetAttributeByKey(string key)
		{
			if (key.Equals("single"))
			{
				return this.isSingle;
			}
			return null;
		}

		#endregion
	}
}
