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
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        struct GraduationLine
        {
            public Line Line
            {
                get;
                set;
            }

            public double Pos
            {
                get;
                set;
            }
        }

        private double scale = 1.0;

        private bool init = false;

        private Dictionary<int, GraduationLine> Graduations
        {
            get;
            set;
        }

        public ChartView()
        {
            InitializeComponent();
            this.Graduations = new Dictionary<int, GraduationLine>();
        }

        public static readonly DependencyProperty TimeScaleProperty =
            DependencyProperty.Register("TimeScale", typeof(long), typeof(ChartView));

        private void TimeAxisLoaded(object sender, RoutedEventArgs e)
        {
            if (init)
            {
                return;
            }
            this.init = true;
            for (int i = 0; i < 100; i++)
            {
                double x = i * 10;
                Line scaleLine = new Line();

                this.Graduations.Add(i, new GraduationLine() { Line = scaleLine, Pos = x });

                scaleLine.X1 = scaleLine.X2 = x;
                scaleLine.Y1 = 0;
                scaleLine.Y2 = (i % 5 != 0) ? Charts.ScaleLength : Charts.MainScaleLength;
                scaleLine.Stroke = new SolidColorBrush(Colors.Gray);
                this.TimeAxis.Children.Add(scaleLine);

                TextBlock t = new TextBlock();
                t.Foreground = new SolidColorBrush(Colors.Gray);
                t.FontWeight = FontWeights.Light;
                t.FontSize = 9;
                if (i % 5 == 0)
                {
                    t.Text = string.Format("16:{0:d2}", i);
                    t.SetValue(Canvas.LeftProperty, (double)i * 10 - 10);
                    t.SetValue(Canvas.TopProperty, (double)10);
                    this.TimeAxis.Children.Add(t);
                }
                
            }
        }

        public CurveView AddCurveView(string curveViewName, double height = 200.0)
        {
            CurveView curveView = new CurveView();
            curveView.CurveViewName = curveViewName;
            curveView.TimeScale = this.TimeScale;
            curveView.Height = height;
            this.ChartContainer.Children.Add(curveView);
            return curveView;
        }

        private void MainViewMouseMove(object sender, MouseEventArgs e)
        {
            
            foreach (var view in this.ChartContainer.Children)
            {
                CurveView curveView = (CurveView)view;

                Point point = e.GetPosition((UIElement)curveView.View);
                
                curveView.TrackTimeLine(point);
            }

            

        }




        public long TimeScale
        {
            get
            {
                return (long)this.GetValue(TimeScaleProperty);
            }

            set
            {
                this.SetValue(TimeScaleProperty, (long)value);
            }
        }

        private void ZoomHandler(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            int a = e.Delta;
            if (a > 0)
            {
                this.scale += 0.1;
            }
            else if (a < 0)
            {
                this.scale -= 0.1;
            }

            this.ZoomChartView(this.scale);
        }

        private void ZoomChartView(double scale)
        {
            this.UpdateTimeAxisScale(scale);
            foreach (var view in this.ChartContainer.Children)
            {
                CurveView curveView = (CurveView)view;
                curveView.UpdateCurveScale(scale);
            }
        }

        private void UpdateTimeAxisScale(double scale)
        {
            if (scale < 1.0 || scale > 3.0)
            {
                return;
            }

            /*
            if (Math.Abs(this.currentScale - scale) < double.Epsilon)
            {
                return;
            }
            this.currentScale = scale;

            if (curve != null)
            {
                curve.RenderTransform = new ScaleTransform(scale, scale, 0.0, centerY);
            }
            */
            //int i = 0;
            foreach (var g in this.Graduations)
            {
                Line l = g.Value.Line;
                l.X1 = l.X2 = (g.Value.Pos - 0) * scale + 0;
            }
        }


    }
}
