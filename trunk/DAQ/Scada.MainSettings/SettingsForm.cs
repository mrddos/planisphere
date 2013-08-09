using Scada.MainSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Main
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SettingsForm : Form
    {
        // private SettingsContext context;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.tabPage1.Controls.Add(new HpicCfgForm());
        }




    }
}
