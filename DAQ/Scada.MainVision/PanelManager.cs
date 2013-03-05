using Scada.Controls;
using Scada.Controls.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Scada.MainVision
{
    /**
     * PanelManager
     * 
     */
    public class PanelManager
	{
		private Window window;

		private List<ListViewPanel> panelList = new List<ListViewPanel>();

		private ListViewPanel currentPanel;

		public PanelManager(Window window)
		{
			this.window = window;
		}

		~PanelManager()
		{

		}

        public ListViewPanel CreateListViewPanel(DataListener dataListener)
		{
			// ListView
			ListView listView = new ListView();
            

            ListViewPanel panel = new ListViewPanel();
            panel.AddDataListener(dataListener);
			panel.ListViewContent = listView;
			GridView gridView = new GridView();
            listView.View = gridView;


            var columnInfoList = dataListener.GetColumnsInfo(); // new List<ColumnInfo>();

            foreach (var columnInfo in columnInfoList)
            {
                GridViewColumn col = new GridViewColumn();
                col.Header = columnInfo.Header;
                string bindingName = string.Format("[{0}]", columnInfo.BindingName);
                col.DisplayMemberBinding = new Binding(bindingName);
                col.Width = columnInfo.Width;
                gridView.Columns.Add(col);
            }
			

			if (this.currentPanel != null)
			{
			}
			this.currentPanel = panel;
			

			this.panelList.Add(panel);
			return panel;
		}

        public GraphViewPanel CreateGraphViewPanel(DataListener dataListener)
        {
            GraphViewPanel panel = new GraphViewPanel();
            panel.AddDataListener(dataListener);

            var columnInfoList = dataListener.GetColumnsInfo(); 

            foreach (var columnInfo in columnInfoList)
            {
                panel.AddLineName(columnInfo.BindingName, columnInfo.Header);
            }

            return panel;
        }

		public void SetListViewPanelPos(ListViewPanel listViewPanel, int row, int column)
		{
			listViewPanel.SetValue(Grid.ColumnProperty, column);
			listViewPanel.SetValue(Grid.RowProperty, row);
		}

        public void SetGraphViewPanelPos(GraphViewPanel listViewPanel, int row, int column)
        {
            listViewPanel.SetValue(Grid.ColumnProperty, column);
            listViewPanel.SetValue(Grid.RowProperty, row);
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
