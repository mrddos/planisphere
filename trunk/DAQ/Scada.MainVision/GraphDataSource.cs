using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.MainVision
{
    class GraphDataSource
    {
        List<double> yl = new List<double>();

        List<DateTime> xl = new List<DateTime>();

        EnumerableDataSource<double> yAxis;

        EnumerableDataSource<DateTime> xAxis;

        public GraphDataSource()
        {

        }

        public void AddPoint(DateTime x, double y)
        {
            xl.Add(x);
            yl.Add(y);

            // yAxis.RaiseDataChanged();
            xAxis.RaiseDataChanged();
        }

        public CompositeDataSource GetCompositeDataSource()
        {

            yAxis = new EnumerableDataSource<double>(yl);
            yAxis.SetYMapping(_y => _y);

            xAxis = new EnumerableDataSource<DateTime>(xl);
            xAxis.SetXMapping((DateTime dt) => { return dt.Ticks / 10000 / 1000; });

            CompositeDataSource ds = new CompositeDataSource(xAxis, yAxis);

            return ds;
        }
    }
}
