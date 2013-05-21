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
            this.ShowDataViewPanel("scada.hipc");
            // this.OnDeviceItemClicked(null, null);
            this.loaded = true;

            // Max when startup;
            this.OnMaxButton(null, null);
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

        private void DisplayPanelData(HerePaneItem panel, string data1, string data2 = "", string data3 = "", string data4 = "")
        {
            TextBlock text1 = panel[0];
            TextBlock text2 = panel[1];
            TextBlock text3 = panel[2];
            TextBlock text4 = panel[3];

            if (data1 != null && data1.Length > 0)
            {
                text1.Text = data1;
            }
            if (data2 != null && data2.Length > 0)
            {
                text2.Text = data2;
            }
            if (data3 != null && data3.Length > 0)
            {
                text3.Text = data3;
            }
            if (data4 != null && data4.Length > 0)
            {
                text4.Text = data4;
            }
        }

        // 1 剂量率
        private void UpdatePanel_HIPC(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_Hipc);
            if (d == null)
            {
                return;
            }
            if (d.ContainsKey("doserate"))
            {
                string doserate = d["doserate"] as string;

                string doserateMsg = "剂量率: " + doserate + "uGy/h";
                this.DisplayPanelData(panel, doserateMsg);
            }
            
        }
        // 2 总剂量率、发现核素（置信度=100，剂量率>5nSv/h，最好可以设置剂量率的阈值）
        /*
         *  K-40 = K-40; (0, 100, 100) 
            I-131 = I-131; (0, 100, 100)
            Bi-214 = Bi-214; (0, 100, 100)
            Pb-214 = Pb-214; (0, 100, 100)
            Cs-137 = Cs-137; (0, 100, 100)
            Co-60 = Co-60; (0, 100, 100)
            Am-241 = Am-241; (0, 100, 100)
            Ba-140 = Ba-140;(0, 100, 100)
            Cs-134 = Cs-134;(0, 100, 100)
            I-133 = I-133; (0, 100, 100)
            Rh-106m = Rh-106m;(0, 100, 100)
            Ru-103 = Ru-103; (0, 100, 100)
            Te-129 = Te-129;(0, 100, 100)
         */
        private void UpdatePanel_NaI(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_NaI);
            if (d == null)
            {
                return;
            }
            if (!d.ContainsKey("doserate"))
            {
                return;
            }

            string doserate = (string)d["doserate"];
            string[] nuclides = { "K-40", "I-131", "Bi-214", "Pb-214", "Cs-137", "Co-60", "Am-241", "Ba-140", "Cs-134", "I-133", "Rh-106m", "Ru-103", "Te-129" };
            string[] nuclideMsgs = new string[3];
            int i = 0;
            foreach (string nuclide in nuclides)
            {
                string nuclideKey = nuclide.ToLower();
                if (d.ContainsKey(nuclideKey))
                {
                    string indicationKey = string.Format("Ind({0})", nuclideKey);
                    string indication = (string)d[indicationKey];
                    if (indication == "100")
                    {
                        string nuclideDoserate = (string)d[nuclide.ToLower()];
                        if (nuclideDoserate.Length > 0)
                        {
                            nuclideMsgs[i] = string.Format("{0}: {1}nSv/h", nuclide, nuclideDoserate);
                            i++;
                            if (i >= 3)
                            {
                                break;
                            }
                        }
                    }
                }

            }

            string doserateMsg = "总剂量率: " + doserate + "uSv/h";
            this.DisplayPanelData(panel, doserateMsg, 
                nuclideMsgs[0], nuclideMsgs[1], nuclideMsgs[2]);
        }
        // 3 // 风速、风向、雨量
        private void UpdatePanel_Weather(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_Weather);
            if (d == null)
            {
                return;
            }
            if (!d.ContainsKey("windspeed"))
            {
                return;
            }
            string windspeed = (string)d["windspeed"];
            string direction = (string)d["direction"];
            string raingauge = (string)d["raingauge"];

            string windspeedMsg = string.Format("风速: {0}", windspeed);
            string directionMsg = string.Format("风向: {0}", direction);
            string raingaugeMsg = string.Format("雨量: {0}", raingauge);

            this.DisplayPanelData(panel, windspeedMsg, directionMsg, raingaugeMsg);
   
        }
        // 4 采样状态（可用颜色表示）、累计采样体积（重要）、累计采样时间、瞬时采样流量、三种故障报警
        private void UpdatePanel_HV(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_HvSampler);
            if (d == null)
            {
                return;
            }
        }
        // 5 采样状态（可用颜色表示）、累计采样体积（重要）、累计采样时间、瞬时采样流量、三种故障报警
        private void UpdatePanel_I(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_ISampler);
            if (d == null)
            {
                return;
            }
        }
        // 6 市电状态、备电时间、舱内温度、门禁报警、烟感报警、浸水报警
        private void UpdatePanel_Shelter(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_Shelter);
            if (d == null)
            {
                return;
            }

            string batteryHours = "";
            string mainPowerOff = "";
            string temperature = "";

            const string MainPowKey = "ifmainpoweroff";
            const string BatteryHoursKey = "batteryhours";
            const string TemperatureKey = "temperature";
            if (d.ContainsKey(MainPowKey))
            {
                mainPowerOff = (string)d[MainPowKey];
            }

            if (d.ContainsKey(BatteryHoursKey))
            {
                batteryHours = (string)d[BatteryHoursKey];
            }

            if (d.ContainsKey(TemperatureKey))
            {
                temperature = (string)d[TemperatureKey];
            }

            string mainPowMsg = string.Format("市电状态: {0}", mainPowerOff);
            string batteryHoursMsg = string.Format("备电时间: {0}", batteryHours);
            string tempMsg = string.Format("舱内温度: {0}", temperature);

            this.DisplayPanelData(panel, mainPowMsg, batteryHoursMsg, tempMsg);
        }
        // 7 仅工作状态
        private void UpdatePanel_DWD(HerePaneItem panel)
        {
            var d = this.dataProvider.GetLatestData(DataProvider.DeviceKey_Dwd);
            if (d == null)
            {
                return;
            }
            if (!d.ContainsKey("islidopen"))
            {
                return;
            }
            string isLidOpen = (string)d["islidopen"];
            string LidOpenMsg = (isLidOpen == "1") ? "盖子打开" : "盖子关闭";
            this.DisplayPanelData(panel, LidOpenMsg);
        }

        void RefreshPanelDataTimerTick(object sender, EventArgs e)
        {
            this.dataProvider.RefreshTimeNow();

            this.UpdatePanel_HIPC(this.panes[0]);
            this.UpdatePanel_NaI(this.panes[1]);
            this.UpdatePanel_Weather(this.panes[2]);

            this.UpdatePanel_HV(this.panes[3]);
            this.UpdatePanel_I(this.panes[4]);

            this.UpdatePanel_Shelter(this.panes[5]);
            this.UpdatePanel_DWD(this.panes[6]);
        }

		private void ShowDataViewPanel(string deviceKey)
		{
            Config cfg = Config.Instance();
            var entry = cfg[deviceKey];


            ListViewPanel panel = this.panelManager.CreateDataViewPanel(this.dataProvider, entry);
            this.dataProvider.CurrentDeviceKey = deviceKey;
            
			panel.CloseClick += this.ClosePanelButtonClick;


			// Manage
            if (!this.Grid.Children.Contains(panel))
            {
                this.Grid.Children.Add(panel);
            }

			this.panelManager.SetListViewPanelPos(panel, 3, 1);
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
                //this.ExpanderContent = this.Expander.Content;
            }

            Expander expander = sender as Expander;
            if (expander != null)
            {
                bool expanded = expander.IsExpanded;

                if (expanded)
                {
                    this.DeviceListColumn.Width = new GridLength(300.0);
                    //this.Expander.Content = this.ExpanderContent;
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
                    //this.Expander.Header = string.Empty;
                    //this.Expander.Content = null;
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


        // Move the window by mouse-press-down.
        private void WindowMoveHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// System Menu
        /// Close the Window.
        private void OnCloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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

        private void OnMinButton(object sender, RoutedEventArgs e)
        {
            // TODO: ? Ask?
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private bool dataPanelHide = false;

        private bool devicePanelHide = false;

        private void OnHideDataPanelButton(object sender, RoutedEventArgs e)
        {
            if (this.dataPanelHide)
            {
                this.dataPanelHide = false;
                this.DataPanelRow.Height = new GridLength(172.0);
                this.herePane.Visibility = Visibility.Visible;
            }
            else
            {
                this.dataPanelHide = true;
                this.DataPanelRow.Height = new GridLength(15.0);
                this.herePane.Visibility = Visibility.Collapsed;
            }
        }

        private void OnHideDeviceButton(object sender, RoutedEventArgs e)
        {
            if (this.devicePanelHide)
            {
                this.devicePanelHide = false;
                this.DeviceListColumn.Width = new GridLength(300.0);
                this.DeviceList.Visibility = Visibility.Visible;
            }
            else
            {
                this.devicePanelHide = true;
                this.DeviceListColumn.Width = new GridLength(15.0);
                this.DeviceList.Visibility = Visibility.Collapsed;
            }
        }



    }
}
