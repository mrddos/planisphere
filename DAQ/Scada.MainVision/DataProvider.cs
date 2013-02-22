
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scada.Controls.Data;

namespace Scada.MainVision
{
	public abstract class DataProvider
	{
		/// <summary>
		/// 
		/// </summary>
		public abstract void Refresh();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public abstract DataListener GetDataListener(string tableName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract void RemoveDataListener(string tableName);

	}
}
