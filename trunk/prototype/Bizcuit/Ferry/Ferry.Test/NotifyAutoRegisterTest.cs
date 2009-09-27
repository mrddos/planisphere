using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ferry.Notification.Attributes;
using Ferry.Notification;

namespace Ferry.Test
{
	public partial class NotifyAutoRegisterTest : Form
	{
		public NotifyAutoRegisterTest()
		{
			InitializeComponent();
		}

		private void NotifyAutoRegisterTest_Load(object sender, EventArgs e)
		{

			NotifySystem.AddNotifyListener(this, InvokePolicy.DirectlyInvoke);
		}


		[NotifyAutoRegister("net", "some")]
		public void OnNetNotify(string message)
		{
			textBox.Text = "net.some on notify";
		}

		[NotifyAutoRegister("error", "error")]
		public void OnErrorNotify(int errorId, string message)
		{
			textBox.Text = "error.error on notify";
		}

		[NotifyAutoRegister("auth", "login")]
		public bool OnLoginNotify(string userName, string session)
		{
			textBox.Text = "auth.login on notify";
			return true;
		}


	}
}