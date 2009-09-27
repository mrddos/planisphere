namespace Ferry.Debug
{
	partial class DebugForm
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
			this.button1 = new System.Windows.Forms.Button();
			this.ferryList = new System.Windows.Forms.ListView();
			this.colId = new System.Windows.Forms.ColumnHeader();
			this.colDomain = new System.Windows.Forms.ColumnHeader();
			this.colNotify = new System.Windows.Forms.ColumnHeader();
			this.colTarget = new System.Windows.Forms.ColumnHeader();
			this.colMethod = new System.Windows.Forms.ColumnHeader();
			this.lblSize = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 560);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(97, 27);
			this.button1.TabIndex = 1;
			this.button1.Text = "Reload";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Reload_Click);
			// 
			// ferryList
			// 
			this.ferryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colId,
            this.colDomain,
            this.colNotify,
            this.colTarget,
            this.colMethod});
			this.ferryList.Location = new System.Drawing.Point(12, 65);
			this.ferryList.Name = "ferryList";
			this.ferryList.Size = new System.Drawing.Size(956, 429);
			this.ferryList.TabIndex = 2;
			this.ferryList.UseCompatibleStateImageBehavior = false;
			this.ferryList.View = System.Windows.Forms.View.Details;
			this.ferryList.SelectedIndexChanged += new System.EventHandler(this.ferryList_SelectedIndexChanged);
			// 
			// colId
			// 
			this.colId.Text = "ID";
			// 
			// colDomain
			// 
			this.colDomain.Text = "Domain";
			this.colDomain.Width = 120;
			// 
			// colNotify
			// 
			this.colNotify.Text = "Notify";
			this.colNotify.Width = 180;
			// 
			// colTarget
			// 
			this.colTarget.Text = "Target";
			this.colTarget.Width = 180;
			// 
			// colMethod
			// 
			this.colMethod.Text = "Method";
			this.colMethod.Width = 120;
			// 
			// lblSize
			// 
			this.lblSize.AutoSize = true;
			this.lblSize.Location = new System.Drawing.Point(12, 19);
			this.lblSize.Name = "lblSize";
			this.lblSize.Size = new System.Drawing.Size(41, 12);
			this.lblSize.TabIndex = 3;
			this.lblSize.Text = "label1";
			// 
			// DebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(980, 599);
			this.Controls.Add(this.lblSize);
			this.Controls.Add(this.ferryList);
			this.Controls.Add(this.button1);
			this.Name = "DebugForm";
			this.Text = "Debug";
			this.Load += new System.EventHandler(this.DebugForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListView ferryList;
		private System.Windows.Forms.ColumnHeader colId;
		private System.Windows.Forms.ColumnHeader colDomain;
		private System.Windows.Forms.ColumnHeader colNotify;
		private System.Windows.Forms.ColumnHeader colTarget;
		private System.Windows.Forms.ColumnHeader colMethod;
		private System.Windows.Forms.Label lblSize;
	}
}