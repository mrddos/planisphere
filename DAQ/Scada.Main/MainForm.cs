using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Scada.Controls;
using Scada.Main.Properties;
using System.Threading;
using System.Diagnostics;

namespace Scada.Main
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
			////////////////////////////////////////////////////////////////
			// Notify Icon
			// sysNotifyIcon.Icon = new Icon(Resources.AppIcon, new Size(16, 16));
			// sysNotifyIcon.Visible = true;



            startMenuItem_Click(null, null);

            ////////////////////////////////////////////////////////////////
			// SQLite!
			// System.Data.SQLite.SQLiteConnection.CreateFile("d:\\a.db");


			////////////////////////////////////////////////////////////////
			// Start Watch Application


			/**
			 * 
			 * 
			 * 
			 */

			// Initialize the Columns
			//deviceListView.Columns.Add(new EditableColumnHeaderEx("Device", 20));
			//deviceListView.Columns.Add(new ColumnHeaderEx("Version", 120));
			// deviceListView.Columns.Add(new EditableColumnHeaderEx("Genre", excmbx_genre, 60));
			// deviceListView.Columns.Add(new EditableColumnHeaderEx("Rate", excmbx_rate, 100));
			//deviceListView.Columns.Add(new ColumnHeaderEx("Status", 80));
        }

		private void InitSysNotifyIcon()
		{
			// Notify Icon
			sysNotifyIcon.Icon = new Icon(Resources.AppIcon, new Size(16, 16));
			sysNotifyIcon.Visible = true;

			sysNotifyIcon.Click += new EventHandler(OnSysNotifyIconContextMenu);

		}

		private void OnSysNotifyIconContextMenu(object sender, EventArgs e)
		{

		}

		private void StartConnectToDevices()
		{
			// Create N Threads to load the devices assemblies.
		}

		// Menu Entries
		private void fileMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void addDeviceFileMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void aboutMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void docMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void startMenuItem_Click(object sender, EventArgs e)
		{
            Program.DeviceManager.DataReceived = this.OnDataReceived;
            Program.DeviceManager.Run(SynchronizationContext.Current, this.OnDataReceived);
		}

		private void stopMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void startMainVisionMenuItem_Click(object sender, EventArgs e)
		{
			// TODO:
			// Proc-start with args
			IntPtr hWnd =  this.Handle;	// As a arg
		}

		private void closeMainVisionMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void logToolMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void logBankMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void logDelMenuItem_Click(object sender, EventArgs e)
		{

		}


		//////////////////////////////////////////////////////////////////////////
		private void func()
		{
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {

        }

    }
}
