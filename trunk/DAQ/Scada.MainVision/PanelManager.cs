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


        public ListViewPanel CreateDataViewPanel(DataListener dataListener, bool showList = true)
		{
            ListViewPanel panel = new ListViewPanel();
            panel.AddDataListener(dataListener);
			if (showList)
			{
				panel.ViewContent = this.ShowListView(panel, dataListener);
			}
			else
			{
				panel.ViewContent = this.ShowGraphView(panel, dataListener);
			}

			if (this.currentPanel != null)
			{
			}
			this.currentPanel = panel;
			

			this.panelList.Add(panel);
			return panel;
		}

        public ListView ShowListView(ListViewPanel panel, DataListener dataListener)
        {
            // ListView
            ListView listView = new ListView();
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

            return listView;
        }


        public GraphView ShowGraphView(ListViewPanel panel, DataListener dataListener)
        {
            GraphView graphView = new GraphView();
            graphView.AddDataListener(dataListener);

            var columnInfoList = dataListener.GetColumnsInfo(); 

            foreach (var columnInfo in columnInfoList)
            {
                graphView.AddLineName(columnInfo.BindingName, columnInfo.Header);
            }

            return graphView;
        }

		public void SetListViewPanelPos(ListViewPanel listViewPanel, int row, int column)
		{
			listViewPanel.SetValue(Grid.ColumnProperty, column);
			listViewPanel.SetValue(Grid.RowProperty, row);
		}

        public void SetGraphViewPanelPos(GraphView listViewPanel, int row, int column)
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
