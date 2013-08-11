﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    public class DataPacket
    {
        private StringBuilder sb = new StringBuilder();

        private string deviceKey;

        private bool splitted = false;

        public ISettings Settings
        {
            get;
            set;
        }

        public DataPacket(SentCommand cmd)
        {
            this.Cn = string.Format("{0}", (int)cmd);
        }

        public DataPacket(string deviceKey, bool realTime = true, bool flow = false)
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

        public bool Splitted
        {
            get
            {
                return this.splitted;
            }
            set
            {
                this.splitted = value;
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
            set;
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

        public void SetContent(string sno, string eno, string dataTime, Dictionary<string, object> data)
        {
            if (data.Count == 0)
            {
                this.Cp = string.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("SNO={0};ENO={1};DataTime={2};", sno, eno, dataTime));
            foreach (var i in data)
            {
                if ("id".Equals(i.Key, StringComparison.OrdinalIgnoreCase) || "time".Equals(i.Key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string item = string.Format("{0}={1};", i.Key, i.Value);
                sb.Append(item);
            }
            sb.Remove(sb.Length - 1, 1);
            this.Cp = sb.ToString();
        }

        public void SetContent(string sno, string eno, string dataTime, string data)
        {
            if (data.Length == 0)
            {
                this.Cp = string.Empty;
                return;
            }
            StringBuilder sb = new StringBuilder();
            //sb.Append(string.Format("SNO={0};ENO={1};DataTime={2};", sno, eno, dataTime));
            sb.Append(data);
            this.Cp = sb.ToString();
        }
        

        public void SetReply(int reply)
        {
            this.Cp = string.Format("QnRtn={0}", reply);
        }

        internal void SetResult(int result)
        {
            this.Cp = string.Format("ExeRtn={0}", result);
        }

        private string Password
        {
            get
            {
                return this.Settings.Password;
            }
            set { }
        }


        private void GenQN()
        {
            if (!string.IsNullOrEmpty(this.QN))
                return;
            DateTime n = this.Settings.CurrentTime;
            string value = string.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}{6:d3}", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second, n.Millisecond);
            this.QN = value;
        }

        public void Build()
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

        public void BuildReply(string qn, int reply)
        {
            this.SetHeader();
            string ds = GetReplySections(qn, reply);

            int len = ds.Length;
            this.SetLength(len);
            this.sb.Append(ds);

            string crc16 = CRC16.GetCode(Encoding.ASCII.GetBytes(ds));
            this.sb.Append(crc16);
            this.sb.Append("\r\n");
        }

        public void BuildResult(string qn, int result)
        {
            this.SetHeader();
            string ds = GetResultSections(qn, result);

            int len = ds.Length;
            this.SetLength(len);
            this.sb.Append(ds);

            string crc16 = CRC16.GetCode(Encoding.ASCII.GetBytes(ds));
            this.sb.Append(crc16);
            this.sb.Append("\r\n");
        }

        internal void BuildNotify(string qn)
        {
            this.SetHeader();
            string ds = GetNotifySections(qn);

            int len = ds.Length;
            this.SetLength(len);
            this.sb.Append(ds);

            string crc16 = CRC16.GetCode(Encoding.ASCII.GetBytes(ds));
            this.sb.Append(crc16);
            this.sb.Append("\r\n");
        }

        internal void BuildGetTime(string time)
        {
            this.SetHeader();
            string ds = GetTimeSection(time);

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
            this.Mn = this.Settings.Mn;

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("QN={0};", this.QN));

            if (this.Splitted)
            {
                sb.Append(string.Format("PNUM={0};PNO={1};", this.PacketCount, this.PacketIndex));
            }

            sb.Append(string.Format("ST={0};CN={1};PW={2};MN={3};", this.St, this.Cn, this.Password, this.Mn));

            if (this.Splitted)
            {
                sb.Append("Flag=3;");
            }

            if (this.Cp == null)
            {
                this.Cp = "";
            }
            sb.Append(string.Format("CP=&&{0}&&", this.Cp));
            return sb.ToString();
        }

        private string GetReplySections(string qn, int reply)
        {
            this.Mn = this.Settings.Mn;
            this.St = Value.SysReply;
            string p = string.Format(
                "ST={0};CN={1};PW={2};MN={3};Flag=1;CP=&&QN={4};QnRtn={5}&&",
                this.St, this.Cn, this.Password, this.Mn, qn, reply);
            return p;
        }

        private string GetResultSections(string qn, int result)
        {
            this.Mn = this.Settings.Mn;
            this.St = Value.SysReply;
            string p = string.Format(
                "ST={0};CN={1};PW={2};MN={3};Flag=1;CP=&&QN={4};ExeRtn={5}&&",
                this.St, this.Cn, this.Password, this.Mn, qn, result);
            return p;
        }

        private string GetNotifySections(string qn)
        {
            this.Mn = this.Settings.Mn;
            this.St = Value.SysReply;
            string p = string.Format(
                "ST={0};CN={1};PW={2};MN={3};Flag=1;CP=&&QN={4}&&",
                this.St, this.Cn, this.Password, this.Mn, qn);
            return p;
        }

        private string GetTimeSection(string time)
        {
            this.Mn = this.Settings.Mn;

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("ST={0};CN={1};PW={2};MN={3};", this.St, this.Cn, this.Password, this.Mn));

            sb.Append(string.Format("CP=&&QN={0};SystemTime={1}&&", this.QN, time));
            return sb.ToString();
        }


        public int PacketCount { get; set; }

        public int PacketIndex { get; set; }

    }
}
