using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Scada.Config;

namespace Scada.MainSettings
{
    public partial class NaICfgForm : SettingFormBase, IApply
    {
        private const string TheDeviceKey = "scada.naidevice";

        public NaICfgForm()
        {
            InitializeComponent();
        }

        public void Apply()
        {
            //throw new NotImplementedException();
        }

        public void Cancel()
        {
            //throw new NotImplementedException();
        }

        private NaISettings settings = new NaISettings();

        private void NaICfgForm_Load(object sender, EventArgs e)
        {
            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            DeviceEntry entry = DeviceEntry.ReadConfigFile(TheDeviceKey, filePath);

            this.Loaded(this.settings);
        }
    }
}
