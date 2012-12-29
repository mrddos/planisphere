using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Scada.Controls;
using Scada.Main.Properties;
using System.Threading;
using System.Diagnostics;
using Scada.Declare;

namespace Scada.Main
{
    public partial class MainForm
    {




		public void OnDataReceived(object state)
		{
			if (state is DeviceData)
			{
				DeviceData data = (DeviceData)state;


				RecordManager.DoRecord();

			}
		}
    }
}
