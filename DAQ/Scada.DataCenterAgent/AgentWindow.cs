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


        public AgentWindow()
        {
            InitializeComponent();
        }

        private void AgentWindow_Load(object sender, EventArgs e)
        {
            this.InitializeAgents();
            this.InitializeTimer();
        }

        private void InitializeAgents()
        {
            Settings s = Settings.Instance;
            foreach (Settings.DataCenter dc in s.DataCenters)
            {
                Agent agent = CreateAgent(dc.Ip, dc.Port, false);
                this.agents.Add(agent);
            }

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

                DataPacket p = builder.GetDataPacket(deviceKey, d);
                // string ps = p.ToString();
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

        }
    }
}
