using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    public class DataPacket
    {
        private StringBuilder sb;

        private string deviceKey;


        public DataPacket(string deviceKey)
        {
            this.deviceKey = deviceKey;
            this.sb = new StringBuilder("##");
        }

        private void SetLength(int length)
        {
            string len = string.Format("{0:d4}", length);
            this.sb.Append(len);
        }

        public void Append()
        {
        }

        public byte[] ToBytes()
        {
            return null;
        }

        public string QN
        {
            get;
            private set;
        }

        // PNO.
        public string Id
        {
            get;
            private set;
        }

        // PNUM.
        public string Number
        {
            get;
            private set;
        }

        public string St
        {
            get;
            private set;
        }

        public string Cn
        {
            get;
            private set;
        }

        // MN=监测点编号，用作数据来源识别。编码规
        public string Mn
        {
            get;
            set;
        }

        public string Flag
        {
            get;
            set;
        }

        public string Cp
        {
            get;
            set;
        }


        private string Password
        {
            get;
            set;
        }


        private void GenQN()
        {
            DateTime n = DateTime.Now;
            string value = string.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}{6:d3}", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second, n.Millisecond);
            this.QN = value;
        }


    }
}
