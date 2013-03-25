using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.DAQ.Tools
{
    public partial class MainForm : Form
    {
        private SerialPort serialPort = null;

        private int baudRate = 9600;

        private string com = "COM1";

        public MainForm()
        {
            InitializeComponent();
        }

        private void Open()
        {
            this.com = this.comboBox1.Text;
            this.serialPort = new SerialPort(com);
             
            this.serialPort.BaudRate = this.baudRate;

            this.serialPort.Parity = Parity.None;
            this.serialPort.StopBits = StopBits.One; //(StopBits)this.stopBits;    //StopBits 1
            this.serialPort.DataBits = 8;// this.dataBits;   // DataBits 8bit
            this.serialPort.ReadTimeout = 10000;// this.readTimeout;

            this.serialPort.RtsEnable = true;
            this.serialPort.NewLine = "/r/n";	//?
            this.serialPort.DataReceived += serialPort_DataReceived;

            this.serialPort.Open();

            string s = textBox1.Text;
            s = s.Replace("\\r", "\r");
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            this.serialPort.Write(bytes, 0, bytes.Length);

        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = this.serialPort.BytesToRead;
            byte[] buffer = new byte[n];

            int r = this.serialPort.Read(buffer, 0, n);

            string a = Encoding.ASCII.GetString(buffer);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] ps = SerialPort.GetPortNames();
            foreach (string port in ps)
            {
                comboBox1.Items.Add(port);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Open();
        }


    }
}
