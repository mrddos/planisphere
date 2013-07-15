using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.DataCenterAgent
{
    public partial class AgentWindow : Form
    {
        private Timer timer;

        private Timer keepAliveTimer;
        //private DataHandler handler;

        private List<Agent> agents = new List<Agent>();

        private DataPacketBuilder builder = new DataPacketBuilder();

        public bool StartState
        {
            get;
            set;
        }

        private bool started = false;

        public AgentWindow()
        {
            this.StartState = false;
            InitializeComponent();
        }

        private void AgentWindow_Load(object sender, EventArgs e)
        {
            this.statusStrip1.Items.Add("状态: 等待");
            this.statusStrip1.Items.Add(new ToolStripSeparator());
            this.statusStrip1.Items.Add("IP ADDR:PORT");
            ToolStripLabel label = new ToolStripLabel();
            label.Alignment = ToolStripItemAlignment.Right;
            label.Text = "22:04";
            this.statusStrip1.Items.Add(label);

            // Start if have the --start args.
            if (this.StartState)
            {
                Start();
            }
        }

        private void Start()
        {
            if (this.started)
            {
                return;
            }
            this.InitializeAgents();
            this.InitializeTimer();
            this.started = true;
        }

        private void InitializeAgents()
        {
            Settings s = Settings.Instance;
            foreach (Settings.DataCenter dc in s.DataCenters)
            {
                Agent agent = CreateAgent(dc.Ip, dc.Port, false);
                this.agents.Add(agent);
            }

            this.statusStrip1.Items[0].Text = "状态: 开始";
        }

        private Agent CreateAgent(string serverAddress, int serverPort, bool wireless)
        {
            Agent agent = new Agent();
            agent.ServerAddress = serverAddress;
            agent.ServerPort = serverPort;

            agent.Wireless = wireless;
            agent.Connect();
            agent.OnReceiveMessage += this.OnReceiveMessage;
            return agent;
        }

        private void InitializeTimer()
        {
            this.timer = new Timer();
            this.timer.Interval = 2000;
            this.timer.Tick += this.SendDataTick;
            this.timer.Start();

            // KeepAlive timer
            this.keepAliveTimer = new Timer();
            this.keepAliveTimer.Interval = 1000 * 60;
            this.keepAliveTimer.Tick += this.KeepAliveTick;
            this.keepAliveTimer.Start();
        }

        private void KeepAliveTick(object sender, EventArgs e)
        {
            DataPacket p = builder.GetKeepAlivePacket();
            foreach (var agent in this.agents)
            {
                agent.SendPacket(p);
            }
        }

        private void SendDataTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (!IsRightSendTime(now))
            {
                return;
            }

            DateTime rightTime = GetRightSendTime(now);
            if (rightTime == this.lastSendTime)
            {
                return;
            }
            this.lastSendTime = rightTime;

            SendDataPackets(rightTime);
        }

        public void SendDataPackets(DateTime time)
        {
            bool willSend = false;
            foreach (var agent in this.agents)
            {
                willSend |= agent.Started;
            }

            if (!willSend)
            {
                return;
            }

            foreach (var deviceKey in Settings.Instance.DeviceKeys)
            {
                var d = DBDataSource.Instance.GetData(deviceKey, time);

                DataPacket p = null;
                // By different device.
                
                if (deviceKey.Equals("Scada.HVSampler", StringComparison.OrdinalIgnoreCase) ||
                    deviceKey.Equals("Scada.ISampler", StringComparison.OrdinalIgnoreCase))
                {
                    p = builder.GetFlowDataPacket(deviceKey, d);
                }
                else
                {
                    p = builder.GetDataPacket(deviceKey, d);
                }

                // Sent by each agent.s
                foreach (var agent in this.agents)
                {
                    agent.SendDataPacket(p, time);
                }
            }
        }



        private DateTime lastSendTime;

        private static DateTime GetRightSendTime(DateTime dt)
        {
            int second = dt.Second / 30 * 30;
            DateTime ret = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, second);
            return ret;
        }

        private static bool IsRightSendTime(DateTime dt)
        {
            int sec = dt.Second - 5;
            if ((sec >= 0 && sec <= 10) || ((sec >= 30) && sec <= 40))
            {
                return true;
            }
            return false;
        }


        private void OnReceiveMessage(Agent agent, string msg)
        {
            this.SafeInvoke(() => {
                string line = string.Format("{0}: {1}", agent.ToString(), msg);
                this.listBox1.Items.Add(line);
            });

        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        // 开始
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Start();
        }

        // 暂停
        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
