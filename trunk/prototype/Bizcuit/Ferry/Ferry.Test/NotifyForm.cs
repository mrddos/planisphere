using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ferry.Notification;
using Ferry.Notification.Attributes;
using System.Threading;
using Ferry.Test;

namespace Ferry.Test
{
	public partial class NotifyForm : Form
	{

		private string title = null;

		private bool connected = false;

		public NotifyForm()
		{
			InitializeComponent();
		}

		public void SetTitle(string title)
		{
			this.title = title;
		}

		private void NotifyForm_Load(object sender, EventArgs e)
		{
			this.Text = title;
			label1.Text = "Id: " + Thread.CurrentThread.ManagedThreadId;

			NotifySystem.SetCurrentThreadInvoke(this.Invoke, this.BeginInvoke);
			NotifySystem ns = NotifySystem.GetNotifySystem("net");
			INotifyConnection conn = ns.Connect("some", this, "OnSomeNotify", new Type[] { typeof(string), typeof(int) });

			if (conn != null)
			{
				conn.InvokePolicy = InvokePolicy.SynchronizedInvoke;
				conn.Available = true;

				connected = true;
			}


			if (connected)
			{
				buttonDisconnect.Text = "Disconnect";
			}
		}

		[NotifyRegister]
		public void OnSomeNotify(string a, int b)
		{
			// NotifySystem ns = NotifySystem.GetNotifySystem("net");
			// INotifyConnection conn = ns.GetConnection("some", this, "OnSomeNotify", new Type[] { typeof(int), typeof(int) });

			int id = Thread.CurrentThread.ManagedThreadId;
			// Need UI Thread Invoke.
			textBox1.Text = "message: " + a + b;
		}

		[NotifyRegister]
		public void OnSomeNotify(int a, int b, string c)
		{
			NotifySystem ns = NotifySystem.GetNotifySystem("net");
			INotifyConnection conn = ns.GetConnection("some", this, "OnSomeNotify");

			// Need UI Thread Invoke.
			textBox1.Text = conn.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NotifySystem ns = NotifySystem.GetNotifySystem("net");
			ns.Notify("some", "On Notify from thread-", Thread.CurrentThread.ManagedThreadId);
		}

		private void ButtonDisconnect_OnClick(object sender, EventArgs e)
		{
			if (connected)
			{
				NotifySystem ns = NotifySystem.GetNotifySystem("net");
				bool disconnect = ns.Disconnect("some", this, "OnSomeNotify", new Type[] { typeof(string), typeof(int) });

				if (disconnect)
				{
					connected = false;
					textBox1.Text = @"Disconnected OnSomeNotify from notify://some";
					buttonDisconnect.Text = "Reconnect";

				}
				else
				{
					textBox1.Text = @"Disconnected Error!";
				}
			}
			else
			{
				NotifySystem ns = NotifySystem.GetNotifySystem("net");
				INotifyConnection conn = ns.Connect("some", this, "OnSomeNotify", new Type[] { typeof(string), typeof(int) });
				if (conn != null)
				{
					connected = true;
					conn.InvokePolicy = InvokePolicy.SynchronizedInvoke;
					conn.Available = true;

					textBox1.Text = "Reconnected";

					buttonDisconnect.Text = "Disconnect";
				}
				else
				{
					textBox1.Text = "Reconnected Error!";
				}
			}

		}

		private void NotifyForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			NotifySystem ns = NotifySystem.GetNotifySystem("net");
			bool disconnect = ns.Disconnect("some", this, "OnSomeNotify", new Type[] { typeof(string), typeof(int) });
		}

	}


	class Child : Base
	{
		public Child()
		{
			NotifySystem ns = NotifySystem.GetNotifySystem("net");
			INotifyConnection conn = ns.Connect("some", this, "OnSomeNotify", new Type[] { typeof(string), typeof(int) });

			conn.InvokePolicy = InvokePolicy.SynchronizedInvoke;
			/*  NOTICE  */
			conn.Available = true;		// ! /*NOTICE*/ !

		}
	}
}