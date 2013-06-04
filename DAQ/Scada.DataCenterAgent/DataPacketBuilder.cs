using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    class DataPacketBuilder
    {
        public DataPacketBuilder()
        {
        }


        public DataPacket GetDataPacket(string deviceKey, Dictionary<string, object> data)
        {
            DataPacket dp = new DataPacket(deviceKey);

            dp.SetContent(data);
            dp.Build();
            return dp;
        }

    }
}
