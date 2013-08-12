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
    public partial class HpicCfgForm : UserControl
    {
        const string TheDeviceKey = "scada.hpic";

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
            DeviceEntry entry = DeviceEntry.ReadConfigFile(TheDeviceKey, filePath);


            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                string portName = port.ToUpper();
                this.comboBoxPort.Items.Add(portName);
            }

            this.comboBoxPort.Text = (StringValue)entry[DeviceEntry.SerialPort];

            this.textBoxFactor.Text = (StringValue)entry["factor1"];
        }
    }
}
