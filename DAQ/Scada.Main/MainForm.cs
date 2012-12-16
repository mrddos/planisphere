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
			// Notify Icon
			sysNotifyIcon.Icon = new Icon(Resources.AppIcon, new Size(16, 16));
			sysNotifyIcon.Visible = true;

			// SQLite!
			// System.Data.SQLite.SQLiteConnection.CreateFile("d:\\a.db");
			
			// Initialize the Columns
			deviceListView.Columns.Add(new EditableColumnHeaderEx("Movie", 20));
			deviceListView.Columns.Add(new ColumnHeaderEx("Progress", 120));
			// deviceListView.Columns.Add(new EditableColumnHeaderEx("Genre", excmbx_genre, 60));
			// deviceListView.Columns.Add(new EditableColumnHeaderEx("Rate", excmbx_rate, 100));
			deviceListView.Columns.Add(new ColumnHeaderEx("Status", 80));
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
    }
}
