using System;
using System.Collections.Generic;
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

namespace Scada.MainVision
{
	/// <summary>
	/// Interaction logic for DeviceListPanel.xaml
	/// </summary>
	public partial class DeviceListPanel : UserControl
	{
		private TreeViewItem groupTreeViewItem;

		public DeviceListPanel()
		{
			InitializeComponent();
			this.AddDeviceGroup();
		}


		public void AddDeviceGroup()
		{
			this.groupTreeViewItem = new TreeViewItem();
			groupTreeViewItem.Header = "Device List";
			groupTreeViewItem.Height = 30;

			this.DeviceList.Items.Add(groupTreeViewItem);
		}

		public void AddDevice(string deviceName)
		{
			Button deviceItem = new Button();
			deviceItem.Height = 40.0;
			deviceItem.Click += OnDeviceItemClick;
			// deviceItem.Template = 
			//this.DeviceListContainer.Children.Add(deviceItem);

			TreeViewItem tvi = new TreeViewItem();
			tvi.Header = deviceName;
			tvi.Height = 30;

			this.groupTreeViewItem.Items.Add(tvi);

		}

		private void OnDeviceItemClick(object sender, RoutedEventArgs args)
		{
			//DeviceList
		}

		private void DeviceListLoaded(object sender, RoutedEventArgs e)
		{
			
		}


	}
}
