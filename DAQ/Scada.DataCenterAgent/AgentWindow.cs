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

        public AgentWindow()
        {
            InitializeComponent();
        }

        private void AgentWindow_Load(object sender, EventArgs e)
        {
            this.agent = this.CreateAgent("localhost", 2112);
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
            this.timer.Interval = 3000;
            this.timer.Start();
            this.timer.Tick += this.SendDataTick;
        }

        private void SendDataTick(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.ASCII.GetBytes("Fucking\n");
            this.agent.Send(bytes);
        }
    }
}
