using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        StartHvs = 7011,
        StopHvs = 7012,

        StartIs = 7021,
        StopIs = 7022,
     
        Reply = 9012
    }

    public enum SentCommand
    {
        None = 0,
        Data = 2011,
        HistoryData = 2042,
        Auth = 6011,
        KeepAlive = 6031,
        Reply = 9011,
        Result = 9012,
    }

    class DataHandler
    {
        private DataPacketBuilder builder = new DataPacketBuilder();

        // Agent ref.
        private Agent agent;

        private SamplerController hvsc = new SamplerController("scada.hvsampler");

        private SamplerController isc = new SamplerController("scada.isampler");


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
                case ReceivedCommand.StartHvs:
                    {
                        hvsc.Start();
                    }
                    break;
                case ReceivedCommand.StopHvs:
                    {
                        hvsc.Stop();
                    }
                    break;
                case ReceivedCommand.StartIs:
                    {
                        isc.Start();
                    }
                    break;
                case ReceivedCommand.StopIs:
                    {
                        isc.Stop();
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
            // TODO:

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
