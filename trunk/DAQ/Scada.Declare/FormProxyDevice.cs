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

        private IntPtr hWnd = IntPtr.Zero;

        private const int WM_GETTEXT = 0x000D;

        private const int BufferLength = 1024;

        StringBuilder sb = new StringBuilder(BufferLength);

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
			return true;
		}

        private string GetText(IntPtr hWnd, int nCtrlId)
        {
            sb.Length = 0;
            IntPtr edit = GetDlgItem(hWnd, nCtrlId);

            IntPtr result = IntPtr.Zero;
            SendMessageTimeout(edit, WM_GETTEXT, BufferLength, sb, 0, 1000, out result);
            return sb.ToString();
        }

		public override void Start(string address)
		{
            Thread thread = new Thread(new ThreadStart(() => {
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

                this.hWnd = (IntPtr)int.Parse(line);


                Timer timer = new Timer(new TimerCallback((object o) => {
                    IntPtr hWnd = (IntPtr)o;
                    if (hWnd != IntPtr.Zero)
                    {
                        string a = GetText(hWnd, 0x3ea);
                        string b = GetText(hWnd, 0x3eb);
                        // TODO: Write these data;
                        // HHAH
                    }
                }), this.hWnd, 1000, 10000);
                
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
