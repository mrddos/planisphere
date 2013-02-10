using Scada.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

		private PanelManager panelManager;

        public MainWindow()
        {
            InitializeComponent();
			this.panelManager = new PanelManager(this);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			ListViewPanel panel = this.panelManager.CreateListViewPanel();
			panel.CloseClick += this.ClosePanelButtonClick;
			

			// Manage
			this.Grid.Children.Add(panel);

			this.panelManager.SetListViewPanelPos(panel, 1, 1);

		}

		void ClosePanelButtonClick(object sender, RoutedEventArgs e)
		{
			
		}




    }
}
