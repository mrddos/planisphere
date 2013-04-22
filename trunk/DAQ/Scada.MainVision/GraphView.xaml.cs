
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
using Scada.Chart;

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



        private Dictionary<string, CurveDataContext> dataSources = new Dictionary<string, CurveDataContext>();

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

        public void AddLineName(string deviceKey, string lineName, string displayName)
        {
            if (lineName.IndexOf("Doserate") >= 0)
            {
                displayName = displayName.Replace("μSv/h", "nSv/h");
            }

            Config cfg = Config.Instance();
            ConfigEntry entry = cfg[deviceKey];

            ConfigItem item = entry.GetConfigItem(lineName);

            CurveView curveView = this.ChartView.AddCurveView(lineName);
            curveView.Max = item.Max;
            curveView.Min = item.Min;
            curveView.Height = item.Height;
            CurveDataContext dataContext = curveView.CreateDataContext(lineName, displayName);

            this.dataSources.Add(lineName, dataContext);
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
                CurveDataContext dataContext = dataSources[key];
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
                dataContext.AddPoint(i * 5  , r);
            }
            i++;
        }

        private void OnDataArrivalEnd()
        {

            //this.plotter.ItemsSource = this.dataSource;
        }
    }

}
