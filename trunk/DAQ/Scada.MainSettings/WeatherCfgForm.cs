using System;
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
    public partial class WeatherCfgForm : SettingFormBase, IApply
    {
        const string TheDeviceKey = "scada.weather";


        private string serialPort = "COM1";

        public WeatherCfgForm()
        {
            InitializeComponent();
        }

        public void Apply()
        {
            // this.serialPort = this.comboBoxPort.Text;

            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            using (ScadaWriter sw = new ScadaWriter(filePath))
            {
                sw.WriteLine(DeviceEntry.SerialPort, this.serialPort);

                sw.Commit();
            }
        }

        public void Cancel()
        {
            // this.comboBoxPort.Text = this.serialPort;
        }

        private WeatherSettings settings = new WeatherSettings();

        private void WeatherCfgForm_Load(object sender, EventArgs e)
        {
            string filePath = Program.GetDeviceConfigFile(TheDeviceKey);
            DeviceEntry entry = DeviceEntry.GetDeviceEntry(TheDeviceKey, filePath);

            this.Loaded(this.settings);

        }
    }
}
