using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    public struct DataPacket
    {
        private StringBuilder sb;

        private string deviceKey;


        public DataPacket(string deviceKey)
        {
            this.deviceKey = deviceKey;
            this.sb = new StringBuilder();
        }

        public void SetLength(int length)
        {
        }

        public void Append()
        {
        }

        public byte[] ToBytes()
        {
            return null;
        }
    }
}
