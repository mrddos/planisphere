using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public static class RecordManager
	{
		private static MySQLRecord mysql = null;

		private static AnalysisRecord analysis = null;

		private static FileRecord frameworkRecord = null;

		public static void Initialize()
		{
			RecordManager.mysql = new MySQLRecord();

			RecordManager.analysis = new AnalysisRecord();

			RecordManager.frameworkRecord = new FileRecord("");
		}

		public static void DoRecord()
		{

		}

	}
}
