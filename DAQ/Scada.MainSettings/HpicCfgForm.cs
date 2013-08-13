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
    public partial class HpicCfgForm : UserControl, IApply
    {
        const string TheDeviceKey = "scada.hpic";


        private string serialPort = "COM1";

        private string factor1;

        public HpicCfgForm()
        {
            InitializeComponent();
        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void HpicCfgForm_Load(object sender, EventArgs e)
        {
            // 
            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            DeviceEntry entry = DeviceEntry.ReadConfigFile(TheDeviceKey, filePath);


            string[] ports = SerialPort.GetPortNames();
            
            foreach (string port in ports)
            {
                string portName = port.ToUpper();
                this.comboBoxPort.Items.Add(portName);
            }


            this.serialPort = (StringValue)entry[DeviceEntry.SerialPort];
            if (!string.IsNullOrEmpty(this.serialPort))
            {
                this.comboBoxPort.Text = this.serialPort;
            }

            this.factor1 = (StringValue)entry["factor1"];
            this.textBoxFactor.Text = this.factor1;

        }



        public void Apply()
        {
            this.serialPort = this.comboBoxPort.Text;
            this.factor1 = this.textBoxFactor.Text;


            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            using (ScadaWriter sw = new ScadaWriter(filePath))
            {
                sw.WriteLine(DeviceEntry.SerialPort, this.serialPort);
                sw.WriteLine("factor1", this.factor1);

                sw.Commit();
            }

        }

        public void Cancel()
        {
            this.comboBoxPort.Text = this.serialPort;
            this.textBoxFactor.Text = this.factor1;
        }
    }
}
