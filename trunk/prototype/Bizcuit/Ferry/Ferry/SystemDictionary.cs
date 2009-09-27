using System;
using System.Collections.Generic;
using System.Text;

using Ferry.Notification;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Ferry
{

	internal static class SystemDictionary
	{
		/// <summary>
		/// Repository for all elements
		/// </summary>
		private static Dictionary<string, ISystemElement> dict = new Dictionary<string, ISystemElement>();

		internal static Dictionary<string, ISystemElement> GetProfile()
		{
			return dict;
		}



		internal static ISystemElement GetSystemElement(string key)
		{
			return dict[key];
		}

		/// <summary>
		/// retrieve a notify name with the format notify://domain
		/// </summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		private static string GetNotifySystemName(string domain)
		{
			return GetNotifyName(domain, "", 0);
		}

		private static string GetNotifyName(string domain, int threadId)
		{
			return GetNotifyName(domain, "", threadId);
		}

		/// <summary>
		/// Get NotifyProperty name, depend on domain name and notify name(short).
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="notify"></param>
		/// <returns></returns>
		private static string GetNotifyPropertyName(string domain, string notify)
		{
			return GetNotifyName(domain, notify, 0);
		}

		private static string GetNotifySetName(string domain, string notify, int threadId)
		{
			return GetNotifyName(domain, notify, threadId);
		}

		/// <summary>
		/// notify://domain/notify/thread
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="notify"></param>
		/// <param name="threadId"></param>
		/// <returns></returns>
		private static string GetNotifyName(string domain, string notify, int threadId)
		{
			StringBuilder sb = new StringBuilder("notify://");
			sb.Append(domain);

			if (notify != null && notify.Length > 0)
			{
				sb.Append("/").Append(notify);
			}

			if (threadId != 0)
			{
				sb.Append("/").Append(threadId);
			}
			return sb.ToString();
		}


		internal static NotifySystem GetNotifySystem(string domain)
		{
			string name = GetNotifySystemName(domain);
			if (dict.ContainsKey(name))
			{
				NotifySystem ns = dict[name] as NotifySystem;
				return ns;
			}
			else
			{
				NotifySystem ns = NotifySystem.CreateNotifySystem(domain);
				dict.Add(name, ns);
				return ns;
			}
		}

		/// <summary>
		/// Get NotifyThreadSystem object, format as thread@1024;
		/// </summary>
		/// <param name="threadId"></param>
		/// <returns></returns>
		internal static NotifyThreadSystem GetNotifyThreadSystem(int threadId)
		{
			string threadSystemName = string.Format("thread@{0}", threadId);
			if (dict.ContainsKey(threadSystemName))
			{
				NotifyThreadSystem ns = dict[threadSystemName] as NotifyThreadSystem;
				return ns;
			}
			else
			{
				NotifyThreadSystem ns = NotifyThreadSystem.CreateNotifyThreadSystem(threadId);
				dict.Add(threadSystemName, ns);
				return ns;
			}
		}

		internal static NotifySet.Notification AddNotification(string domain, string notify, int threadId, object target, MethodInfo mi, bool single)
		{

			NotifySet ns = GetNotifySet(domain, notify, threadId, single);
			NotifySet.Notification n = ns.AddNotification(target, mi);
			return n;
			
		}


		internal static bool RemoveNotification(string domain, string notify, int threadId, object target, MethodInfo mi)
		{
			NotifySet ns = GetNotifySet(domain, notify, threadId);
			bool remove = ns.RemoveNotification(target, mi);
			return remove;
		}

		/// <summary>
		/// A notify set depends on domain, notify, and thread.
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="notify"></param>
		/// <param name="threadId"></param>
		/// <returns></returns>
		internal static NotifySet GetNotifySet(string domain, string notify, int threadId)
		{
			string name = GetNotifySetName(domain, notify, threadId);
			if (dict.ContainsKey(name))
			{
				NotifySet ns = dict[name] as NotifySet;
				return ns;
			}
			return null;
		}

		internal static NotifySet GetNotifySet(string domain, string notify, int threadId, bool single)
		{
			string name = GetNotifySetName(domain, notify, threadId);

			NotifyProperty np = GetNotifyProperty(domain, notify);
			if (np != null)
			{
				if (np.Single == null)
				{
					np.Single = single.ToString();
				}
			}

			if (dict.ContainsKey(name))
			{
				return dict[name] as NotifySet;
			}
			else
			{			
				NotifySet ns = NotifySet.CreateNotifySet(single);
				dict.Add(name, ns);
				return ns;
			}
			
		}

		internal static void SetNotifyProperty(string domain, string notify, bool single)
		{
			string name = GetNotifyPropertyName(domain, notify);
			if (dict.ContainsKey(name))
			{
				NotifyProperty np = dict[name] as NotifyProperty;
				np.Single = single.ToString();
			}
		}

		internal static NotifyProperty GetNotifyProperty(string domain, string notify)
		{
			string name = GetNotifyPropertyName(domain, notify);
			if (dict.ContainsKey(name))
			{
				NotifyProperty np = dict[name] as NotifyProperty;
				return np;
			}
			else
			{
				NotifyProperty np = new NotifyProperty();
				dict.Add(name, np);
				return np;
			}
		}

		internal static bool IsMulticastNotify(string domain, string notify)
		{
			string name = GetNotifyPropertyName(domain, notify);
			if (dict.ContainsKey(name))
			{
				NotifyProperty np = dict[name] as NotifyProperty;
				return np.Single.Equals(false.ToString());

			}
			return true;
		}


		[Conditional("DEBUG")]
		internal static void AssertValidNotifyName(string notify)
		{
			if (true)
			{
				throw new NotifyException("");
			}
		}

		/// <summary>
		/// Name pattern related
		/// </summary>
		/// <param name="name"></param>
		/// <param name="threadId"></param>
		/// <returns></returns>
		private static bool IsThreadMatched(string name, int threadId)
		{
			// notify://net/23/hello
			if (name.StartsWith("notify:"))
			{
				int i = name.IndexOf('/', 8);
				if (i > 0)
				{
					int e = name.IndexOf('/', i);
					string t = name.Substring(i, e - i);
					return (Int32.Parse(t) == threadId);
				}
			}
			else if (name.StartsWith("event:"))
			{
				int i = name.IndexOf('/', 7);
				if (i > 0)
				{
					int e = name.IndexOf('/', i);
					string t = name.Substring(i, e - i);
					return (Int32.Parse(t) == threadId);
				}
			}
			return false;
		}

		/// <summary>
		/// Name pattern related
		/// </summary>
		/// <param name="name"></param>
		/// <param name="domain"></param>
		/// <returns></returns>
		private static bool IsDomainMatched(string name, string domain)
		{
			if (name.StartsWith("notify:"))
			{
				int i = name.IndexOf('/', 8);
				if (i > 0)
				{
					int e = name.IndexOf('/', 8);
					string d = name.Substring(8, e - 8);
					return (d == domain);
				}
			}
			else if (name.StartsWith("event:"))
			{
				int i = name.IndexOf('/', 7);
				if (i > 0)
				{
					int e = name.IndexOf('/', 7);
					string d = name.Substring(7, e - 7);
					return (d == domain);
				}
			}
			return false;
		}

		/// <summary>
		/// Name pattern related
		/// </summary>
		/// <param name="name"></param>
		/// <param name="word"></param>
		/// <returns></returns>
		private static bool IsWordMatched(string name, string word)
		{
			int l = name.LastIndexOf('/');
			return name.Substring(l).Equals(word);
		}

		/// <summary>
		/// Debug?
		/// </summary>
		/// <param name="tw"></param>
		public static void Profile(TextWriter tw)
		{
			foreach (string k in dict.Keys)
			{
				tw.WriteLine(k);
			}
		}

		public static void Profile(TextWriter tw, int threadId)
		{
			foreach (string k in dict.Keys)
			{
				if (IsThreadMatched(k, threadId))
				{
					tw.WriteLine(k);
				}
			}
		}

		public static void Profile(TextWriter tw, string domain)
		{
			foreach (string k in dict.Keys)
			{
				if (IsDomainMatched(k, domain))
				{
					tw.WriteLine(k);
				}
			}
		}



	}
}
