

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
		private bool OnDataArrival(DeviceData deviceData)
		{
			// object[] data = deviceData.Data;
			// Device device = deviceData.Device;

			RecordManager.DoRecord(deviceData);

			return true;
		}

		////////////////////////////////////////////////////////
		public void OnDataReceived(object state)
		{
			if (state is DeviceData)
			{
				this.OnDataArrival((DeviceData)state);

                // TODO: Register the next time and action.

				




			}
		}


    }
}
