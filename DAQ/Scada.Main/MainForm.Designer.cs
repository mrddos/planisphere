namespace Scada.Main
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sysNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDeviceFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMainVisionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMainVisionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataCenterSetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logBankMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logDelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.docMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.deviceListView = new Scada.Main.DeviceListView();
            this.groupBox1.SuspendLayout();
            this.menuBar.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.deviceListView);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(30, 33, 30, 33);
            this.groupBox1.Size = new System.Drawing.Size(629, 332);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // sysNotifyIcon
            // 
            this.sysNotifyIcon.Text = "notifyIcon1";
            this.sysNotifyIcon.Visible = true;
            // 
            // menuBar
            // 
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.operateMenuItem,
            this.dataMenuItem,
            this.logMenuItem,
            this.helpMenuItem});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(660, 24);
            this.menuBar.TabIndex = 2;
            this.menuBar.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDeviceFileMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(45, 20);
            this.fileMenuItem.Text = "文件";
            this.fileMenuItem.Click += new System.EventHandler(this.fileMenuItem_Click);
            // 
            // addDeviceFileMenuItem
            // 
            this.addDeviceFileMenuItem.Name = "addDeviceFileMenuItem";
            this.addDeviceFileMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addDeviceFileMenuItem.Text = "添加设备文件";
            this.addDeviceFileMenuItem.Click += new System.EventHandler(this.addDeviceFileMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMenuItem.Text = "退出";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // operateMenuItem
            // 
            this.operateMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startMenuItem,
            this.stopMenuItem});
            this.operateMenuItem.Name = "operateMenuItem";
            this.operateMenuItem.Size = new System.Drawing.Size(45, 20);
            this.operateMenuItem.Text = "操作";
            // 
            // startMenuItem
            // 
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(100, 22);
            this.startMenuItem.Text = "启动";
            this.startMenuItem.Click += new System.EventHandler(this.startMenuItem_Click);
            // 
            // stopMenuItem
            // 
            this.stopMenuItem.Name = "stopMenuItem";
            this.stopMenuItem.Size = new System.Drawing.Size(100, 22);
            this.stopMenuItem.Text = "停止";
            this.stopMenuItem.Click += new System.EventHandler(this.stopMenuItem_Click);
            // 
            // dataMenuItem
            // 
            this.dataMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startMainVisionMenuItem,
            this.closeMainVisionMenuItem,
            this.dataCenterSetMenuItem});
            this.dataMenuItem.Name = "dataMenuItem";
            this.dataMenuItem.Size = new System.Drawing.Size(45, 20);
            this.dataMenuItem.Text = "数据";
            // 
            // startMainVisionMenuItem
            // 
            this.startMainVisionMenuItem.Name = "startMainVisionMenuItem";
            this.startMainVisionMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startMainVisionMenuItem.Text = "启动数据视图";
            this.startMainVisionMenuItem.Click += new System.EventHandler(this.startMainVisionMenuItem_Click);
            // 
            // closeMainVisionMenuItem
            // 
            this.closeMainVisionMenuItem.Name = "closeMainVisionMenuItem";
            this.closeMainVisionMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeMainVisionMenuItem.Text = "关闭数据视图";
            this.closeMainVisionMenuItem.Click += new System.EventHandler(this.closeMainVisionMenuItem_Click);
            // 
            // dataCenterSetMenuItem
            // 
            this.dataCenterSetMenuItem.Name = "dataCenterSetMenuItem";
            this.dataCenterSetMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dataCenterSetMenuItem.Text = "数据中心设置";
            // 
            // logMenuItem
            // 
            this.logMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolMenuItem,
            this.logBankMenuItem,
            this.logDelMenuItem});
            this.logMenuItem.Name = "logMenuItem";
            this.logMenuItem.Size = new System.Drawing.Size(45, 20);
            this.logMenuItem.Text = "日志";
            // 
            // logToolMenuItem
            // 
            this.logToolMenuItem.Name = "logToolMenuItem";
            this.logToolMenuItem.Size = new System.Drawing.Size(152, 22);
            this.logToolMenuItem.Text = "日志分析工具";
            this.logToolMenuItem.Click += new System.EventHandler(this.logToolMenuItem_Click);
            // 
            // logBankMenuItem
            // 
            this.logBankMenuItem.Name = "logBankMenuItem";
            this.logBankMenuItem.Size = new System.Drawing.Size(152, 22);
            this.logBankMenuItem.Text = "备份日志";
            this.logBankMenuItem.Click += new System.EventHandler(this.logBankMenuItem_Click);
            // 
            // logDelMenuItem
            // 
            this.logDelMenuItem.Name = "logDelMenuItem";
            this.logDelMenuItem.Size = new System.Drawing.Size(152, 22);
            this.logDelMenuItem.Text = "删除日志";
            this.logDelMenuItem.Click += new System.EventHandler(this.logDelMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.docMenuItem,
            this.toolStripSeparator2,
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(45, 20);
            this.helpMenuItem.Text = "帮助";
            // 
            // docMenuItem
            // 
            this.docMenuItem.Name = "docMenuItem";
            this.docMenuItem.Size = new System.Drawing.Size(100, 22);
            this.docMenuItem.Text = "文档";
            this.docMenuItem.Click += new System.EventHandler(this.docMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(97, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(100, 22);
            this.aboutMenuItem.Text = "关于";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolBar.Location = new System.Drawing.Point(0, 24);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(660, 25);
            this.toolBar.TabIndex = 3;
            this.toolBar.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 484);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(660, 22);
            this.statusBar.TabIndex = 4;
            this.statusBar.Text = "statusStrip1";
            // 
            // deviceListView
            // 
            this.deviceListView.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.deviceListView.ControlPadding = 4;
            this.deviceListView.FullRowSelect = true;
            this.deviceListView.Location = new System.Drawing.Point(68, 68);
            this.deviceListView.Margin = new System.Windows.Forms.Padding(20, 22, 20, 22);
            this.deviceListView.Name = "deviceListView";
            this.deviceListView.OwnerDraw = true;
            this.deviceListView.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.deviceListView.RightToLeftLayout = true;
            this.deviceListView.Size = new System.Drawing.Size(511, 196);
            this.deviceListView.TabIndex = 0;
            this.deviceListView.UseCompatibleStateImageBehavior = false;
            this.deviceListView.View = System.Windows.Forms.View.Details;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 506);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuBar);
            this.MainMenuStrip = this.menuBar;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.groupBox1.ResumeLayout(false);
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NotifyIcon sysNotifyIcon;
		private System.Windows.Forms.MenuStrip menuBar;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addDeviceFileMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.ToolStripMenuItem operateMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dataMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startMainVisionMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeMainVisionMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logToolMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logBankMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logDelMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem docMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
		private DeviceListView deviceListView;
		private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
		private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
		private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
		private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
		private System.Windows.Forms.ToolStripContentPanel ContentPanel;
		private System.Windows.Forms.ToolStrip toolBar;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripMenuItem dataCenterSetMenuItem;
    }
}

