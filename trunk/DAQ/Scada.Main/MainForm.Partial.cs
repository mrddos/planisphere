

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

			RecordManager.DoRecord();


			Actions.Delay(1000, () =>
			{
				// TODO: When match some action entry, then do action

			});

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
