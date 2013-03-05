using Scada.Controls;
using Scada.Controls.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scada.MainVision
{
    public class Item
    {
        public string Name { get; set; }


        public UIElement Contents { get; set; }
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private DataProvider dataProvider;

		private PanelManager panelManager;

		private Timer refreshDataTimer;

		private bool connectedToDataBase = false;

        public MainWindow()
        {
            InitializeComponent();
			this.panelManager = new PanelManager(this);
        }

		private void LoadDataProvider()
		{
			if (connectedToDataBase)
			{
				this.dataProvider = new DBDataProvider();
			}
			else
			{
				this.dataProvider = new VirtualDataProvider();
			}
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
        {
			
			// TODO: Window Loaded.
			this.LoadDataProvider();

			this.refreshDataTimer = new Timer();
			this.refreshDataTimer.Interval = 2000;
			this.refreshDataTimer.Tick += RefreshDataTimerTick;

			this.refreshDataTimer.Start();

			// Device List
            this.DeviceList.ClickDeviceItem += this.OnDeviceItemClicked;

            this.DeviceList.AddDevice("高压电离室");
            this.DeviceList.AddDevice("气象");


            this.herePane.AddItem("高压电离室");
            this.herePane.AddItem("气象");


            this.herePane.AddItem("虚构设备1");
            this.herePane.AddItem("虚构设备2");
        }

		void RefreshDataTimerTick(object sender, EventArgs e)
		{
			if (this.dataProvider != null)
			{
				this.dataProvider.Refresh();
			}
		}

		private void ShowListViewPanel()
		{
			string tableName = "weather";
            DataListener dl = this.dataProvider.GetDataListener(tableName);
            ListViewPanel panel = this.panelManager.CreateListViewPanel(dl);
			// panel.AddDataListener(this.dataProvider.GetDataListener(tableName));
			panel.CloseClick += this.ClosePanelButtonClick;


			// Manage
			this.Grid.Children.Add(panel);

			this.panelManager.SetListViewPanelPos(panel, 1, 1);
		}

        private void ShowGraphViewPanel()
        {
            string tableName = "weather";
            DataListener dl = this.dataProvider.GetDataListener(tableName);
            GraphViewPanel panel = this.panelManager.CreateGraphViewPanel(dl);
            // panel.AddDataListener(this.dataProvider.GetDataListener(tableName));
            // panel.CloseClick += this.ClosePanelButtonClick;


            // Manage
            this.Grid.Children.Add(panel);

            this.panelManager.SetGraphViewPanelPos(panel, 1, 1);
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.ShowListViewPanel();

		}

		void ClosePanelButtonClick(object sender, RoutedEventArgs e)
		{
			ListViewPanel panel = (ListViewPanel)sender;
			this.panelManager.CloseListViewPanel(panel);
		}

        void OnDeviceItemClicked(object sender, EventArgs e)
        {
            this.ShowGraphViewPanel();
        }




    }
}
