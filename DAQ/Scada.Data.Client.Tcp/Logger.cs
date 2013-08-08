using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    class Logger
    {
    }

    class Log
    {
        public static Logger GetLogFile(string deviceKey)
        {
            return new Logger();
        }
    }
}
