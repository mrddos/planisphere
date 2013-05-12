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
        public const string ProcessName = "ProcessName";

		[DllImport("user32.dll")]
		static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport("user32.dll")]
		public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, int msg, int wParam, StringBuilder lParam, int flags, int timeout, out IntPtr pdwResult);

        [DllImport("kernel32.dll")]
        public extern static int GetLastError();

        private string processName;

        private IntPtr hWnd = IntPtr.Zero;


        private const int WM_GETTEXT = 0x000D;

        private const int BufferLength = 1024;

        private const int MaxDelay = 10;

        StringBuilder sb = new StringBuilder(BufferLength);

        private readonly string[] EmptyStringArray = new string[0];

        private string insertIntoCommand;

        private FieldConfig[] fieldsConfig = null;

        private List<int> elemIdList = new List<int>();

        private DateTime lastDateTime = default(DateTime);

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
            this.Id = entry[DeviceEntry.Identity].ToString();
            this.Path = entry[DeviceEntry.Path].ToString();
            this.Version = entry[DeviceEntry.Version].ToString();

            this.processName = (StringValue)entry[ProcessName];

            for (int i = 1; i < 50; i++)
            {
                string elemId = string.Format("ElemId{0}", i);
                if (!entry.Contains(elemId))
                {
                    break;
                }
                string controlIdStr = (StringValue)entry[elemId];
                int controlId = 0;
                if (int.TryParse(controlIdStr, out controlId))
                {
                    this.elemIdList.Add(controlId);
                }
            }

            string tableName = (StringValue)entry[DeviceEntry.TableName];
            string tableFields = (StringValue)entry[DeviceEntry.TableFields];

            string[] fields = tableFields.Split(',');
            string atList = string.Empty;
            for (int i = 0; i < fields.Length; ++i)
            {
                string at = string.Format("@{0}, ", i + 1);
                atList += at;
            }
            atList = atList.TrimEnd(',', ' ');

            string cmd = string.Format("insert into {0}({1}) values({2})", tableName, tableFields, atList);
            this.insertIntoCommand = cmd;


            string fieldsConfigStr = (StringValue)entry[DeviceEntry.FieldsConfig];
            List<FieldConfig> fieldConfigList = ParseDataFieldConfig(fieldsConfigStr);
            this.fieldsConfig = fieldConfigList.ToArray<FieldConfig>();

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
            string hWndCfgFile = this.Path + "\\HWND.r";

            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Scada.FormProxy.exe"; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出
                
                string args = string.Format("{0} {1}", this.processName, hWndCfgFile);
                process.StartInfo.Arguments = args;
                process.Start();
            }

            Thread.Sleep(678);

            byte[] bytes = new byte[32];
            
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
                int i = 0;
                ret = new string[this.elemIdList.Count];
                foreach (int elemId in this.elemIdList)
                {
                    ret[i] = GetText(hWnd, elemId);
                    i++;
                }
            }
            return ret;
        }

        private bool GetDeviceData(string[] data, DateTime time, out DeviceData deviceData)
        {
            deviceData = default(DeviceData);
            if (data == null || data.Length == 0)
            {
                return false;
            }
            deviceData.Time = time;
            object[] fields = Device.GetFieldsData(data, time, this.fieldsConfig);
            deviceData = new DeviceData(this, fields);
            deviceData.InsertIntoCommand = this.insertIntoCommand;
            //deviceData.FieldsConfig = this.fieldsConfig;
            return true;
        }

		public override void Start(string address)
		{
            Timer timer = null;

            Thread thread = new Thread(new ThreadStart(() => {

                // Fetch Window Handle from HWND.r file.
                this.hWnd = FetchWindowHandle(this.processName);

                // Start timer to work.
                timer = new Timer(new TimerCallback((object o) => {

                    DateTime now = DateTime.Now;
                    if (!this.IsRightTime(now))
                    {
                        return;
                    }

                    int second = (now.Second < 30) ? 0 : 30;
                    DateTime rightTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second);
                    if (rightTime == this.lastDateTime)
                    {
                        return;
                    }
                    // this.lastDateTime = rightTime;
                    string[] dataSet = GetData(hWnd);

                    DeviceData dd = default(DeviceData);
                    bool got = this.GetDeviceData(dataSet, rightTime, out dd);
                    if (got)
                    {
                        this.lastDateTime = rightTime;
                        this.SynchronizationContext.Post(this.DataReceived, dd);
                    }

                }), null, 1000, 1000);
                
            }));

            thread.Start();
		}

		public override void Stop()
		{
			throw new NotImplementedException();
		}

		public override void Send(byte[] action, DateTime time)
		{
			throw new NotImplementedException();
		}

        // VB form data every 30 sec.
        // Verify the time (second) is the right time.
        private bool IsRightTime(DateTime now)
        {
            if (now.Second >= 0 && now.Second <= MaxDelay)
            {
                return true;
            }
            else if (now.Second >= 30 && now.Second <= (30 + MaxDelay))
            {
                return true;
            }
            return false;
        }
	}
}
