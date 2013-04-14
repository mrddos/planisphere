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


        //private long timeScale;

        public ChartView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TimeScaleProperty =
            DependencyProperty.Register("TimeScale", typeof(long), typeof(ChartView));

        private void TimeAxisLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                Line scaleLine = new Line();
                scaleLine.X1 = scaleLine.X2 = i * 10;
                scaleLine.Y1 = 0;
                scaleLine.Y2 = (i % 5 != 0) ? Charts.ScaleLength : Charts.MainScaleLength;
                scaleLine.Stroke = new SolidColorBrush(Colors.Gray);
                this.TimeAxis.Children.Add(scaleLine);
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

                Point point = e.GetPosition((UIElement)curveView.Canvas);
                
                curveView.UpdateTimeLine(point);
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


    }
}
