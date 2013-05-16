﻿
namespace Scada.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;

    using Scada.Controls.Data;
    using Scada.MainVision;
    using Microsoft.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
	/// <summary>
	/// Interaction logic for ListViewPanel.xaml
	/// </summary>
	public partial class ListViewPanel : UserControl
	{
        private Control listView = null;

        private Control searchView = null;

        private Control graphView = null;

        private Control ctrlView = null;

		private DataListener dataListener;

        private DataProvider dataProvider;

        private string deviceKey;

		private List<Dictionary<string, object>> dataSource;

        private DataArrivalConfig config;

        private const string Time = "time";

        private bool readTimeMode = true;

        private const int MaxListCount = 100;


        // Must Use the <Full Name>
        private System.Windows.Forms.Timer refreshDataTimer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName">Display Name</param>
        /// <param name="interval">In Seconds</param>
        public ListViewPanel(DataProvider dataProvider, ConfigEntry entry)
		{
			InitializeComponent();
            this.deviceKey = entry.DeviceKey;
            this.DisplayName = entry.DisplayName;
            this.dataProvider = dataProvider;

            this.refreshDataTimer = new System.Windows.Forms.Timer();
            this.refreshDataTimer.Interval = (entry.Interval * 1000);
            this.refreshDataTimer.Tick += RefreshDataTimerTick;
            this.refreshDataTimer.Start();
		}

        private void RefreshDataTimerTick(object sender, EventArgs e)
        {
            if (!this.readTimeMode)
            {
                return;
            }
            // TODO: Current settings? if show current, continue.
            // If filter by start -> end time, returns.

            // TODO: Check Whether the DeviceKey is in current...
            if (this.deviceKey != null)
            {
                if (this.deviceKey == this.dataProvider.CurrentDeviceKey)
                {
                    this.dataProvider.RefreshTimeline(this.deviceKey);
                }
                else
                {
                    string msg = "Not current device key.";
                }
            }
        }

        public Control ListView
		{
			get
			{
                return this.listView;
			}
			set
			{
                this.listView = value;
                if (this.listView != null)
                {
                    this.ListViewContainer.Content = this.listView;
                    this.ApplyListStyle((ListView)this.listView);
                }
			}
		}

        public Control SearchView
        {
            get
            {
                return this.searchView;
            }

            set
            {
                this.searchView = value;
                if (this.listView != null)
                {
                    this.SearchViewContainer.Content = this.searchView;
                    this.ApplyListStyle((ListView)this.searchView);
                }
            }
        }

        public Control GraphView
		{
			get
			{
                return this.graphView;
			}
			set
			{
                this.graphView = value;
                if (this.graphView != null)
                {
                    this.GraphViewContainer.Content = this.graphView;
                }
			}
		}

        public Control ControlPanel
        {
            get
            {
                return this.ctrlView;
            }

            set
            {
                this.ctrlView = value;
                if (this.ctrlView != null)
                {
                    this.ControlPanelTabItem.Visibility = Visibility.Visible;
                    this.ControlPanelContainer.Content = this.ctrlView;
                }
            }
        }

        public string DisplayName
        {
            get;
            set;
        }

		[Category("Behavior")]
		public event RoutedEventHandler CloseClick;

        private void ApplyListStyle(ListView listView)
        {
            Color c = Color.FromRgb(83, 83, 83);
            listView.Background = new SolidColorBrush(c);
            var d = listView.ItemTemplate;

            listView.Style = (Style)this.Resources["ListViewKey"];
        }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            DateTime now = DateTime.Now;
            this.FromDate.SelectedDate = now.AddDays(-2);
            this.ToDate.SelectedDate = now.AddDays(-1);
			// Can NOT Find Element in Template;
		}

		private void ContentLoaded(object sender, RoutedEventArgs e)
		{
            this.Title.Text = this.DisplayName;

            this.FrList.Items.Add("每分钟");
            this.FrList.Items.Add("每小时");
            
            this.CloseButton.Click += (s, c) => 
				{
					this.CloseClick(this, c);
				};
		}

		public void AddDataListener(DataListener listener)
		{
			this.dataListener = listener;
			if (this.dataListener != null)
			{
				this.dataListener.OnDataArrivalBegin += this.OnDataArrivalBegin;
				this.dataListener.OnDataArrival += this.OnDataArrival;
				this.dataListener.OnDataArrivalEnd += this.OnDataArrivalEnd;
			}

            // TODO: !!
            this.dataSource = new List<Dictionary<string, object>>();
		}

        // BEGIN
        private void OnDataArrivalBegin(DataArrivalConfig config)
		{
            this.config = config;
            if (config == DataArrivalConfig.TimeRange)
            {
                // For show new data source, so clear the old data source.
                this.dataSource.Clear();
            }
            else if (config == DataArrivalConfig.TimeRecent)
            {

            }
		}


        // ARRIVAL
		private void OnDataArrival(Dictionary<string, object> entry)
		{
            if (this.config == DataArrivalConfig.TimeRecent)
            {
                this.dataSource.Add(entry); 
            }
            else
            {
                this.dataSource.Add(entry);
            }
            // Sort ...
            // Insert to the Top.
            
		}

        // END
		private void OnDataArrivalEnd()
		{
            // TODO: Chekc the data source?
            if (this.ListView != null)
            {
                if (this.ListView is ListView)
                {
                    this.dataSource.Sort(DBDataProvider.DateTimeCompare);

                    ListView listView = (ListView)this.ListView;
                    // Remember the Selected item.
                    int selected = listView.SelectedIndex;
                    listView.ItemsSource = null;
                    // List can only hold 100 items.
                    if (this.dataSource.Count > MaxListCount)
                    {
                        int p = 100;
                        int l = this.dataSource.Count - p;
                        this.dataSource.RemoveRange(p, l);
                    }
                    listView.ItemsSource = this.dataSource;
                    listView.SelectedIndex = selected;
                }
            }
			
		}
        
        // When click the Search Button.
        private void SearchByDateRange(object sender, RoutedEventArgs e)
        {
            this.readTimeMode = false;
            var dt1 = this.FromDate.SelectedDate.Value;
            var dt2 = this.ToDate.SelectedDate.Value;

            this.dataProvider.RefreshTimeRange(this.deviceKey, dt1, dt2);

        }

        private bool ValidTimeRange(DateTime fromDate, DateTime toDate)
        {
            return fromDate <= toDate;
        }

        private void DatePickerCalendarClosed(object sender, RoutedEventArgs e)
        {
            DatePicker picker = (DatePicker)sender;
            if (picker.Name == "FromDate")
            {
                DateTime? dt = picker.SelectedDate;
                if (dt.HasValue)
                {
                    if (!this.ToDate.SelectedDate.HasValue)
                    {
                        this.ToDate.SelectedDate = dt.Value.AddDays(1);
                    }
                }

                if (!this.ValidTimeRange(this.FromDate.SelectedDate.Value, this.ToDate.SelectedDate.Value))
                {

                }
            }
            else if (picker.Name == "ToDate")
            {
                DateTime? dt = picker.SelectedDate;
                if (dt.HasValue)
                {
                    if (!this.FromDate.SelectedDate.HasValue)
                    {
                        this.FromDate.SelectedDate = dt.Value.AddDays(-1);
                    }
                }

                if (!this.ValidTimeRange(this.FromDate.SelectedDate.Value, this.ToDate.SelectedDate.Value))
                {

                }
            }
        }

        // Select the ChartView shown.
        private void ShowChartView(object sender, RoutedEventArgs e)
        {
            this.TabCtrl.SelectedItem = this.ChartViewTabItem;
        }


	}
}
