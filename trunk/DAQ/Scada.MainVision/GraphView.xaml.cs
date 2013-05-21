
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

        private bool realTime = true;

        static Color[] colors = { Colors.Green, Colors.Red, Colors.Blue, Colors.OrangeRed, Colors.Purple };


        private List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

        private Dictionary<string, CurveDataContext> dataSources = new Dictionary<string, CurveDataContext>();
        
        // private DataArrivalConfig config;

        public GraphView(bool realTime)
        {
            InitializeComponent();
            this.realTime = realTime;
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

        public int Interval
        {
            get
            {
                return this.ChartView.Interval;
            }

            set
            {
                this.ChartView.Interval = value;
            }
        }

        public void AddLineName(string deviceKey, string lineName, string displayName)
        {
            // TODO:
            if (lineName.IndexOf("Doserate") >= 0)
            {
                displayName = displayName.Replace("μSv/h", "nSv/h");
            }

            Config cfg = Config.Instance();
            ConfigEntry entry = cfg[deviceKey];

            ConfigItem item = entry.GetConfigItem(lineName);

            CurveView curveView = this.ChartView.AddCurveView(lineName, displayName);
            
            curveView.Max = item.Max;
            curveView.Min = item.Min;
            curveView.Height = item.Height;
            CurveDataContext dataContext = curveView.CreateDataContext(lineName, displayName);

            this.dataSources.Add(lineName.ToLower(), dataContext);
        }


        private void OnDataArrivalBegin(DataArrivalConfig config)
        {
            if (config == DataArrivalConfig.TimeRange)
            {
                if (!this.realTime)
                {
                    // Clear
                    foreach (string key in dataSources.Keys)
                    {
                        CurveDataContext dataContext = dataSources[key];
                        i = 0;
                        dataContext.Clear();
                    }
                }
            }
            else if (config == DataArrivalConfig.TimeRecent)
            {
                if (this.realTime)
                {
                    // Do nothing with dataContext
                }
            }
        }



        private void OnDataArrival(DataArrivalConfig config, Dictionary<string, object> entry)
        {
            if (config == DataArrivalConfig.TimeRecent)
            {
                this.AddTimePoint(i, entry);
                i++;
            }
            else if (config == DataArrivalConfig.TimeRange)
            {
                this.AddTimePoint(i, entry);
                i++;
            }

        }

        private void AddTimePoint(int index, Dictionary<string, object> entry)
        {
            foreach (string key in dataSources.Keys)
            {
                // 存在这条曲线
                if (entry.ContainsKey(key))
                {
                    string v = (string)entry[key];
                    double r = 0.0;
                    if (v.Length > 0)
                    {
                        if (!double.TryParse(v, out r))
                        {
                            return;
                        }
                    }

                    CurveDataContext dataContext = dataSources[key];
                    dataContext.AddTimeValuePair(index * 5, r);
                }
            }
        }

        private void OnDataArrivalEnd(DataArrivalConfig config)
        {
        }

        private void ChartView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            foreach (string key in dataSources.Keys)
            {
                CurveDataContext dataContext = dataSources[key];
                dataContext.UpdateCurves();
            }
        }

    }

}
