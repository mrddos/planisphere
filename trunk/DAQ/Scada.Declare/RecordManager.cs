﻿using Scada.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Scada.Declare
{
	public static class RecordManager
	{
		private static MySQLRecord mysql = null;

		private static AnalysisRecord analysis = null;

		private static FileRecord frameworkRecord = null;

        private static bool analysisToolOpen = false;

        private const string LocalPipeServer = "."; 

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

            return;
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Scada.RecordAnalysis.exe"; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出
                process.Start();

                Thread.Sleep(600);
            }

		}



		public static void DoRecord(DeviceData deviceData)
		{
			// TODO: Record it in the files.

            string line = RecordManager.WriteDataToLog(deviceData);

            if (analysisToolOpen)
            {
                SendToAnalysisWindow(line);
            }

			if (!RecordManager.mysql.DoRecord(deviceData))
			{
				// TODO: Do log this failure.
			}
		}

        private static string WriteDataToLog(DeviceData deviceData)
        {
            DateTime now = DateTime.Now;
            FileStream stream = RecordManager.GetLogFileStream(deviceData.Device, now);
            string time = string.Format("[{0:HH:mm:ss}] ", now);
            StringBuilder sb = new StringBuilder(time);
            foreach (object o in deviceData.Data)
            {
                if (o != null)
                {
                    sb.Append(o.ToString()).Append(" ");
                }
            }
            sb.Append("\r\n");
            string line = sb.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(line);
            stream.Write(bytes, 0, bytes.Length);
            
            // TODO: Optimize!
            stream.Flush();
            // TODO:???

            return line;
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
                string logPath = device.Path + "\\log";
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
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

        private static void SendToAnalysisWindow(string line)
        {
            IInterProcessConnection clientConnection = null;
            try
            {
                clientConnection = new ClientPipeConnection(Defines.LocalPipeName, LocalPipeServer);
                clientConnection.Connect();
                clientConnection.Write(line);
                clientConnection.Close();
            }
            catch (Exception)
            {
                clientConnection.Dispose();
                RecordManager.analysisToolOpen = false;
            }
        }
	}
}