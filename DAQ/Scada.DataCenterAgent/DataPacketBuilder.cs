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


        public DataPacket GetDataPacket(string deviceKey, Dictionary<string, object> data)
        {
            DataPacket dp = new DataPacket(deviceKey);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            dp.SetContent(data);
            dp.Build();
            return dp;
        }

        public DataPacket GetFlowDataPacket(string deviceKey, Dictionary<string, object> data)
        {
            DataPacket dp = new DataPacket(deviceKey, true, true);
            // DataPacket is for sending, SO ST=38.(SysSend)
            dp.St = SysSend;
            dp.SetContent(data);
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
