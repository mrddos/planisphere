using Scada.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.RecordAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x004a)
            {
                CopyDataStruct cds = new CopyDataStruct();
                cds = (CopyDataStruct)m.GetLParam(cds.GetType());
                //Debug.WriteLine(cds.lpData);
                // MessageBox.Show(cds.lpData);
            }
            base.DefWndProc(ref m);
        }
    }
}
