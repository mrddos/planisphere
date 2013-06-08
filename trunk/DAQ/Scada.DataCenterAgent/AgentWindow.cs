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

        private DataHandler handler;

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

            }

            this.handler = new DataHandler();
            this.handler.Agents = this.agents;
        }

        private Agent CreateAgent(string serverAddress, int serverPort, bool wireless)
        {
            Agent agent = new Agent();
            agent.ServerAddress = serverAddress;
            agent.ServerPort = serverPort;
            agent.Greeting = "Hello";
            agent.Wireless = wireless;
            agent.Connect();
            agent.OnReceiveMessage += this.OnReceiveMessage;
            return agent;
        }

        private void InitializeTimer()
        {
            this.timer = new Timer();
            this.timer.Interval = 2000;
            this.timer.Start();
            this.timer.Tick += this.SendDataTick;
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

            if (this.handler != null)
            {
                this.handler.SendDataPackets(rightTime);
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
            if (this.handler != null)
            {
                this.handler.OnMessage(msg);
            }
        }
    }
}
