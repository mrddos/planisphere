

using Scada.Declare;

namespace Scada.Main
{
    public partial class MainForm
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private bool OnDataArrival(DeviceData data)
		{
			string line = data.Line;
			Device device = data.Device;
			RecordManager.DoRecord();

			if (line.IndexOf("SFTW-131-001ER Ver") >= 0)
			{
				Actions.Delay(1000, () =>
				{
					device.Send("#S 0\r");

				});
			}


			// TODO: Into Database;
			return true;
		}

		////////////////////////////////////////////////////////
		public void OnDataReceived(object state)
		{
			if (state is DeviceData)
			{
				this.OnDataArrival((DeviceData)state);
				




			}
		}


    }
}
