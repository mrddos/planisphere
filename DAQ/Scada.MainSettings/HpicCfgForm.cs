using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Scada.Config;

namespace Scada.MainSettings
{
    public partial class HpicCfgForm : SettingFormBase, IApply
    {
        // Device Key
        const string TheDeviceKey = "scada.hpic";

        private HpicSettings settings = new HpicSettings();

        private string serialPort = "COM1";

        // private string factor1;

        public HpicCfgForm()
        {
            InitializeComponent();
        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void HpicCfgForm_Load(object sender, EventArgs e)
        {
            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            DeviceEntry entry = DeviceEntry.GetDeviceEntry(TheDeviceKey, filePath);

            this.Loaded(this.settings);
        }






        public void Apply()
        {
            // this.serialPort = this.comboBoxPort.Text;
            // this.factor1 = this.textBoxFactor.Text;


            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            using (ScadaWriter sw = new ScadaWriter(filePath))
            {
                sw.WriteLine(DeviceEntry.SerialPort, this.serialPort);
                // sw.WriteLine("factor1", this.factor1);

                sw.Commit();
            }

        }

        public void Cancel()
        {

        }

        private void propertyGrid_Click(object sender, EventArgs e)
        {

        }
    }

}
