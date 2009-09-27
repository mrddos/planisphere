using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using Ferry.Notification.Attributes;

namespace Ferry.Notification
{

	public class NotifySystem : ISystemElement
	{

		private static List<string> domainList = new List<string>();

		private static bool threadSwitchOnce = false;

		private string domain = null;

		private List<int> threadIdList = new List<int>();

		private Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

		private int CurrentThreadId
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <summary>
		/// Get a NotifySystem instance by domain name
		/// </summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		public static NotifySystem GetNotifySystem(string domain)
		{
			return SystemDictionary.GetNotifySystem(domain);
		}


		private NotifySystem(string domain)
		{
			this.domain = domain;
		}

		internal static NotifySystem CreateNotifySystem(string domain)
		{
			if (!domainList.Contains(domain))
			{
				domainList.Add(domain);
			}
			return new NotifySystem(domain);
		}

		public static bool? GetThreadPolicy(string item)
		{
			if (string.Compare(item, "switch.once", true) == 0)
			{
				return threadSwitchOnce;
			}

			return null;
		}

		public static void SetThreadPolicy(string item, string value)
		{
			if (string.Compare(item, "switch.once", true) == 0)
			{
				threadSwitchOnce = bool.Parse(value);
			}

			
		}


		public static void AddNotifyListener(object target, InvokePolicy invokePolicy)
		{
			/**
			 * Method should be Public, and an instance method.
			 */
			MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
			
			foreach (MethodInfo mi in methods)
			{
				object[] attributes = mi.GetCustomAttributes(false);
				foreach (object e in attributes)
				{
					if (e is NotifyAutoRegisterAttribute)
					{
						NotifyAutoRegisterAttribute attr = e as NotifyAutoRegisterAttribute;
						
						string methodName = mi.Name;

						NotifySystem ns = GetNotifySystem(attr.DomainName);
						if (ns != null)
						{
							INotifyConnection conn = ns.Connect(attr.NotifyName, target, methodName);
							conn.InvokePolicy = invokePolicy;
							conn.Available = true;
						}
					}
				}
			}
		}


		public delegate bool AddNotifyListenerPolicy(string methodName, INotifyConnection conn);

		public static void AddNotifyListener(object target, AddNotifyListenerPolicy addNotifyListenerPolicy)
		{
			/**
			 * Method should be Public, and an instance method.
			 */
			MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

			foreach (MethodInfo mi in methods)
			{
				object[] attributes = mi.GetCustomAttributes(false);
				foreach (object e in attributes)
				{
					if (e is NotifyAutoRegisterAttribute)
					{
						NotifyAutoRegisterAttribute attr = e as NotifyAutoRegisterAttribute;

						string methodName = mi.Name;

						NotifySystem ns = GetNotifySystem(attr.DomainName);
						if (ns != null)
						{
							INotifyConnection conn = ns.Connect(attr.NotifyName, target, methodName);
							// conn.InvokePolicy = invokePolicy;
							// conn.Available = true;
							addNotifyListenerPolicy(methodName, conn);
						}
					}
				}
			}
		}

		public static void RemoveNotifyListener(object target)
		{
		}

		private INotifyConnection AddConnection(string notify, object target, string methodName, Type[] typeList, bool single)
		{
			MethodInfo mi = null;
			if (typeList == null)
			{
				mi = target.GetType().GetMethod(methodName);
			}
			else
			{
				mi = target.GetType().GetMethod(methodName, typeList);
			}
			if (mi == null)
			{
				throw new NotifyException(Ferry.E_NoMethodFound);
			}
			// object[] attributes = mi.GetCustomAttributes(false);
			// foreach (object e in attributes)
			// {
			// }

			NotifyThreadSystem nts = GetNotifyThreadSystem();
			if (nts == null)
			{
				throw new NotifyException(Ferry.E_NoNotifyThreadSystem);
			}
			NotifySet.Notification n = null;
			if (single)
			{
				INotifyConnection exist = GetConnection(notify, target, methodName, typeList);
				if (exist != null)
				{
					throw new NotifyException(Ferry.E_MoreNotificationPoint);
				}
				n = nts.AddNotification(domain, notify, target, mi, single);
				AddThreadId();
			}
			else
			{
				n = nts.AddNotification(domain, notify, target, mi, single);
				AddThreadId();
			}

			return n;
		}

