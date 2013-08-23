using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Data.Client
{
    public class Packet
    {
        private JObject jobject = new JObject();

        public Packet()
        {

        }

        public Packet(int result)
        {
            this.Result = result;
        }

        private int Result
        {
            get;
            set;
        }


        public string ToString()
        {
            this.jobject["result"] = this.Result;
            return this.jobject.ToString();
        }
    }
}
