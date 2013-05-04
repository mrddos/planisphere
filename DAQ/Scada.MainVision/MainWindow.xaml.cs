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

        private Timer refreshPanelDataTimer;

		private bool connectedToDataBase = true;

        private bool loaded = false;



        public MainWindow()
        {
            InitializeComponent();
			this.panelManager = new PanelManager(this);
        }

        /// <summary>
        /// Load data Provider, and would set the provider into every ListViewPanel instance.
        /// </summary>
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
            // this.ShowDataViewPanel("scada.hipc");
            // this.OnDeviceItemClicked(null, null);
            this.loaded = true;
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

        /*
		void RefreshDataTimerTick(object sender, EventArgs e)
		{
			if (this.dataProvider != null)
			{
				this.dataProvider.Refresh();

                // this.refreshDataTimer.Stop();
			}
		}
        */

        private string HeaderContent
        {
            get;
            set;
        }

        private object ExpanderContent
        {
            get;
            set;
        }

        private void DisplayPanelData(HerePaneItem panel, string data1, string data2)
        {
            TextBlock text2 = panel[0];
            TextBlock text3 = panel[1];

            text2.Text = data1;
            text3.Text = data2;
        }

        void RefreshPanelDataTimerTick(object sender, EventArgs e)
        {
            Random r = new Random();

            int c1 = r.Next(146, 150);
            int c2 = r.Next(450, 550);
            int c3 = r.Next(1460, 1500);
            int c4 = r.Next(1460, 1500);

            int c5 = r.Next(20, 30);
            int c6 = r.Next(13, 22);

            int c7 = r.Next(1460, 1500);
            int c8 = r.Next(10, 20);

            string d01 = string.Format("剂量率: {0} nSv/h", c1);
            string d02 = string.Format("高压值: {0} nSv/h", c2);

            string d11 = string.Format("剂量率: {0} nSv/h", c3);
            string d12 = "";

            string d21 = string.Format("温度: {0} ℃", c5);
            string d22 = string.Format("风速: {0} m/s", c6);

            string d31 = string.Format("流量: {0}", c7);
            string d32 = "";

            this.DisplayPanelData(this.panes[0], d01, d02);
            this.DisplayPanelData(this.panes[1], d11, d12);
            this.DisplayPanelData(this.panes[2], d21, d22);
            this.DisplayPanelData(this.panes[3], d31, d32);

        }

		private void ShowDataViewPanel(string deviceKey)
		{
            Config cfg = Config.Instance();
            var entry = cfg[deviceKey];


            ListViewPanel panel = this.panelManager.CreateDataViewPanel(this.dataProvider, entry);
			// panel.AddDataListener(this.dataProvider.GetDataListener(tableName));
            
			panel.CloseClick += this.ClosePanelButtonClick;


			// Manage
            if (!this.Grid.Children.Contains(panel))
            {
                this.Grid.Children.Add(panel);
            }

			this.panelManager.SetListViewPanelPos(panel, 2, 1);
		}

      

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            DeviceItem di = (DeviceItem)sender;
            this.ShowDataViewPanel(di.DeviceKey);

		}

		void ClosePanelButtonClick(object sender, RoutedEventArgs e)
		{
			ListViewPanel panel = (ListViewPanel)sender;
			this.panelManager.CloseListViewPanel(panel);
		}

        void OnDeviceItemClicked(object sender, EventArgs e)
        {
            DeviceItem di = sender as DeviceItem;
            if (di != null)
            {
                this.ShowDataViewPanel(di.DeviceKey);
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            if (!this.loaded)
            {
                return;
            }

            if (this.ExpanderContent == null)
            {
                this.ExpanderContent = this.Expander.Content;
            }

            Expander expander = sender as Expander;
            if (expander != null)
            {
                bool expanded = expander.IsExpanded;

                if (expanded)
                {
                    this.DeviceListColumn.Width = new GridLength(300.0);
                    this.Expander.Content = this.ExpanderContent;
                    this.DeviceList.Visibility = Visibility.Visible;
                    /*
                    this.Expander.Header = this.HeaderContent;
                    this.Expander.Width = 300;
                     * */
                    //this.DeviceList.Margin = new Thickness(5, 0, 5, 0);
                    //this.Expander.Margin = new Thickness(3, 3, 3, 3);
                }
                else
                {
                    this.Expander.Header = string.Empty;
                    this.Expander.Content = null;
                    this.DeviceListColumn.Width = new GridLength(40.0);
                    
                    this.DeviceList.Visibility = Visibility.Hidden;
                    /*
                    this.HeaderContent = (string)this.Expander.Header;
                    
                    this.Expander.Width = 30;
                     * */
                    //this.DeviceList.Margin = default(Thickness);
                    //this.Expander.Margin = default(Thickness);
                }

            }
        }


    }
}
