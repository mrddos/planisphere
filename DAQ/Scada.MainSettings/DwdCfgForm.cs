﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Scada.Config;
using System.IO.Ports;

namespace Scada.MainSettings
{
    public partial class DwdCfgForm : SettingFormBase, IApply
    {
        const string TheDeviceKey = "scada.dwd";

        private DwdSettings settings = new DwdSettings();

        public DwdCfgForm()
        {
            InitializeComponent();
        }

        public void Apply()
        {


            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            using (ScadaWriter sw = new ScadaWriter(filePath))
            {
                // sw.WriteLine(DeviceEntry.SerialPort, this.serialPort);

                sw.Commit();
            }

        }

        public void Cancel()
        {
            // this.comboBoxPort.Text = this.serialPort;
        }

        private void DwdCfgForm_Load(object sender, EventArgs e)
        {
            // 
            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            DeviceEntry entry = DeviceEntry.ReadConfigFile(TheDeviceKey, filePath);




            this.Loaded(this.settings);
        }
    }
}
