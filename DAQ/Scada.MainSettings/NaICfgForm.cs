using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.MainSettings
{
    public partial class NaICfgForm : UserControl, IApply
    {
        public NaICfgForm()
        {
            InitializeComponent();
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
