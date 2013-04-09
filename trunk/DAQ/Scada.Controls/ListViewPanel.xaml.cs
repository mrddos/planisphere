
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Scada.Controls
{
	using Scada.Controls.Data;
	using System.Collections.Generic;

	/// <summary>
	/// Interaction logic for ListViewPanel.xaml
	/// </summary>
	public partial class ListViewPanel : UserControl
	{
        private Control listView = null;

        private Control graphView = null;

		private DataListener dataListener;

		private List<Dictionary<string, object>> dataSource;

		public ListViewPanel()
		{
			InitializeComponent();
			
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

		[Category("Behavior")]
		public event RoutedEventHandler CloseClick;


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// Can NOT Find Element in Template;
		}

		private void ContentLoaded(object sender, RoutedEventArgs e)
		{

            this.FrList.Items.Add("每分钟");
            this.FrList.Items.Add("每15分钟");
            this.FrList.Items.Add("每小时");
            // this.FrList.Items.Add("");
            /*
			Button closeButton = (Button)this.HeaderBar.Template.FindName("CloseButton", this.HeaderBar);
			if (closeButton != null)
			{
				closeButton.Click += (s, c) => 
				{
					this.CloseClick(this, c);
				};
			}
             * */
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
		}

		private void OnDataArrivalBegin()
		{
			if (this.dataSource != null)
			{
				this.dataSource.Clear();
			}
			this.dataSource = new List<Dictionary<string, object>>();
		}


		private void OnDataArrival(Dictionary<string, object> entry)
		{
			this.dataSource.Add(entry);
		}

		private void OnDataArrivalEnd()
		{
            if (this.ListView != null)
            {
                if (this.ListView is ListView)
                {
                    ((ListView)this.ListView).ItemsSource = this.dataSource;
                }
            }
			
		}


	}
}
