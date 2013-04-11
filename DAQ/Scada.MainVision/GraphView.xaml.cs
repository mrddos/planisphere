
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

        private Dictionary<string, DataSource> dataSources = new Dictionary<string, DataSource>();

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
            // CompositeDataSource cds = new CompositeDataSource();
            
            // CompositeDataSource
            DataSource dataSource = new DataSource();
            // dataSource.SetXMapping(CustomHorizontalDateTimeAxis.ConvertToDoubleFunction);
            // dataSource.SetXMapping(x => x.X / 100);
            plotter.AddLineGraph(dataSource.GetCompositeDataSource(), colors[dataSources.Count], 2, displayName);
            dataSources.Add(lineName, dataSource);
            plotter.Viewport.PredictFocus(FocusNavigationDirection.Right);
            
            plotter.Viewport.AutoFitToView = true;

            //plotter.Viewport.Zoom(2);
            this.timeAxis.ShowMayorLabels = false;
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

            foreach (string key in dataSources.Keys)
            {
                DataSource dataSource = dataSources[key];
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

    class DataSource
    {
        List<double> yl = new List<double>();

        List<DateTime> xl = new List<DateTime>();

        EnumerableDataSource<double> yAxis;

        EnumerableDataSource<DateTime> xAxis;

        public DataSource()
        {

        }

        public void AddPoint(DateTime x, double y)
        {
            xl.Add(x);
            yl.Add(y);

            yAxis.RaiseDataChanged();
        }

        public CompositeDataSource GetCompositeDataSource()
        {
            
            yAxis = new EnumerableDataSource<double>(yl);
            yAxis.SetYMapping(_y => _y);

            xAxis = new EnumerableDataSource<DateTime>(xl);
            xAxis.SetXMapping(CustomHorizontalDateTimeAxis.ConvertToDoubleFunction);

            CompositeDataSource ds = new CompositeDataSource(xAxis, yAxis);
            return ds;
        }
    }

    public class CustomHorizontalDateTimeAxis : HorizontalDateTimeAxis
    {
        public const double K =  0.001;

        public CustomHorizontalDateTimeAxis()
            : base()
        {
            this.AxisControl.MayorLabelProvider.SetCustomFormatter(i =>
            {
                return i.Tick.ToString("HH:mm:ss.fff");
            });

            
            this.ConvertFromDouble = ConvertFromDoubleFunction;
            this.ConvertToDouble = ConvertToDoubleFunction;
        }

        public static Func<DateTime, double> ConvertToDoubleFunction
        {
            get
            {
                return (dt) =>
                {
                    double v = dt.Ticks / 10000 / 1000 ;
                    return v;
                };
            }
        }

        public static Func<double, DateTime> ConvertFromDoubleFunction
        {
            get
            {
                return (d) =>
                {
                    return DateTime.FromOADate(d / K);
                };
            }
        }
    }
}
