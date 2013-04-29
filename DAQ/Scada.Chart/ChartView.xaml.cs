﻿using System;
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

        struct GraduationTime
        {
            public TextBlock Text
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

        private Dictionary<int, GraduationTime> GraduationTimes
        {
            get;
            set;
        }

        public ChartView()
        {
            InitializeComponent();
            this.Graduations = new Dictionary<int, GraduationLine>();
            this.GraduationTimes = new Dictionary<int, GraduationTime>();
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

            int timeTextCount = 0;
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

                if (i % 5 == 0)
                {
                    TextBlock t = new TextBlock();
                    t.Foreground = new SolidColorBrush(Colors.Gray);
                    t.FontWeight = FontWeights.Light;
                    t.FontSize = 9;
                    int pos = i * 10 + 10;
                    GraduationTimes.Add(timeTextCount, new GraduationTime() 
                    {
                        Text = t, Pos = pos
                    });
                    timeTextCount++;
                    t.Text = string.Format("16:{0:d2}", i);
                    t.SetValue(Canvas.LeftProperty, (double)pos);
                    t.SetValue(Canvas.TopProperty, (double)10);
                    this.TimeAxis.Children.Add(t);
                }
                
            }
        }

        public CurveView AddCurveView(string curveViewName, string displayName, double height = 200.0)
        {
            CurveView curveView = new CurveView();
            curveView.CurveViewName = curveViewName;
            curveView.TimeScale = this.TimeScale;
            curveView.Height = height;
            this.ChartContainer.Children.Add(curveView);
            this.AddCurveViewCheckItem(curveViewName, displayName);
            return curveView;
        }

        private void AddCurveViewCheckItem(string curveViewName, string displayName)
        {
            CheckBox cb = new CheckBox();
            cb.IsChecked = true;
            cb.Content = displayName;
            cb.Margin = new Thickness(5, 0, 5, 0);
            
            cb.Checked += (object sender, RoutedEventArgs e) => 
            {
                this.OnItemChecked(curveViewName, true);
            };
            cb.Unchecked += (object sender, RoutedEventArgs e) =>
            {
                this.OnItemChecked(curveViewName, false);
            };
            this.SelectedItems.Children.Add(cb);
        }

        private void OnItemChecked(string curveViewName, bool itemChecked)
        {
            foreach (var cv in this.ChartContainer.Children)
            {
                CurveView curveView = (CurveView)cv;
                if (curveView.CurveViewName == curveViewName)
                {
                    curveView.Visibility = itemChecked ? Visibility.Visible : Visibility.Collapsed;
                    break;
                }
            }
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

            foreach (var g in this.Graduations)
            {
                Line l = g.Value.Line;
                l.X1 = l.X2 = (g.Value.Pos - 0) * scale + 0;
            }


            foreach (var t in this.GraduationTimes)
            {
                TextBlock b = t.Value.Text;
                double pos = (t.Value.Pos - 0) * scale + 0;
                b.SetValue(Canvas.LeftProperty, (double)pos);
            }
        }


    }
}
