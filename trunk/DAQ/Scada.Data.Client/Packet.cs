using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Data.Client
{
    public class Packet
    {
        public const string EntryKey = "entry";

        public const string StationKey = "station";

        public const string TokenKey = "token";

        private JObject jobject = new JObject();

        private int result = 0;

        private bool hasResult = false;

        public Packet()
        {

        }

        private int Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
                this.hasResult = true;
            }
        }

        private string GetProperty(string propertyName)
        {
            return Packet.GetProperty(propertyName, this.jobject);
        }

        private static string GetProperty(string propertyName, JObject jsonObject)
        {
            JToken s = jsonObject[propertyName];
            if (s != null)
            {
                return s.ToString();
            }
            return string.Empty;
        }


        public string Station
        {
            get
            {
                return this.GetProperty(StationKey);
            }
            set
            {
                this.jobject[StationKey] = value;
            }
        }

        public string Token
        {
            get
            {
                return this.GetProperty(TokenKey);
            }
            set
            {
                this.jobject[TokenKey] = value;
            }
        }

        public override string ToString()
        {
            if (this.hasResult)
            {
                this.jobject["result"] = this.Result;
            }
            return this.jobject.ToString();
        }

        internal void AddData(Dictionary<string, object> d)
        {
            JArray entries = (JArray)this.jobject[EntryKey];
            if (entries == null)
            {
                entries = new JArray();
                this.jobject[EntryKey] = entries;
            }

            entries.Add(this.GetObject(d));

            entries.Add(this.GetObject(d));
        }

        private JObject GetObject(Dictionary<string, object> d)
        {
            JObject json = new JObject();
            foreach (var kv in d)
            {
                if (kv.Key.ToLower() == "id")
                    continue;
                if (kv.Value is string)
                {
                    json[kv.Key] = (string)kv.Value;
                }
            }
            return json;
        }
    }
}
