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
    class DeviceItem
    {
        public string DisplayName { get; set; }

		public string DeviceKey { get; set; }
    }

	/// <summary>
	/// Interaction logic for DeviceListPanel.xaml
	/// </summary>
	public partial class DeviceListPanel : UserControl
	{
        private const string DeviceItemTemplate = "DeviceItemTemplate";

		private TreeViewItem deviceGroup;

		public DeviceListPanel()
		{
			InitializeComponent();
			this.AddDeviceGroup();
		}


		public void AddDeviceGroup()
		{
			this.deviceGroup = new TreeViewItem();
			deviceGroup.Header = "设备列表";
			deviceGroup.IsExpanded = true;

			this.DeviceList.Items.Add(deviceGroup);
		}

		public void AddDevice(string deviceName, string deviceKey)
		{
            //
            //ControlTemplate ct = (ControlTemplate)this.Resources[DeviceItemTemplate];


			TreeViewItem tvi = new TreeViewItem();
            //tvi.Template = ct;
			tvi.DataContext = new DeviceItem() { DisplayName = deviceName, DeviceKey = deviceKey };
            tvi.MouseDoubleClick += OnDeviceItemClick;
			tvi.Header = deviceName;
            tvi.FontSize = 14.0;
            

			this.deviceGroup.Items.Add(tvi);

		}

		private void OnDeviceItemClick(object sender, RoutedEventArgs args)
		{
            this.ClickDeviceItem(sender, args);
		}

		private void DeviceListLoaded(object sender, RoutedEventArgs e)
		{
			
		}



        public event EventHandler ClickDeviceItem;
    }
}
