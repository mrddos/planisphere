using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    class Value
    {
        internal static string Parse(string msg, string key)
        {
            string tof = string.Format("{0}=", key);
            int p = msg.IndexOf(tof);
            if (p > 0)
            {
                int e = msg.IndexOf(";", p);
                if (e < 0)
                {
                    e = msg.IndexOf("&&", p);
                    if (e < 0)
                    {
                        e = msg.Length;
                    }
                }
                int len = tof.Length;
                // 3 is CN='s length
                string value = msg.Substring(p + len, e - p - len);
                return value;
            }
            return string.Empty;
        }
    }
}
