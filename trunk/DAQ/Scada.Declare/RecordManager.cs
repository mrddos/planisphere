using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scada.Declare
{
	public static class RecordManager
	{
		private static MySQLRecord mysql = null;

		private static AnalysisRecord analysis = null;

		private static FileRecord frameworkRecord = null;

        public struct StreamHolder
        {
            private FileStream fileStream;

            private string filePath;

            public FileStream FileStream
            {
                get { return this.fileStream; }
                set { this.fileStream = value; }
            }

            public string FilePath
            {
                get { return this.filePath; }
                set { this.filePath = value; }
            }
            
        }

        private static Dictionary<string, StreamHolder> streams = new Dictionary<string, StreamHolder>();

		public static void Initialize()
		{
			RecordManager.mysql = new MySQLRecord();

			RecordManager.analysis = new AnalysisRecord();

			RecordManager.frameworkRecord = new FileRecord("");
		}

		public static void DoRecord(DeviceData deviceData)
		{
			// TODO: Record it in the files.

            RecordManager.WriteDataToLog(deviceData);

			if (!RecordManager.mysql.DoRecord(deviceData))
			{
				// TODO: Do log this failure.
			}
		}

        private static void WriteDataToLog(DeviceData deviceData)
        {
            DateTime now = DateTime.Now;
            FileStream stream = RecordManager.GetLogFileStream(deviceData.Device, now);
            string time = string.Format("<{0}:{1}:{2}> ", now.Hour, now.Minute, now.Second);
            StringBuilder sb = new StringBuilder(time);
            foreach (object o in deviceData.Data)
            {
                if (o != null)
                {
                    sb.Append(o.ToString()).Append(" ");
                }
            }
            sb.Append("\r\n");
            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            stream.Write(bytes, 0, bytes.Length);
            
            // TODO: Optimize!
            stream.Flush();
            // TODO:???
        }

        private static FileStream GetLogFileStream(Device device, DateTime now)
        {
            string path = GetDeviceLogPath(device, now);
            string deviceName = device.Name;
            if (streams.ContainsKey(deviceName))
            {
                StreamHolder holder = streams[deviceName];
                if (holder.FilePath.ToLower() == path.ToLower())
                {
                    return holder.FileStream;
                }
                else
                {
                    FileStream fileStream = holder.FileStream;
                    if (fileStream != null)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    streams.Remove(deviceName);
                }
            }

            StreamHolder newHolder = new StreamHolder() { FilePath = path };
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                newHolder.FileStream = fs;
                streams[deviceName] = newHolder;
                return fs;
            }
            else
            {
                FileStream fs = File.Open(path, FileMode.Append);
                newHolder.FileStream = fs;
                streams[deviceName] = newHolder;
                return fs;
            }
        }

        private static string GetDeviceLogPath(Device device, DateTime now)
        {
            string fileName = string.Format("{0}-{1}-{2}.log", now.Year, now.Month, now.Day);
            string path = string.Format("{0}\\log\\{1}", device.Path, fileName);
            return path;
        }

	}
}