		/// <summary>
		/// Connect
		/// </summary>
		/// <param name="notify"></param>
		/// <param name="target"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public INotifyConnection Connect(string notify, object target, string methodName)
		{
			INotifyConnection conn = AddConnection(notify, target, methodName, null, false);
			return conn;
		}

		public INotifyConnection Connect(string notify, object target, string methodName, Type[] typeList)
		{
			INotifyConnection conn = AddConnection(notify, target, methodName, typeList, false);
			return conn;
		}

		public INotifyConnection Connect(string notify, object target, string methodName, Type[] typeList, bool single)
		{
			INotifyConnection conn = AddConnection(notify, target, methodName, typeList, single);
			return conn;
		}

		/// <summary>
		/// GetConnection
		/// </summary>
		/// <param name="notify"></param>
		/// <param name="target"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public INotifyConnection GetConnection(string notify, object target, string methodName)
		{
			return GetConnection(notify, target, methodName, null);
		}

		public INotifyConnection GetConnection(string notify, object target, string methodName, Type[] typeList)
		{
			foreach (int threadId in threadIdList)
			{
				NotifySet ns = SystemDictionary.GetNotifySet(domain, notify, threadId);

				MethodInfo mi = null;
				if (typeList == null)
				{
					mi = target.GetType().GetMethod(methodName);
				}
				else
				{
					mi = target.GetType().GetMethod(methodName, typeList);
				}
				if (mi == null)
				{
					throw new NotifyException("");
				}
				NotifySet.Notification n = ns.FindNotification(target, mi);
				if (n != null)
				{
					return n;
				}
			}
			return null;

		}

		public bool Disconnect(string notify, object target, string methodName)
		{
			return Disconnect(notify, target, methodName, null);
		}

		public bool Disconnect(string notify, object target, string methodName, Type[] typeList)
		{
			
			foreach (int threadId in threadIdList)
			{
				NotifySet ns = SystemDictionary.GetNotifySet(domain, notify, threadId);

				MethodInfo mi = null;
				if (typeList == null)
				{
					mi = target.GetType().GetMethod(methodName);
				}
				else
				{
					mi = target.GetType().GetMethod(methodName, typeList);
				}
				if (mi == null)
				{
					throw new NotifyException("");
				}

				bool remove = SystemDictionary.RemoveNotification(domain, notify, threadId, target, mi);

			}

			return true;
		}


		private NotifyThreadSystem GetNotifyThreadSystem()
		{
			return this.GetNotifyThreadSystem(CurrentThreadId);
		}

		private NotifyThreadSystem GetNotifyThreadSystem(int threadId)
		{
			return SystemDictionary.GetNotifyThreadSystem(threadId);
		}


		public static void SetCurrentThreadInvoke(Invoke syncInvoke, Invoke asynInvoke)
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;
			NotifyThreadSystem nts = SystemDictionary.GetNotifyThreadSystem(threadId);
			nts.AsynInvoke = asynInvoke;
			nts.SyncInvoke = syncInvoke;

		}

		private bool AddThreadId()
		{
			if (!threadIdList.Contains(CurrentThreadId))
			{
				threadIdList.Add(CurrentThreadId);
				return true;
			}
			return false;
		}

		
		public void Notify(string notify, params object[] objects)
		{
			foreach (int threadId in threadIdList)
			{
				NotifyThreadSystem nts = SystemDictionary.GetNotifyThreadSystem(threadId);
				nts.Notify(domain, notify, objects);
			}
		}


		public static Dictionary<string, ISystemElement>.KeyCollection Profile()
		{
			return SystemDictionary.GetProfile().Keys;
		}

		public static ISystemElement GetSystemElement(string key)
		{
			return SystemDictionary.GetSystemElement(key);
		}

		#region ISystemElement Members

		object ISystemElement.GetAttributeByKey(string key)
		{
			switch (key)
			{
				case "domain":
					return domain;
				case "count.thread":
					return threadIdList.Count;
			}
			return null;
		}

		#endregion
	}
}
