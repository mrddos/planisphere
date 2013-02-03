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

        private string terminateString;

        public Form1()
        {
            InitializeComponent();


            thread = new Thread(new ParameterizedThreadStart(this.WorkThreadStart));
            thread.Start(null);
        }


        private void WorkThreadStart(object param)
        {
            serverPipeConn = new ServerPipeConnection(Defines.LocalPipeName, 1024, 1024, 1024);

            this.terminateString = string.Format("make_thread_{0}_closed", Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                serverPipeConn.Connect();
                string line = serverPipeConn.Read();
                if (line == this.terminateString)
                {
                    break;
                }
                logListBox.Invoke(new MyInvoke(this.AddString), this, line);
                serverPipeConn.Disconnect();
            }
            
        }

        private void AddString(object sender, string line)
        {
            logListBox.Items.Add(line);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ClosePipeServer(this.terminateString);
        }

        private void ClosePipeServer(string terminateString)
        {
            IInterProcessConnection clientConnection = null;
            try
            {
                clientConnection = new ClientPipeConnection(Defines.LocalPipeName, ".");
                clientConnection.Connect();
                clientConnection.Write(terminateString);
                clientConnection.Close();
            }
            catch (Exception)
            {
                clientConnection.Dispose();
            }
        }

    }
}
