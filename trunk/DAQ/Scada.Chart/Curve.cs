using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Scada.Chart
{
    public class Curve
    {
        private List<Point> points = new List<Point>();

        public Curve(string curveName)
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

        public void AddPoint(double x, double y)
        {
            points.Add(new Point(x, y));
        }
    }
}
