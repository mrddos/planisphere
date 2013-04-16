using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace Scada.Declare
{
	public class WebFileDevice : Device
	{
		private bool isVirtual = false;

		private bool isOpen = false;

		private DeviceEntry entry = null;

		private Timer timer = null;

		private string addr = "127.0.0.1";


        private string sn = "sara0240";

        private int index = 0;

		public WebFileDevice(DeviceEntry entry)
		{
            this.entry = entry;
			this.Initialize(entry);
		}

        ~WebFileDevice()
        {
        }

		// Initialize the device
		private void Initialize(DeviceEntry entry)
		{
			this.Name = entry[DeviceEntry.Name].ToString();
			this.Path = entry[DeviceEntry.Path].ToString();
			this.Version = entry[DeviceEntry.Version].ToString();

			this.addr = (StringValue)entry[DeviceEntry.IPAddress];

            // Virtual On
            string isVirtual = (StringValue)entry[DeviceEntry.Virtual];
            if (isVirtual != null && isVirtual.ToLower() == "true")
            {
                this.isVirtual = true;
            }
		}

		public bool IsVirtual
		{
			get { return this.isVirtual; }
		}

		private bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		/// <summary>
		/// 
		/// Ignore the address parameter
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private bool Connect(string address)
		{
			bool connected = true;

			this.timer = new Timer(new TimerCallback(TimerCallback), null, 1000, 1000 * 5);

			
			return connected;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
		private void TimerCallback(object o)
		{
            string fileName = GetFileName(index);
            string filePath = string.Empty;
            if (this.IsVirtual)
            {
                filePath = this.Path + "/sara0240_2013-01-19T06_05_00Z-5min.n42";
            }
            else
            {
                // TODO: Download the file.
                string address = this.addr + fileName;
                string localPath = this.Path + "\\" + fileName;
  
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(address, localPath);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            if (File.Exists(filePath))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);



                    // TODO: Post the data to UI thread.
                    
                }
                catch (Exception)
                {

                }
            }
		}

		public override void Start(string address)
		{
			this.Connect(address);
		}

		public override void Stop()
		{
			if (this.timer != null)
				this.timer.Dispose();
			isOpen = false;
		}

		public override void Send(byte[] action)
		{
            
		}

        /// <summary>
        /// index = 0, min = 0
        /// index = 1, min = 5
        /// ...
        /// index = 11, min = 55
        /// Returns this hour the Nth file by index.
        /// sara0240_2013-01-19T06_05_00Z-5min.n42
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        private string GetFileName(int index)
        {
            string fileName;
            DateTime t = DateTime.Now;
            fileName = string.Format("{0}_{1}-{2:D2}-{3:D2}T{4:D2}_{5:D2}_00Z-5min.n42",
                sn, t.Year, t.Month, t.Day, t.Hour, index * 5);
            return fileName;
        }
	}
}
