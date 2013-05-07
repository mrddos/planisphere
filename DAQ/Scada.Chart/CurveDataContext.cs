using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Scada.Chart
{
    public delegate void UpdateView();

    public delegate void UpdateCurve(Point point);

    public delegate void ClearCurve();

    public class CurveDataContext
    {
        private List<Point> points = new List<Point>();

        public event UpdateView UpdateView;

        public event UpdateCurve UpdateCurve;

        public event ClearCurve ClearCurve;

        public CurveDataContext(string curveName)
        {
            this.CurveName = curveName;
        }

        public string CurveName
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public void UpdateCurves()
        {
            this.UpdateView();
        }

        public void AddPoint(double x, double y)
        {
            var p = new Point(x, y);
            points.Add(p);
            this.UpdateCurve(p);
        }

        public void AddValuePair(DateTime dateTime, double value)
        {

        }

        public void Clear()
        {
            points.Clear();
            this.ClearCurve();
        }
    }
}
