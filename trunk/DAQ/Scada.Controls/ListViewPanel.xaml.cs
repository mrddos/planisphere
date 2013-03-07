
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
        private Control view = null;

		private DataListener dataListener;

		private List<Dictionary<string, object>> dataSource;

		public ListViewPanel()
		{
			InitializeComponent();
			
		}

        public Control ViewContent
		{
			get
			{
                return this.view;
			}
			set
			{
                this.view = value;
                if (this.view != null)
                {
                    this.ViewContainer.Content = this.view;
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
            //if (!entry.ContainsKey("Name"))
            //{
            //    entry.Add("Name", "Healer");
            //}
			this.dataSource.Add(entry);
		}

		private void OnDataArrivalEnd()
		{
            if (this.ViewContent != null)
            {
                if (this.ViewContent is ListView)
                {
                    ((ListView)this.ViewContent).ItemsSource = this.dataSource;
                }
            }
			
		}


	}
}
