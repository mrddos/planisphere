using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Scada.DataCenterAgent
{
    // Command for received.
    enum ReceivedCommand
    {
        Unknown = -1,
        None = 0,
        SetPassword = 1072,
        GetTime = 1011,
        SetTime = 1012,
        StartSend =  2011,
        StopSend = 2012,

        HistoryData = 2042,

        DirectData = 3101,
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

    class DataHandler
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

        public DataHandler(Agent agent)
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
        

        public void OnMessage(string msg)
        {
            ReceivedCommand code = (ReceivedCommand)ParseCommandCode(msg);

            switch (code)
            {
                case ReceivedCommand.SetTime:
                    {
                        this.OnSetTime(msg);
                    }
                    break;
                case ReceivedCommand.GetTime:
                    {
                        this.OnGetTime(msg);
                    }
                    break;
                case ReceivedCommand.SetPassword:
                    {
                        this.HandleSetPassword(msg);
                    }
                    break;
                case ReceivedCommand.StartSend:
                    {
                        this.OnDataRequest(msg);
                    }
                    break;
                case ReceivedCommand.StopSend:
                    {
                        this.agent.Started = false;
                    }
                    break;
                case ReceivedCommand.HistoryData:
                    {
                        this.agent.History = true;
                        this.HandleHistoryData(msg);
                        this.agent.History = false;
                    }
                    break;
                case ReceivedCommand.DirectData:
                    {
                        this.OnDirectDataRequest(msg);
                    }
                    break;
                case ReceivedCommand.Init:
                    {
                        this.OnInitializeRequest(msg);
                    }
                    break;
                case ReceivedCommand.KeepAlive:
                    {
                        this.OnKeepAlive(msg);
                    }
                    break;
                case ReceivedCommand.StartDev:
                    {
                        this.OnStartDevice(msg);
                        hvsc.Start();
                    }
                    break;
                case ReceivedCommand.StopDev:
                    {
                        this.OnStopDevice(msg);
                    }
                    break;

                case ReceivedCommand.Reply:
                    {
                        this.OnServerReply(msg);
                    }
                    break;
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
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);

            var p = this.builder.GetTimePacket(qn);
            this.agent.SendPacket(p, default(DateTime));
            this.SendResultPacket(qn);
        }

        private void OnSetTime(string msg)
        {
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);

            string time = ParseValue(msg, "SystemTime");
            DateTime dt = this.ParseDateTime(time);
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
            string ret = DataHandler.ParseValue(msg, "ExeRtn");
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
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);

            string sno = ParseValue(msg, "SNO");

            string eno = ParseValue(msg, "ENO");

            string beginTime = ParseValue(msg, "BeginTime");
            string endTime = ParseValue(msg, "EndTime");

            string polId = ParseValue(msg, "PolId");

            this.UploadHistoryData(qn, eno, beginTime, endTime, polId);

            this.SendResultPacket(qn);
        }

        private void UploadHistoryData(string qn, string eno, string beginTime, string endTime, string polId)
        {
            DateTime f = ParseDateTime(beginTime);
            DateTime t = ParseDateTime(endTime);
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

                    List<DataPacket> pks = builder.GetDataPackets(deviceKey, dt, content, true);
                    foreach (var p in pks)
                    {
                        Thread.Sleep(10);
                        this.agent.SendDataPacket(p, dt);
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
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);

        }

        private void OnDataRequest(string msg)
        {
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);
            // Upload data when right time.
            this.agent.Started = true;
        }

        private void OnStartDevice(string msg)
        {
            string eno = ParseValue(msg, "ENO");
            if (eno == "")
            {
                hvsc.Start();
            }
            else if (eno == "")
            {
                hvsc.Start();
            }
        }

        private void OnStopDevice(string msg)
        {
            string eno = ParseValue(msg, "ENO");
            if (eno == "")
            {
                hvsc.Stop();
            }
            else if (eno == "")
            {
                isc.Stop();
            }
        }

        private void OnDirectDataRequest(string msg)
        {
            string qn = ParseValue(msg, "QN");
            this.SendReplyPacket(qn);
            this.SendResultPacket(qn);
            // Upload data when right time.
            this.agent.Started = true;
        }

        private static int ParseCommandCode(string msg)
        {
            int code = 0;
            if (int.TryParse(ParseValue(msg, "CN"), out code))
            {
                return code;
            }
            return 0;
        }

        private void HandleSetPassword(string msg)
        {
            Settings.Instance.Password = DataHandler.ParseValue(msg, "PW");
            // TODO: 应答
        }

        private DateTime ParseDateTime(string time)
        {
            // 2009 05 06 08 30 30
            try
            {
                int y = int.Parse(time.Substring(0, 4));
                int m = int.Parse(time.Substring(4, 2));
                int d = int.Parse(time.Substring(6, 2));
                int h = int.Parse(time.Substring(8, 2));
                int min = int.Parse(time.Substring(10, 2));
                int sec = int.Parse(time.Substring(12, 2));
                DateTime dt = new DateTime(y, m, d, h, min, sec);
                return dt;
            }
            catch (FormatException e)
            {
                return (default(DateTime));
            }
        }

        private static string ParseValue(string msg, string key)
        {
            string tof = string.Format("{0}=", key);
            int p = msg.IndexOf(tof);
            if (p > 0)
            {
                int e = msg.IndexOf(";", p);
                if (e < 0)
                {
                    e = msg.IndexOf("&&", p);
                    if (e < 0)
                    {
                        e = msg.Length;
                    }
                }
                int len = tof.Length;
                // 3 is CN='s length
                string value = msg.Substring(p + len, e - p - len);
                return value;
            }
            return string.Empty;
        }

        
    }
}
