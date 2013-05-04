
namespace Scada.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;

    using Scada.Controls.Data;
    using Scada.MainVision;
	/// <summary>
	/// Interaction logic for ListViewPanel.xaml
	/// </summary>
	public partial class ListViewPanel : UserControl
	{
        private Control listView = null;

        private Control graphView = null;

		private DataListener dataListener;

        private DataProvider dataProvider;

        private string deviceKey;

		private List<Dictionary<string, object>> dataSource;





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
            // TODO: Current settings? if show current, continue.
            // If filter by start -> end time, returns.

            // TODO: Check Whether the DeviceKey is in current...
            if (this.deviceKey != null)
            {
                this.dataProvider.Refresh(this.deviceKey);
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

        public string DisplayName
        {
            get;
            set;
        }

		[Category("Behavior")]
		public event RoutedEventHandler CloseClick;


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
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

		private void OnDataArrivalBegin()
		{
            /*
			if (this.dataSource != null)
			{
				this.dataSource.Clear();
			}
			this.dataSource = new List<Dictionary<string, object>>();
             * */
            
            //this.dataSource.Clear();

            
		}



		private void OnDataArrival(Dictionary<string, object> entry)
		{
			this.dataSource.Add(entry);
            // Sort ...
            // Insert to the Top.
            this.dataSource.Sort(this.DateTimeCompare);
		}

        private int DateTimeCompare(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            DateTime adt = DateTime.Parse((string)a["Time"]);
            DateTime bdt = DateTime.Parse((string)b["Time"]);
            if (adt > bdt)
            {
                return -1;
            }
            return 1;
        }

		private void OnDataArrivalEnd()
		{
            // TODO: Chekc the data source?
            if (this.ListView != null)
            {
                if (this.ListView is ListView)
                {
                    ((ListView)this.ListView).ItemsSource = null;
                    ((ListView)this.ListView).ItemsSource = this.dataSource;
                }
            }
			
		}


	}
}
