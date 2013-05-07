
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

        static Color[] colors = { Colors.Green, Colors.Red, Colors.Blue, Colors.OrangeRed, Colors.Purple };


        private List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

        private Dictionary<string, CurveDataContext> dataSources = new Dictionary<string, CurveDataContext>();
        
        private DataArrivalConfig config;

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

            CurveView curveView = this.ChartView.AddCurveView(lineName, displayName);
            
            curveView.Max = item.Max;
            curveView.Min = item.Min;
            curveView.Height = item.Height;
            CurveDataContext dataContext = curveView.CreateDataContext(lineName, displayName);

            this.dataSources.Add(lineName.ToLower(), dataContext);
        }


        private void OnDataArrivalBegin(DataArrivalConfig config)
        {
            this.config = config;
            if (config == DataArrivalConfig.TimeRange)
            {
                // For show new data source, so clear the old data source.
                foreach (string key in dataSources.Keys)
                {
                    CurveDataContext dataContext = dataSources[key];
                    dataContext.Clear();
                }
            }
            else if (config == DataArrivalConfig.TimeRecent)
            {

            }
        }



        private void OnDataArrival(Dictionary<string, object> entry)
        {
            if (this.config == DataArrivalConfig.TimeRecent)
            {
                // Add new data into the datasource.
                foreach (string key in dataSources.Keys)
                {
                    CurveDataContext dataContext = dataSources[key];
                    if (entry.ContainsKey(key))
                    {
                        string v = (string)entry[key];
                        double r = 0.0;
                        if (v.Length > 0)
                        {
                            r = double.Parse(v);
                        }


                        dataContext.AddPoint(i * 5, r);
                    }
                }
                i++;
            }

        }

        private void OnDataArrivalEnd()
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
