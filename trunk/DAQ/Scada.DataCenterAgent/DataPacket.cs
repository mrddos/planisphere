using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    public class DataPacket
    {
        private StringBuilder sb = new StringBuilder();

        private string deviceKey;

        public DataPacket(SentCommand cmd)
        {
            this.Cn = string.Format("{0}", (int)cmd);
        }

        public DataPacket(string deviceKey, bool realTime = true)
        {
            this.deviceKey = deviceKey;
            if (realTime)
            {
                this.Cn = string.Format("{0}", (int)SentCommand.Data);;
            }
            else
            {
                this.Cn = string.Format("{0}", (int)SentCommand.HistoryData);
            }
        }

        private void SetHeader()
        {
            this.sb.Append("##");
        }

        private void SetLength(int length)
        {
            string len = string.Format("{0:d4}", length);
            this.sb.Append(len);
        }

        public byte[] ToBytes()
        {
            return null;
        }


        public new string ToString()
        {
            return this.sb.ToString();
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

        public int St
        {
            get;
            set;
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
            private set;
        }

        public void SetContent(Dictionary<string, object> data)
        {
            if (data.Count == 0)
            {
                this.Cp = string.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var i in data)
            {
                string item = string.Format("{0}={1};", i.Key, i.Value);
                sb.Append(item);
            }
            sb.Remove(sb.Length - 1, 1);
            this.Cp = sb.ToString();
        }


        private string Password
        {
            get
            {
                return Settings.Instance.Password;
            }
            set { }
        }


        private void GenQN()
        {
            DateTime n = DateTime.Now;
            string value = string.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}{6:d3}", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second, n.Millisecond);
            this.QN = value;
        }



        internal void Build()
        {
            this.SetHeader();
            string ds = GetDataSections();

            int len = ds.Length;
            this.SetLength(len);
            this.sb.Append(ds);

            string crc16 = CRC16.GetCode(Encoding.ASCII.GetBytes(ds));
            this.sb.Append(crc16);
            this.sb.Append("\r\n");
        }


        private string GetDataSections()
        {
            this.GenQN();
            this.Mn = Settings.Instance.Mn;
            string p = string.Format("QN={0};ST={1};CN={2};PW={3};MN={4};Flag=1;CP=&&{5}&&", this.QN, this.St, this.Cn, this.Password, this.Mn, this.Cp);
            return p;
        }
    }
}
