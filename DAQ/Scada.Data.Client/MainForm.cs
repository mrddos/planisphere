﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Scada.Data.Client
{
    public partial class MainForm : Form
    {
        private Thread workThread;

        public MainForm()
        {
            InitializeComponent();
        }


        private void CreateWorkThread()
        {
            this.workThread = new Thread(new ParameterizedThreadStart(this.WorkThread));

            this.workThread.Start(null);
        }

        private void WorkThread(object param)
        {

        }
    }
}
