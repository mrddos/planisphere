using Scada.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Scada.RecordAnalysis
{
    public partial class Form1 : Form
    {
        private ServerPipeConnection serverPipeConn;

        private Thread thread;

        public delegate void MyInvoke(object sender, string str2);

        public Form1()
        {
            InitializeComponent();


            //WorkThreadStart(null);
            thread = new Thread(new ParameterizedThreadStart(this.WorkThreadStart));
            thread.Start(null);
        }

        private void WorkThreadStart(object param)
        {
            serverPipeConn = new ServerPipeConnection("MyPipe", 1024, 1024, 1024);
            
            while (true)
            {
                serverPipeConn.Connect();
                string line = serverPipeConn.Read();
                logListBox.Invoke(new MyInvoke(this.AddString), this, line);
                serverPipeConn.Disconnect();
            }
            
        }

        private void AddString(object sender, string line)
        {
            logListBox.Items.Add(line);
        }


    }
}
