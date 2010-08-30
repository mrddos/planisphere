using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Planisphere.Demo
{
	public partial class MainPage : UserControl
	{

		private Storyboard timer = null;

		public MainPage()
		{
			InitializeComponent();
		}

		private void OnSendClick(object sender, RoutedEventArgs e)
		{
			SendChatMessage(this.sentText.Text);
		}

		private void SendChatMessage(string message)
		{
			WebClient wc = new WebClient();
			string listUrl = "/service/chat/send?msg=" + message + "&ts=" + DateTime.Now.Ticks;
			Uri uri = new Uri(listUrl, UriKind.Relative);

			wc.UploadStringAsync(uri, null, "");

			wc.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringCompleted);
		}

		public void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{

		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			timer = new Storyboard();

			timer.Duration = TimeSpan.FromSeconds(5);
			timer.Completed += new EventHandler(OnTimer);
			timer.Begin();


		}


		void OnTimer(object sender, EventArgs e)
		{

			WebClient wc = new WebClient();
			string listUrl = "/service/chat/recv?ts=" + DateTime.Now.Ticks;
			Uri uri = new Uri(listUrl, UriKind.Relative);

			wc.DownloadStringAsync(uri);

			wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
			timer.Begin();

		}

		public void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			this.recvText.Text += "\r\n" + e.Result;
		}

	}
}
