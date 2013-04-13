
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
using Microsoft.Research.DynamicDataDisplay.Charts;


// TODO:http://dynamicdatadisplay.codeplex.com/discussions/53203
namespace Scada.MainVision
{
    /// <summary>
    /// Interaction logic for GraphViewPanel.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        public const string TimeKey = "Time";

        int i = 0;

        DateTime now = DateTime.Now;

        private DataListener dataListener;

        static Color[] colors = { Colors.Green, Colors.Red, Colors.Blue, Colors.OrangeRed, Colors.Purple };
        // private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
        
        //private List<Dictionary<string, object>> dataSource;

        private Dictionary<string, GraphDataSource> dataSources = new Dictionary<string, GraphDataSource>();

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
            if (lineName.IndexOf("Doserate") >= 0)
            {
                displayName = displayName.Replace("μSv/h", "nSv/h");
            }

            GraphDataSource dataSource = new GraphDataSource();

            plotter.AddLineGraph(dataSource.GetCompositeDataSource(), colors[dataSources.Count], 2, displayName);
            dataSources.Add(lineName, dataSource);
            plotter.Viewport.PredictFocus(FocusNavigationDirection.Right);
            
            plotter.Viewport.AutoFitToView = true;

            this.timeAxis.ShowMayorLabels = true;

            // this.timeAxis.AxisControl.TicksProvider
            // this.timeAxis.AxisControl.TicksProvider = new DateTimeTicksProvider();

            //plotter.Viewport.Zoom(2);
            //
            //plotter.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
        }

        private void OnDataArrivalBegin()
        {
            // this.timeAxis.ShowMinorTicks = false;
            

            
            //if (this.dataSource != null)
            //{
            //    this.dataSource.Clear();
            //}
            //this.dataSource = new List<Dictionary<string, object>>();
            
        }



        private void OnDataArrival(Dictionary<string, object> entry)
        {
            // timeAxis.SetValue(
            // timeAxis.SetMaxDate(new DateTime(2013, 3, 10, 12, 0, 0)); 
            // this.timeAxis.AxisControl.MayorLabelProvider.LabelStringFormat = "HH:mm:ss";

            
            foreach (string key in dataSources.Keys)
            {
                GraphDataSource dataSource = dataSources[key];
                string v = (string)entry[key];
                double r = double.Parse(v);
                if (key.ToLower() == "doserate")
                {
                    Random rd = new Random();
                    int c = rd.Next(1460, 1500);
                    int d = rd.Next(-20, 0);
                    c -= d;
                    r = c / 10;
                }
                dataSource.AddPoint(this.now.AddSeconds((double)i), r);
            }


            i++;
        }

        private void OnDataArrivalEnd()
        {

            //this.plotter.ItemsSource = this.dataSource;
        }
    }

}
