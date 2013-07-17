﻿using System;
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

        public DataPacket GetDataPacket(string deviceKey, Dictionary<string, object> data)
        {
            DataPacket dp = new DataPacket(deviceKey);
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

        public DataPacket GetFlowDataPacket(string deviceKey, Dictionary<string, object> data)
        {
            DataPacket dp = new DataPacket(deviceKey, true, true);
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
    }
}
