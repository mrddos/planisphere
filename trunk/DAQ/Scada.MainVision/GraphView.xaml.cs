
using Scada.Controls.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace Scada.MainVision
{
    /// <summary>
    /// Interaction logic for GraphViewPanel.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        int i = 0;

        private DataListener dataListener;

        static Color[] colors = { Colors.Green, Colors.Red, Colors.Blue, Colors.OrangeRed, Colors.Purple };
        // private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
        // private List<Dictionary<string, object>> dataSource;

        private Dictionary<string, ObservableDataSource<Point>> dataSources = new Dictionary<string, ObservableDataSource<Point>>();

        public GraphView()
        {
            InitializeComponent();
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

        public void AddLineName(string lineName, string displayName)
        {
            ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
            
            plotter.AddLineGraph(dataSource, colors[dataSources.Count], 2, displayName);
            dataSources.Add(lineName, dataSource);
            plotter.Viewport.PredictFocus(FocusNavigationDirection.Right);
            
            plotter.Viewport.AutoFitToView = true;
            
            //plotter.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
        }

        private void OnDataArrivalBegin()
        {
            //if (this.dataSource != null)
            //{
                // this.dataSource.Clear();
            //}
            // this.dataSource = new List<Dictionary<string, object>>();
        }


        private void OnDataArrival(Dictionary<string, object> entry)
        {

            foreach (string key in entry.Keys)
            {
                ObservableDataSource<Point> dataSource = (ObservableDataSource<Point>)dataSources[key];

                string v = (string)entry[key];
                dataSource.AppendAsync(this.Dispatcher, new Point(i, double.Parse(v)));
            }
            i++;
        }

        private void OnDataArrivalEnd()
        {
            // this.ListViewContent.ItemsSource = this.dataSource;
        }
    }
}
