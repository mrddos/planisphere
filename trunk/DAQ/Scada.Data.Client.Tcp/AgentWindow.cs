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

        private Dictionary<string, DateTime> lastDeviceSendData = new Dictionary<string, DateTime>();

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
                agent.AddWirelessInfo(dc.WirelessIp, dc.WirelessPort);
                this.agents.Add(agent);
            }

            this.statusStrip1.Items[0].Text = "状态: 开始";
        }

        // 先连接有线的线路
        private Agent CreateAgent(string serverAddress, int serverPort, bool wireless)
        {
            Agent agent = new Agent(serverAddress, serverPort);

            agent.Wireless = wireless;
            agent.Connect();
            agent.OnReceiveMessage += this.OnReceiveMessage;
            agent.OnNotifyEvent += this.OnNotifyEvent;
            return agent;
        }

        private void InitializeTimer()
        {
            this.timer = new Timer();
            this.timer.Interval = 4000;
            this.timer.Tick += this.SendDataTick;
            this.timer.Start();

            // KeepAlive timer
            this.keepAliveTimer = new Timer();
            this.keepAliveTimer.Interval = 1000 * 5;
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

            foreach (var deviceKey in Settings.Instance.DeviceKeys)
            {
                if (IsDeviceSendTimeOK(now, deviceKey))
                {
                    DateTime sendTime = GetDeviceSendTime(now, deviceKey);

                    if (!this.lastDeviceSendData.ContainsKey(deviceKey))
                    {
                        this.lastDeviceSendData[deviceKey] = default(DateTime);
                    }

                    if (sendTime == this.lastDeviceSendData[deviceKey])
                    {
                        return;
                    }

                    this.lastDeviceSendData[deviceKey] = sendTime;

                    SendDataPackets(sendTime, deviceKey);
                }
            }
        }

        public void SendDataPackets(DateTime time, string deviceKey)
        {
            bool willSend = false;
            foreach (var agent in this.agents)
            {
                willSend |= agent.Started;
            }

            if (willSend) //// TODO: !
            {
                return;
            }

            if (deviceKey.Equals("Scada.NaIDevice", StringComparison.OrdinalIgnoreCase))
            {
                // 分包
                string content = DBDataSource.Instance.GetNaIDeviceData(time);

                List<DataPacket> pks = builder.GetDataPackets(deviceKey, time, content);
                foreach (var p in pks)
                {
                    // Sent by each agent.s
                    foreach (var agent in this.agents)
                    {
                        agent.SendDataPacket(p, time);
                    }
                }
            }
            else
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

        private static DateTime GetDeviceSendTime(DateTime dt, string deviceKey)
        {
            if (deviceKey.Equals("Scada.NaIDevice", StringComparison.OrdinalIgnoreCase))
            {
                int min = dt.Minute - 1;
                DateTime ret = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, min, 0);
                return ret;
            }
            else
            {
                int second = dt.Second / 30 * 30;
                DateTime ret = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, second);
                return ret;
            }
        }

        private static bool IsDeviceSendTimeOK(DateTime dt, string deviceKey)
        {
            if (deviceKey.Equals("Scada.NaIDevice", StringComparison.OrdinalIgnoreCase))
            {
                // 00, 05, 10, ...55,
                // Send data after 1 min.
                if ((dt.Minute - 1) % 5 == 0)
                {
                    return true;
                }
            }
            else
            {
                // 5 < current.second < 15 OR
                // 35 < current.second < 45
                int sec = dt.Second - 5;
                if ((sec >= 0 && sec <= 10) || ((sec >= 30) && sec <= 40))
                {
                    return true;
                }
            }
            return false;
        }


        private void OnReceiveMessage(Agent agent, string msg)
        {
            this.SafeInvoke(() => {
                string line = string.Format("{0}: {1}", agent.ToString(false), msg);
                this.listBox1.Items.Add(line);
            });

        }

        private void OnNotifyEvent(Agent agent, NotifyEvent ne, string msg)
        {
            this.SafeInvoke(() =>
            {
                if (NotifyEvent.Connected == ne)
                {
                    this.statusStrip1.Items[1].Text = agent.ToString() + " 已连接";
                }
                else if (NotifyEvent.ConnectError == ne)
                {
                    this.listBox1.Items.Add(msg);
                }
            });
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
