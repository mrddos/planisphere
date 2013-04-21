using Scada.Controls;
using Scada.Controls.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

    enum Device
    {
        HIPC = 0,
        Weather,
        Safe,
        RD
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private DataProvider dataProvider;

		private PanelManager panelManager;

        private List<HerePaneItem> panes;

		private Timer refreshDataTimer;

        private Timer refreshPanelDataTimer;

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
			this.LoadConfig();
			this.LoadDataProvider();

			this.refreshDataTimer = new Timer();
			this.refreshDataTimer.Interval = 2000;
			this.refreshDataTimer.Tick += RefreshDataTimerTick;

			this.refreshDataTimer.Start();

            this.SetRefreshPanelDataTimer();

			// Device List
            this.DeviceList.ClickDeviceItem += this.OnDeviceItemClicked;

			Config cfg = Config.Instance();
			string[] deviceKeys = cfg.DeviceKeys;
			foreach (string deviceKey in deviceKeys)
            {
				string displayName = cfg.GetDisplayName(deviceKey);
				if (!string.IsNullOrEmpty(displayName))
				{
					this.DeviceList.AddDevice(displayName, deviceKey);
				}
            }

            this.AddDevicePanes();
            this.OnDeviceItemClicked(null, null);
        }

        private void SetRefreshPanelDataTimer()
        {
            this.refreshPanelDataTimer = new Timer();
            this.refreshPanelDataTimer.Interval = 2000;
            this.refreshPanelDataTimer.Tick += RefreshPanelDataTimerTick;

            this.refreshPanelDataTimer.Start();
        }

		private void LoadConfig()
		{
			Config.Instance().Load("./dsm.cfg");
		}

        private void AddDevicePanes()
        {
            this.panes = new List<HerePaneItem>();
			Config cfg = Config.Instance();
			string[] deviceKeys = cfg.DeviceKeys;
			foreach (string deviceKey in deviceKeys)
            {
				string displayName = cfg.GetDisplayName(deviceKey);
				if (!string.IsNullOrEmpty(displayName))
				{
					HerePaneItem herePaneItem = this.herePane.AddItem(displayName);
					panes.Add(herePaneItem);
				}
            }

            if (true)
            {
                var herePaneItem = panes[0];
            }
        }

		void RefreshDataTimerTick(object sender, EventArgs e)
		{
			if (this.dataProvider != null)
			{
				this.dataProvider.Refresh();

                // this.refreshDataTimer.Stop();
			}
		}

        void RefreshPanelDataTimerTick(object sender, EventArgs e)
        {
            // TODO: Refactor;
            Random r = new Random();

            HerePaneItem pane0 = this.panes[2];
            TextBlock text0 = pane0[0];
            TextBlock text1 = pane0[1];
            
            float a = r.Next(110, 120);
            int b = r.Next(20, 30);
            a += (b % 2 == 0) ? (float)b / 10 : -(float)b / 10;
            text0.Text = string.Format("温度: {0} ℃", a / 10);
            text1.Text = string.Format("风速: {0} m/s", a / 20);


            HerePaneItem pane1 = this.panes[0];
            TextBlock text2 = pane1[0];
            TextBlock text3 = pane1[1];

            int c = r.Next(1460, 1500);
            int d = r.Next(-20, 0);
            c -= d;
            float v = r.Next(390, 410);
            
            text2.Text = string.Format("剂量率: {0} nSv/h", c / 10);
            text3.Text = string.Format("高压值: {0} V", v);

        }

		private void ShowDataViewPanel(string tableName)
		{
            DataListener dl = this.dataProvider.GetDataListener(tableName);
            ListViewPanel panel = this.panelManager.CreateDataViewPanel(dl);
			// panel.AddDataListener(this.dataProvider.GetDataListener(tableName));
			panel.CloseClick += this.ClosePanelButtonClick;


			// Manage
			this.Grid.Children.Add(panel);

			this.panelManager.SetListViewPanelPos(panel, 2, 1);
		}

      

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            this.ShowDataViewPanel("Scada.HIPC");

		}

		void ClosePanelButtonClick(object sender, RoutedEventArgs e)
		{
			ListViewPanel panel = (ListViewPanel)sender;
			this.panelManager.CloseListViewPanel(panel);
		}

        void OnDeviceItemClicked(object sender, EventArgs e)
        {
            this.ShowDataViewPanel("Scada.HIPC");
            // this.ShowGraphViewPanel();
        }




    }
}
