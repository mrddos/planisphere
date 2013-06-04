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

        private Agent agent;

        private DataPacketBuilder builder = new DataPacketBuilder();


        public AgentWindow()
        {
            InitializeComponent();
        }

        private void AgentWindow_Load(object sender, EventArgs e)
        {
            this.agent = this.CreateAgent("localhost", 2112);
            Settings s = Settings.Instance;
            this.InitializeTimer();
        }

        private Agent CreateAgent(string serverAddress, int serverPort)
        {
            Agent agent = new Agent();
            agent.ServerAddress = serverAddress;
            agent.ServerPort = serverPort;
            agent.Greeting = "Hello";
            agent.Connect();

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


            this.SendPackets(rightTime);
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


        private void SendPackets(DateTime time)
        {
            // For test
            // var d = DBDataSource.Instance.GetData("scada.hipc", time);

            List<Agent> agents = new List<Agent>() { this.agent };

            foreach (var deviceKey in Settings.Instance.DeviceKeys)
            {
                var d = DBDataSource.Instance.GetData(deviceKey, time);

                DataPacket p = builder.GetDataPacket(deviceKey, d);
                string ps = p.ToString();
                foreach (var agent in agents)
                {
                    byte[] bytes = p.ToBytes();
                    agent.Send(bytes);
                }
            }

            // TODO: Agents

        }
    }
}
