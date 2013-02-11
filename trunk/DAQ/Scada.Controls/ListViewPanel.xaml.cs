using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Scada.Controls
{
	/// <summary>
	/// Interaction logic for ListViewPanel.xaml
	/// </summary>
	public partial class ListViewPanel : UserControl
	{

        private ListView listView = null;

		private DBDataListener listener;

		public ListViewPanel()
		{
			InitializeComponent();
			
		}

        public ListView ListViewContent
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

		[Category("Behavior")]
		public event RoutedEventHandler CloseClick;


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// Can NOT Find Element in Template;
		}

		private void ContentLoaded(object sender, RoutedEventArgs e)
		{
			Button closeButton = (Button)this.HeaderBar.Template.FindName("CloseButton", this.HeaderBar);
			if (closeButton != null)
			{
				closeButton.Click += (s, c) => 
				{
					this.CloseClick(this, c);
				};
			}
		}

		public void AddDataListener(DBDataListener listener)
		{
			this.listener = listener;
			if (this.listener != null)
			{
				this.listener.OnDataArrival += this.OnDataArrival;
			}
		}

		private void OnDataArrival(params object[] data)
		{

		}

	}
}
