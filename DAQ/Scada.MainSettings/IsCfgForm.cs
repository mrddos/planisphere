using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.MainSettings
{
    public partial class IsCfgForm : SettingFormBase, IApply
    {
        public IsCfgForm()
        {
            InitializeComponent();
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private AisSettings settings = new AisSettings();

        private void IsCfgForm_Load(object sender, EventArgs e)
        {
            this.Loaded(this.settings);
        }
    }
}
