using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Scada.Main.Properties;
using System.Threading;
using System.Diagnostics;
using Scada.Common;
using Scada.Declare;

namespace Scada.Main
{
    public partial class MainForm : Form
    {
		private System.Windows.Forms.Timer timer = null;

        //private Process mainVisionProcess;

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

			// Keep-Alive timer
			this.timer = new System.Windows.Forms.Timer();
			this.timer.Interval = Defines.KeepAliveInterval;
			this.timer.Tick += timerKeepAliveTick;
			this.timer.Start();

            // TODO: Start the selected device by main args;
            // startMenuItem_Click(null, null);

            ////////////////////////////////////////////////////////////////
			// SQLite!
			// System.Data.SQLite.SQLiteConnection.CreateFile("d:\\a.db");
            bool recover = false;
            string[] args = Program.DeviceManager.Args;
            if (args != null && args.Length > 0)
            {
                string options = args[0];
                if (options.Contains("/R"))
                {
                    recover = true;
                }
            }

			////////////////////////////////////////////////////////////////
			// Device List in Group.

            deviceListView.Columns.Add("设备", 280);
			deviceListView.Columns.Add("版本", 80);
			deviceListView.Columns.Add("状态", 100);

            deviceListView.ShowGroups = true;

            foreach (string deviceName in Program.DeviceManager.DeviceNames)
            {
                string deviceKey = deviceName.ToLower();
                string displayName = Program.DeviceManager.GetDeviceDisplayName(deviceKey);
                ListViewGroup g = deviceListView.Groups.Add(deviceKey, displayName);

                List<string> versions = Program.DeviceManager.GetVersions(deviceKey);
                ListViewItem lvi = this.AddDeviceToList(deviceName, versions[0], "Wait");

                g.Items.Add(lvi);
            }

            // TODO:
            // Do Recover.
            if (recover)
            {
                // TODO: Load devices selected from a file, insert them into ListView.
                for (; ; )
                {
                    // Select the devices.
                    Program.DeviceManager.SelectDevice("", "", true);

                    // TODO: List Them into ListView
                }
                //
                this.RunDevices();
            }
            
        }

        private void RunDevices()
        {
            deviceListView.Enabled = false;
            RecordManager.Initialize();
            Program.DeviceManager.DataReceived = this.OnDataReceived;
            Program.DeviceManager.Run(SynchronizationContext.Current, this.OnDataReceived);
        }

        private ListViewItem AddDeviceToList(string deviceName, string version, string status)
        {
            ListViewItem lvi = deviceListView.Items.Add(new ListViewItem(deviceName));
            // Subitems;
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, version));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, status));

            return lvi;
        }

		void timerKeepAliveTick(object sender, EventArgs e)
		{
			Program.SendKeepAlive();
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

		//private void StartConnectToDevices()
		//{
		//	// Create N Threads to load the devices assemblies.
		//}

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
            // TODO: Select the device in the list;
            this.SelectDevices();
            this.RunDevices();
		}

		private void stopMenuItem_Click(object sender, EventArgs e)
		{
			Program.DeviceManager.ShutdownDeviceConnection();
		}

		private void startMainVisionMenuItem_Click(object sender, EventArgs e)
		{
            // Show MainVision
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Scada.MainVision.exe"; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出
                process.Start();

            }
            
		}

		private void logToolMenuItem_Click(object sender, EventArgs e)
		{
			RecordManager.OpenRecordAnalysis();
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

        private void deviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void SelectDevices()
        {
            foreach (ListViewItem item in this.deviceListView.Items)
            {
                if (item.Checked)
                {
                    string deviceName = item.SubItems[0].Text;
                    string version = item.SubItems[1].Text;
                    Program.DeviceManager.SelectDevice(deviceName, version, true);
                }
            }
        }

        private void deviceListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            bool itemChecked = e.Item.Checked;
            if (itemChecked)
            {
                // TODO: If there are multi-items, only one can be selected.    
            }
            
        }

    }
}
