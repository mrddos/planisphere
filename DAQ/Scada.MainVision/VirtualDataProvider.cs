
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Controls.Data;

namespace Scada.MainVision
{
	internal class VirtualDataProvider : DataProvider
	{
		public VirtualDataProvider()
		{

		}

		public override void Refresh()
		{

		}

		public override DataListener GetDataListener(string tableName)
		{
			return null;
		}
	}
}
