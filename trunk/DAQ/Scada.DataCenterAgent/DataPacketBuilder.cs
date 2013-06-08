using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    class DataPacketBuilder
    {
        public const int SysSend = 38;

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


    }
}
