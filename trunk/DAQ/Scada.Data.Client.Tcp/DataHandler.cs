﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Scada.DataCenterAgent
{
    // Command for received.
    public enum ReceivedCommand
    {
        Unknown = -1,
        None = 0,
        SetPassword = 1072,
        GetTime = 1011,
        SetTime = 1012,
        StartSendData =  2011,
        StopSendData = 2012,

        HistoryData = 2042,

        StartSendDataDirectly = 3101,
        StopSendDataDirectly = 3102,
        Init = 6021,
        KeepAlive = 6031,
        StartDev = 3012,
        StopDev = 3015,
     
        Reply = 9012
    }

    public enum SentCommand
    {
        None = 0,
        GetTime = 1011,
        Data = 2011,
        FlowData = 2014,
        HistoryData = 2042,
        Auth = 6011,
        KeepAlive = 6031,
        Reply = 9011,
        Result = 9012,
        Notify = 9013,

    }

    [StructLayout(LayoutKind.Sequential)]
    public class SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort whour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    /// <summary>
    /// DataHandler
    /// </summary>
    class MessageDataHandler
    {
        private DataPacketBuilder builder = new DataPacketBuilder();

        // Agent ref.
        private Agent agent;

        private SamplerController hvsc = new SamplerController("scada.hvsampler");

        private SamplerController isc = new SamplerController("scada.isampler");

        // Win32 API
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(SystemTime st);

        [DllImport("Kernel32.dll")]
        public static extern void SetLocalTime(SystemTime st);

        private SentCommand CurrentSentCommand
        {
            get;
            set;
        }

        public MessageDataHandler(Agent agent)
        {
            this.agent = agent;
        }


        public void SendAuthPacket()
        {
            // TODO:QN=20090516010101001;ST=38;CN=6011;PW=123456;
            // MN=0101A010000000;Flag=1;CP=&&&&
            var p = this.builder.GetAuthPacket();
            this.agent.SendPacket(p);
            this.CurrentSentCommand = SentCommand.Auth;
        }

        public void SendKeepAlivePacket()
        {
            // QN=20090516010101001;ST=38;CN=6031;PW=123456;
            // MN=0101A010000000;CP=&&&&
            var p = this.builder.GetKeepAlivePacket();
            this.agent.SendPacket(p);
        }

        public void SendReplyPacket(string qn)
        {
            // QN=20090516010101001;ST=38;CN=6031;PW=123456;
            // MN=0101A010000000;CP=&&&&
            var p = this.builder.GetReplyPacket(qn);
            this.agent.SendPacket(p, default(DateTime));
        }

        private void SendResultPacket(string qn)
        {
            var p = this.builder.GetResultPacket(qn);
            this.agent.SendPacket(p, default(DateTime));
        }

        private void SendNotifyPacket(string qn)
        {
            var p = this.builder.GetNotifyPacket(qn);
            this.agent.SendPacket(p, default(DateTime));
        }
        

        public void OnMessage(string msg)
        {
            ReceivedCommand code = (ReceivedCommand)ParseCommandCode(msg);

            switch (code)
            {
                // 设置系统时间
                case ReceivedCommand.SetTime:
                    {
                        this.OnSetTime(msg);
                    }
                    break;
                // 获得系统时间
                case ReceivedCommand.GetTime:
                    {
                        this.OnGetTime(msg);
                    }
                    break;
                // 设置密码
                case ReceivedCommand.SetPassword:
                    {
                        this.HandleSetPassword(msg);
                    }
                    break;
                // 开始
                case ReceivedCommand.StartSendData:
                    {
                        this.OnStartSendData(msg);
                    }
                    break;
                // 结束
                case ReceivedCommand.StopSendData:
                    {
                        this.OnStopSendData(msg);
                    }
                    break;
                // 历史数据
                case ReceivedCommand.HistoryData:
                    {
                        this.agent.OnHistoryData = true;
                        this.HandleHistoryData(msg);
                        this.agent.OnHistoryData = false;
                    }
                    break;
                // 直接数据
                case ReceivedCommand.StartSendDataDirectly:
                    {
                        Debug.Assert(this.agent.Type != Type.Country);
                        this.OnStartSendDataDirectly(msg);
                    }
                    break;
                // 停止直接数据
                case ReceivedCommand.StopSendDataDirectly:
                    {
                        Debug.Assert(this.agent.Type != Type.Country);
                        this.OnStopSendDataDirectly(msg);
                    }
                    break;
                // 初始化
                case ReceivedCommand.Init:
                    {
                        this.OnInitializeRequest(msg);
                    }
                    break;
                // 心跳包
                case ReceivedCommand.KeepAlive:
                    {
                        this.OnKeepAlive(msg);
                    }
                    break;
                // 启动设备
                case ReceivedCommand.StartDev:
                    {
                        this.OnStartDevice(msg);
                        hvsc.Start();
                    }
                    break;
                // 停止设备
                case ReceivedCommand.StopDev:
                    {
                        this.OnStopDevice(msg);
                    }
                    break;
                // Server Reply
                case ReceivedCommand.Reply:
                    {
                        this.OnServerReply(msg);
                    }
                    break;
                // Error!
                case ReceivedCommand.None:
                case ReceivedCommand.Unknown:
                default:
                    break;
            }
        }

        private void OnKeepAlive(string msg)
        {
            // TODO: Handle Timeout, but NO doc details talk about this.
        }

        private void OnGetTime(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);

            var p = this.builder.GetTimePacket(qn);
            this.agent.SendPacket(p, default(DateTime));
            this.SendResultPacket(qn);
        }

        private void OnSetTime(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);

            string time = Value.Parse(msg, "SystemTime");
            DateTime dt = DeviceTime.Parse(time);
            SystemTime st = new SystemTime();

            st.wYear = (ushort)dt.Year;
            st.wMonth = (ushort)dt.Month;
            st.wDay = (ushort)dt.Day;
            st.whour = (ushort)dt.Hour;
            st.wMinute = (ushort)dt.Minute;
            st.wSecond = (ushort)dt.Second;
            st.wMilliseconds = 0;

            SetLocalTime(st);
            this.SendResultPacket(qn);
        }

        private void OnServerReply(string msg)
        {
            // TODO:
            string ret = Value.Parse(msg, "ExeRtn");
            if (this.CurrentSentCommand == SentCommand.Auth)
            {

            }
            else if (this.CurrentSentCommand == SentCommand.Data)
            {

            }
            else if (this.CurrentSentCommand == SentCommand.HistoryData)
            {

            }

            // After reply, reset the Current Sent Command
            this.CurrentSentCommand = SentCommand.None;
        }

        private void HandleHistoryData(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);

            string sno = Value.Parse(msg, "SNO");

            string eno = Value.Parse(msg, "ENO");

            string beginTime = Value.Parse(msg, "BeginTime");
            string endTime = Value.Parse(msg, "EndTime");

            string polId = Value.Parse(msg, "PolId");

            this.UploadHistoryData(qn, eno, beginTime, endTime, polId);

            this.SendResultPacket(qn);
        }

        private void UploadHistoryData(string qn, string eno, string beginTime, string endTime, string polId)
        {
            DateTime f = DeviceTime.Parse(beginTime);
            DateTime t = DeviceTime.Parse(endTime);
            if (f >= t)
            {
                return;
            }
            string deviceKey = Settings.Instance.GetDeviceKeyByEno(eno);
            string deviceKeyLower = deviceKey.ToLower();
            DateTime dt = f;
            while (dt <= t)
            {
                if (deviceKey.Equals("Scada.NaIDevice", StringComparison.OrdinalIgnoreCase))
                {
                    // NaIDevice ... Gose here.
                    // 分包
                    string content = DBDataSource.Instance.GetNaIDeviceData(dt);
                    if (!string.IsNullOrEmpty(content))
                    {
                        List<DataPacket> pks = builder.GetDataPackets(deviceKey, dt, content, true);
                        foreach (var p in pks)
                        {
                            Thread.Sleep(10);
                            this.agent.SendDataPacket(p, dt);
                        }
                    }
                    dt = dt.AddSeconds(60 * 5);
                }
                else
                {
                    // Non NaIDevice
                    var d = DBDataSource.Instance.GetData(deviceKey, dt);

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

                    this.agent.SendDataPacket(p, dt);
                    dt = dt.AddSeconds(30);
                }
            }

        }

        private void OnInitializeRequest(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);

        }

        private void OnStartSendData(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);
            this.agent.Started = true;
        }

        private void OnStopSendData(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendNotifyPacket(qn);

            this.agent.Started = false;
        }

        private void OnStartDevice(string msg)
        {
            string eno = Value.Parse(msg, "ENO");
            string deviceKey = Settings.Instance.GetDeviceKeyByEno(eno);
            if (deviceKey.ToLower() == "Scada.HVSampler".ToLower())
            {
                hvsc.Start();
            }
            else if (deviceKey.ToLower() == "Scada.ISampler".ToLower())
            {
                isc.Start();
            }
        }

        private void OnStopDevice(string msg)
        {
            string eno = Value.Parse(msg, "ENO");
            string deviceKey = Settings.Instance.GetDeviceKeyByEno(eno);
            if (deviceKey.ToLower() == "Scada.HVSampler".ToLower())
            {
                hvsc.Stop();
            }
            else if (deviceKey.ToLower() == "Scada.ISampler".ToLower())
            {
                isc.Stop();
            }
        }

        private void OnStartSendDataDirectly(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);
            this.StartConnectCountryCenter();
            this.agent.StartedForDirect = true;
        }

        private void OnStopSendDataDirectly(string msg)
        {
            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);
            this.StopConnectCountryCenter();
        }

        private static int ParseCommandCode(string msg)
        {
            int code = 0;
            if (int.TryParse(Value.Parse(msg, "CN"), out code))
            {
                return code;
            }
            return 0;
        }

        private void HandleSetPassword(string msg)
        {
            Settings.Instance.Password = Value.Parse(msg, "PW");

            string qn = Value.Parse(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);
        }


        // 开始向国家数据中心发送数据
        private void StartConnectCountryCenter()
        {
            this.agent.StartConnectCountryCenter();
            this.agent.StartedForDirect = true;
        }

        // 停止向国家数据中心发送数据
        private void StopConnectCountryCenter()
        {
            this.agent.StartedForDirect = false;
            this.agent.StopConnectCountryCenter();
        }

        
    }
}
