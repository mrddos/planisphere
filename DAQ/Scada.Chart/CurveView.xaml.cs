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

namespace Scada.Chart
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class CurveView : UserControl
    {
        public const double GridViewHeight = 1000.0;

        public const double GridViewWidth = 1000.0;

        private bool init = false;

        private Line timeLine = new Line();

        private Polyline curve = new Polyline();

        private CurveDataContext dataContext;

        public CurveView()
        {
            InitializeComponent();
            
        }

        private void CurveViewLoaded(object sender, RoutedEventArgs e)
        {
            if (!init)
            {
                this.Initialize();
                init = true;
            }
        }

        private void Initialize()
        {
            this.CanvasView.Height = this.Height;
            this.Graduation.Height = this.Height;
            // Grid Line ---
            for (int i = 0; i < 10; i++)
            {
                Line l = new Line();
                l.X1 = l.X2 = i * 30;
                l.Y1 = 0;
                l.Y2 = GridViewHeight;
                
                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            // Grid Line |||
            for (int i = 0; i < 10; i++)
            {
                Line l = new Line();
                l.Y1 = l.Y2 = i * 30;
                l.X1 = 0;
                l.X2 = GridViewWidth;

                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            // Scale line
            double height = this.Height;

            double scaleWidth = 30;
            this.Graduation.ClipToBounds = true;
            for (int i = 0; i < 100; i++)
            {
                Line l = new Line();
                l.Y1 = l.Y2 = height - i * 10;
                l.X1 = (i % 5 != 0) ? scaleWidth - Charts.ScaleLength : scaleWidth - Charts.MainScaleLength;
                l.X2 = scaleWidth;

                l.Stroke = new SolidColorBrush(Colors.Gray);
                this.Graduation.Children.Add(l);
            }

            timeLine.Y1 = 0;
            timeLine.Y2 = GridViewHeight / 2;
            timeLine.Stroke = new SolidColorBrush(Colors.Gray);
            this.CanvasView.Children.Add(timeLine);
            this.CanvasView.ClipToBounds = true;

            curve.Stroke = new SolidColorBrush(Colors.Green);
            this.CanvasView.Children.Add(curve);

        }

        public CurveDataContext CreateDataContext(string curveName, string displayName)
        {
            this.dataContext = new CurveDataContext(curveName);
            this.dataContext.UpdateCurve += UpdateCurveHandler;

            return this.dataContext;
        }

        public string CurveViewName
        {
            get;
            set;
        }

        public long TimeScale
        {
            get;
            internal set;
        }

        /// <summary>
        /// H               Max
        /// 
        /// 
        /// 0               Min     
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="point"></param>
        void UpdateCurveHandler(Point point)
        {
            Point p;
            this.Convert(point, out p);
            curve.Points.Add(p);
        }

        public void TrackTimeLine(Point point)
        {
            timeLine.X1 = timeLine.X2 = point.X;
        }

        private void Convert(Point p, out Point po)
        {
            double range = this.Max - this.Min;
            double pa = 0.0;
            double pb = 0.0;
            if (p.Y <= this.Max && p.Y >= this.Min)
            {
                pa = this.Max - p.Y;
                pb = p.Y - this.Min;
            }

            double pos = this.Height / (pa / pb + 1);
            double y = this.Height - pos;

            po = new Point(p.X, y);
        }

        private void CanvasViewMouseMove(object sender, MouseEventArgs e)
        {
            // Point p = e.GetPosition((UIElement)sender);
            // this.UpdateTimeLine(p);
        }

        public UIElement Canvas
        {
            get
            {
                return (UIElement)this.CanvasView;
            }
        }

        public double Min
        {
            set;
            get;
        }

        public double Max
        {
            set;
            get;
        }


    }
}
