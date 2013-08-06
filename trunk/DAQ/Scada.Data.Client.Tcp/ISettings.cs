using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.DataCenterAgent
{
    public interface ISettings
    {


        string Mn
        {
            get;
            set;
        }

        string Password 
        { 
            get; 
            set; 
        }

        DateTime CurrentTime
        {
            get;
            set;
        }
    }
}
