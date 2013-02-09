using Scada.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Scada.MainVision
{
	public class PanelManager
	{
		private Window window;

		private List<ListViewPanel> panelList = new List<ListViewPanel>();

		public PanelManager(Window window)
		{
			this.window = window;
		}

		~PanelManager()
		{

		}

		public ListViewPanel CreateListViewPanel()
		{
			ListViewPanel panel = new ListViewPanel();

			// ListView
			ListView listView = new ListView();
			panel.ListViewContent = listView;
			GridView gridView = new GridView();

			GridViewColumn col = new GridViewColumn();
			col.Header = "ASS";
			gridView.Columns.Add(col);

			listView.View = gridView;
			

			this.panelList.Add(panel);
			return panel;
		}

		public void HideListViewPanel(ListViewPanel listViewPanel)
		{
			listViewPanel.Visibility = Visibility.Hidden;
		}


		public void CloseListViewPanel(ListViewPanel listViewPanel)
		{
			listViewPanel.Visibility = Visibility.Hidden;
			this.panelList.Remove(listViewPanel);
		}
	}
}
