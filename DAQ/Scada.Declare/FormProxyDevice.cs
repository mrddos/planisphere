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
	public class FormProxyDevice : Device
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport("user32.dll")]
		public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, int msg, int wParam, StringBuilder lParam, int flags, int timeout, out IntPtr pdwResult);

        [DllImport("kernel32.dll")]
        public extern static int GetLastError();

        private IntPtr hWnd1 = IntPtr.Zero;

        private IntPtr hWnd2 = IntPtr.Zero;

        private const int WM_GETTEXT = 0x000D;

        private const int BufferLength = 1024;

        StringBuilder sb = new StringBuilder(BufferLength);

        private readonly string[] EmptyStringArray = new string[0]; 

        int nEditId1 = 0;
        int nEditId2 = 0;
        int nEditId3 = 0;
        int nEditId4 = 0;

		public FormProxyDevice(DeviceEntry entry)
		{

			this.Initialize(entry);
		}

		~FormProxyDevice()
        {
        }

		private bool Initialize(DeviceEntry entry)
		{
            this.Name = entry[DeviceEntry.Name].ToString();
            this.Path = entry[DeviceEntry.Path].ToString();
            this.Version = entry[DeviceEntry.Version].ToString();

            int nEditId1 = (StringValue)entry["EditId1"];
            int nEditId2 = (StringValue)entry["EditId2"];
            int nEditId3 = (StringValue)entry["EditId3"];
            int nEditId4 = (StringValue)entry["EditId4"];

			return true;
		}

        private string GetText(IntPtr hWnd, int nCtrlId)
        {
            sb.Length = 0;
            IntPtr edit = GetDlgItem(hWnd, nCtrlId);
            if (edit == IntPtr.Zero)
            {
                string wrongEditId = string.Format("Wrong Edit Id: {0}", nCtrlId);
                RecordManager.DoSystemEventRecord(this, wrongEditId);
            }
            IntPtr result = IntPtr.Zero;
            SendMessageTimeout(edit, WM_GETTEXT, BufferLength, sb, 0, 1000, out result);
            return sb.ToString();
        }

        private IntPtr FetchWindowHandle(string processName)
        {
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Scada.FormProxy.exe"; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出
                process.Start();
            }

            Thread.Sleep(678);

            byte[] bytes = new byte[32];
            string hWndCfgFile = this.Path + "\\HWND.r";
            FileStream fs = File.Open(hWndCfgFile, FileMode.Open);
            int r = fs.Read(bytes, 0, 32);
            fs.Close();

            string line = Encoding.ASCII.GetString(bytes, 0, r);

            return (IntPtr)int.Parse(line);
        }

        private string[] GetData(IntPtr hWnd)
        {
            string[] ret = EmptyStringArray;
            if (hWnd != IntPtr.Zero)
            {
                string e1 = GetText(hWnd, this.nEditId1);
                string e2 = GetText(hWnd, this.nEditId2);
                string e3 = GetText(hWnd, this.nEditId3);
                ret = new string[] { e1, e2, e3 };
            }
            return ret;
        }

		public override void Start(string address)
		{
            Thread thread = new Thread(new ThreadStart(() => {

                // TODO:
                this.hWnd1 = FetchWindowHandle("?");
                this.hWnd2 = FetchWindowHandle("?");


                Timer timer = new Timer(new TimerCallback((object o) => {

                    string[] ds1 = GetData(hWnd1);
                    string[] ds2 = GetData(hWnd2);
                }), null, 1000, 10000);
                
            }));

            thread.Start();
		}

		public override void Stop()
		{
			throw new NotImplementedException();
		}

		public override void Send(byte[] action)
		{
			throw new NotImplementedException();
		}
	}
}
