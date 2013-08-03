using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    class DataPacketBuilder
    {
        public const int SysSend = 38;

        public const int SysReply = 91;

        public DataPacketBuilder()
        {
        }

        private string GetDataTimeString(DateTime time)
        {
            DateTime n = time;
            string value = string.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);

            return value;
        }

        public DataPacket GetDataPacket(string deviceKey, Dictionary<string, object> data, bool realTime = false)
        {
            if (data.Count == 0)
            {
                return null;
            }
            DataPacket dp = new DataPacket(deviceKey, realTime);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            string sno = Settings.Instance.Sno;
            string eno = Settings.Instance.GetEquipNumber(deviceKey);
            string timeStr = string.Empty;
            if (data.ContainsKey("time"))
            {
                timeStr = (string)data["time"];
            }
            string dataTime = this.GetDataTimeString(DateTime.Parse(timeStr));
            dp.SetContent(sno, eno, dataTime, data);
            dp.Build();
            return dp;
        }

        public DataPacket GetFlowDataPacket(string deviceKey, Dictionary<string, object> data, bool realTime = false)
        {
            if (data.Count == 0)
            {
                return null;
            }
            DataPacket dp = new DataPacket(deviceKey, realTime, true);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            string sno = Settings.Instance.Sno;
            string eno = Settings.Instance.GetEquipNumber(deviceKey);
            string timeStr = (string)data["time"];
            string dataTime = this.GetDataTimeString(DateTime.Parse(timeStr));
            dp.SetContent(sno, eno, dataTime, data);
            dp.Build();
            return dp;
        }

        public DataPacket GetAuthPacket()
        {
            DataPacket dp = new DataPacket(SentCommand.Auth);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            dp.Build();
            return dp;
        }

        public DataPacket GetKeepAlivePacket()
        {
            DataPacket dp = new DataPacket(SentCommand.KeepAlive);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            dp.Build();
            return dp;
        }


        internal DataPacket GetReplyPacket(string qn)
        {
            DataPacket dp = new DataPacket(SentCommand.Reply);
            // DataPacket is for sending, SO ST=91.(SysReply)
            dp.St = SysReply;
            dp.SetReply(1);
            dp.QN = qn;
            dp.BuildReply();
            return dp;
        }

        internal DataPacket GetResultPacket(string qn)
        {
            DataPacket dp = new DataPacket(SentCommand.Result);
            // DataPacket is for sending, SO ST=91.(SysReply)
            dp.St = SysReply;
            dp.SetResult(1);
            dp.QN = qn;
            dp.BuildResult();
            return dp;
        }

        internal List<DataPacket> GetDataPackets(string deviceKey, DateTime dateTime, string content, bool history = false, string qn = null)
        {
            List<DataPacket> rets = new List<DataPacket>();
            int from = 0;
            const int MaxContent = 920;
            int count = content.Length / MaxContent + 2;
            int index = 1;

            string sno = Settings.Instance.Sno;
            string eno = Settings.Instance.GetEquipNumber(deviceKey);

            string dataTime = this.GetDataTimeString(dateTime);

            // Header packet
            DataPacket dp = null;
            if (!history)
                dp = new DataPacket(SentCommand.Data);
            else
                dp = new DataPacket(SentCommand.HistoryData);

            dp.Splitted = true;
            dp.PacketCount = count;
            dp.PacketIndex = index;
            dp.St = SysSend;
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<Settings.DeviceCode> codes = Settings.Instance.GetCodes(deviceKey);
            string contentCode = codes[0].Code;
            data.Add(contentCode, string.Empty);
            data.Add("time", dateTime.ToString());
            dp.SetContent(sno, eno, dataTime, data);
            dp.Build();
            rets.Add(dp);

            string pqn = dp.QN;

            while (true)
            {
                index += 1;
                dp = null;

                if (!history)
                    dp = new DataPacket(SentCommand.Data);
                else
                    dp = new DataPacket(SentCommand.HistoryData);
                dp.Splitted = true;
                dp.PacketCount = count;
                dp.PacketIndex = index;
                dp.St = SysSend;

                string c = content.Substring(from, Math.Min(MaxContent, content.Length - from));
                dp.QN = pqn;
                dp.SetContent(sno, eno, dataTime, c);
                dp.Build();

                rets.Add(dp);

                from += c.Length;
                if (from >= content.Length)
                    break;
                
            }
            return rets;
        }

        internal DataPacket GetTimePacket(string qn)
        {
            DataPacket dp = new DataPacket(SentCommand.GetTime);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.QN = qn;
            dp.St = SysSend;
            dp.BuildGetTime(this.GetDataTimeString(DateTime.Now));
            return dp;
        }
    }
}
