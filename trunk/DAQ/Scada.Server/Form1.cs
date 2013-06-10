﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Scada.Server
{
    public partial class Form1 : Form
    {
        private Thread thread = null;

        private TcpListener tcpListener;

        private delegate void InvokeCallback(string msg);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.thread = new Thread(new ParameterizedThreadStart(this.WorkThread));
            this.thread.Start();
        }

        private NetworkStream networkStream = null;


        private void WorkThread(object state)
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 6000);

            this.tcpListener.Start();

            TcpClient tcpClient = this.tcpListener.AcceptTcpClient();

            using (NetworkStream ns = tcpClient.GetStream())
            {
                this.networkStream = ns;
                StreamReader sr = new StreamReader(ns);

                while (true)
                {
                    string result = sr.ReadLine();
                    Debug.WriteLine(result);

                    this.Invoke(new InvokeCallback(this.OnReceived), result);


                    string received = "Server Received\n";
                    var b = Encoding.ASCII.GetBytes(received);
                    ns.Write(b, 0, b.Length);
                }
            }
            // TODO:
            tcpListener.Stop();
        }

        private void OnReceived(string msg)
        {
            this.textBox1.Text += msg;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.thread != null)
            {
                this.tcpListener.Stop();
                this.thread.Abort();
                
            }
        }

        private void Send(string msg)
        {
            var b = Encoding.ASCII.GetBytes(msg);
            this.networkStream.Write(b, 0, b.Length);
        }

        // Test connection
        private void button1_Click(object sender, EventArgs e)
        {
            string received = "Connection testing from server\n";
            this.Send(received);
        }

        private bool hvsStarted = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (hvsStarted)
            {
                hvsStarted = false;
                this.button2.Text = "超大流量气溶胶采样器 - 启动";
            }
            else
            {
                hvsStarted = true;
                this.button2.Text = "超大流量气溶胶采样器 - 停止";
            }
        }

        /// <summary>
        /// 碘 i sampler
        /// </summary>
        private bool isStarted = false;
        private void button3_Click(object sender, EventArgs e)
        {
            if (isStarted)
            {
                isStarted = false;
                this.button3.Text = "碘采样器 - 启动";
            }
            else
            {
                isStarted = true;
                this.button3.Text = "碘采样器 - 停止";
            }
        }

        private bool uploading = false;
        private void button4_Click(object sender, EventArgs e)
        {
            if (uploading)
            {
                uploading = false;
                this.button4.Text = "开始 传输监测项实时数据";

                string msg = "QN=20090516010101001;ST=38;CN=2011;PW=123456;MN=80110010000000;Flag=3;CP=&&&&";
                this.Send(msg);

            }
            else
            {
                uploading = true;
                this.button4.Text = "停止 传输监测项实时数据";

                string msg = "QN=20090516010101001;ST=38;CN=2011;PW=123456;MN=80110010000000;Flag=3;CP=&&&&";
                this.Send(msg);

            }
        }
    }
}
