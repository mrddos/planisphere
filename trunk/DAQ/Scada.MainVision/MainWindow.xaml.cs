using Scada.Controls;
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

		private void WindowLoaded(object sender, RoutedEventArgs e)
        {
			// TODO: Window Loaded.
			if (connectedToDataBase)
			{
				this.dataProvider = new DBDataProvider();
			}
			else
			{
				this.dataProvider = new VirtualDataProvider();
			}

			this.refreshDataTimer = new Timer();
			this.refreshDataTimer.Interval = 2000;
			this.refreshDataTimer.Tick += RefreshDataTimerTick;

			this.refreshDataTimer.Start();
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
			ListViewPanel panel = this.panelManager.CreateListViewPanel();
			panel.AddDataListener(this.dataProvider.GetDataListener(""));
			panel.CloseClick += this.ClosePanelButtonClick;


			// Manage
			this.Grid.Children.Add(panel);

			this.panelManager.SetListViewPanelPos(panel, 1, 1);
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




    }
}
