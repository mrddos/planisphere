

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


				// TODO: Into Database;


			}
		}
    }
}
