using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry
{
	public class NotifyException : Exception
	{

		/// <summary>
		/// Exception
		/// </summary>
		/// <param name="message"></param>
		public NotifyException(string message)
			: base(message)
		{
		}
	}
}
