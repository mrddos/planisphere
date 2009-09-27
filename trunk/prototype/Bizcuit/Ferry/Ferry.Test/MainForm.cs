using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ferry.Notification;
using System.Threading;
using Ferry.Debug;

namespace Ferry.Test
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			textBox1.Text = "Id: " + Thread.CurrentThread.ManagedThreadId;


		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked)
			{
				DebugForm df = new DebugForm();
				df.Show();
			}
			else
			{
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			/*
			NotifyForm nf = new NotifyForm();
			nf.SetTitle("Form in main thread");
			nf.Show();
			*/

			//Child c = new Child();

			
			new Thread(delegate()
			{
				NotifyForm n = new NotifyForm();
				n.SetTitle("Form in a thread");
				n.ShowDialog();
			}).Start();
			
			/*
			new Thread(delegate()
			{
				NotifyForm n = new NotifyForm();
				n.SetTitle("Form in a thread");
				n.ShowDialog();
			}).Start();
			*/
			
		}

		private void autoTestButton_Click(object sender, EventArgs e)
		{
			new Thread(delegate()
			{
				NotifyAutoRegisterTest n = new NotifyAutoRegisterTest();
				n.Text = "Form in a thread";
				n.ShowDialog();
			}).Start();
		}
	}
}