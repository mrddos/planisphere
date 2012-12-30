using Scada.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Scada.Watch
{
	public partial class WatchForm : Form
	{
		

		private Timer timer = null;

		private long lastKeepAliveTime = 0;

		public WatchForm()
		{
			InitializeComponent();
		}




		protected override void DefWndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case Defines.WM_KEEPALIVE:
					{
						if (m.WParam.ToInt32() == Defines.KeepAlive)
						{
							DateTime now = DateTime.Now;
							this.lastKeepAliveTime = now.Ticks;
						}
						
					}
					break;

				default:
					base.DefWndProc(ref m);
					break;
			}
		}

		private void WatchForm_Load(object sender, EventArgs e)
		{
			this.timer = new Timer();
			this.timer.Interval = Defines.KeepAliveInterval;
			this.timer.Tick += timerTick;
			this.timer.Start();

		}

		void timerTick(object sender, EventArgs e)
		{
			
		}
	}
}
