using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Main
{
    public class Error
    {
        private string domain = null;

        private int code = 0;

        static Error()
        {
            // TODO: Load error descriptions from files.
        }

        public Error(string domain, int code)
        {
            this.domain = domain;
            this.code = code;
        }

        static string Lookup(string domain, int code)
        {
            return string.Empty;
        }

        public string Domain
        {
            get { return this.domain; }
        }

        public int Code
        {
            get
            {
                return this.code;
            }
        }

        public string Message
        {
            get
            {
                return Error.Lookup(this.domain, this.code);
            }
        }
    }
}
