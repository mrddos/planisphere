using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scada.Auth
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

        // Move the window by mouse-press-down.
        private void WindowMoveHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// Close the Window.
        private void OnCloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
        private void OnMaxButton(object sender, RoutedEventArgs e)
        {
            this.MaxWidth = SystemParameters.WorkArea.Width + 8;
            this.MaxHeight = SystemParameters.WorkArea.Height + 8;
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        */

        private void OnMinButton(object sender, RoutedEventArgs e)
        {
            // TODO: ? Ask?
            this.WindowState = WindowState.Minimized;
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Scada.MainVision.exe"; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出
                process.Start();

            }
            this.Close();
        }
	}
}
