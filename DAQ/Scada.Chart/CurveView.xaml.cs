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
using System.Windows.Media.Effects;
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

        private double i = 0;

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
            this.CanvasView.Height = this.Height - 20;
            this.Graduation.Height = this.Height - 20;
            // Grid Line |||
            for (int i = 0; i < 15; i++)
            {
                Line l = new Line();
                l.X1 = l.X2 = i * 30;
                l.Y1 = 0;
                l.Y2 = GridViewHeight;
                
                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            // Grid Line ---
            for (int i = 0; i < 20; i++)
            {
                Line l = new Line();
                l.Y1 = l.Y2 = i * 30;
                l.X1 = 0;
                l.X2 = GridViewWidth;

                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            // Scale line
            double height = this.CanvasView.Height;

            double scaleWidth = 30;
            this.Graduation.ClipToBounds = true;
            for (int i = 0; i < 100; i++)
            {
                double y = height - i * 10;
                Line l = new Line();
                l.Y1 = l.Y2 = y;
                l.X1 = (i % 5 != 0) ? scaleWidth - Charts.ScaleLength : scaleWidth - Charts.MainScaleLength;
                l.X2 = scaleWidth;

                l.Stroke = new SolidColorBrush(Colors.Gray);
                this.Graduation.Children.Add(l);

                TextBlock t = new TextBlock();
                t.Foreground = new SolidColorBrush(Colors.Gray);
                t.FontSize = 9;

                if (i % 5 == 0)
                {
                    t.Text = string.Format("{0}", this.Min + i);
                    t.SetValue(Canvas.RightProperty, (double)10.0);
                    t.SetValue(Canvas.TopProperty, (double)y - 10);
                    this.Graduation.Children.Add(t);
                }
            }

            timeLine.Y1 = 0;
            timeLine.Y2 = GridViewHeight / 2;
            timeLine.Stroke = new SolidColorBrush(Colors.Gray);
            this.CanvasView.Children.Add(timeLine);
            this.CanvasView.ClipToBounds = true;

            curve.Stroke = new SolidColorBrush(Colors.Green);
            this.CanvasView.Children.Add(curve);

            this.SetDisplayName(this.DisplayName);

        }

        public CurveDataContext CreateDataContext(string curveName, string displayName)
        {
            this.dataContext = new CurveDataContext(curveName);
            this.dataContext.UpdateCurve += UpdateCurveHandler;

            this.DisplayName = displayName;
            return this.dataContext;
        }

        public string CurveViewName
        {
            get;
            set;
        }

        public string DisplayName
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
        /// 
        /// </summary>
        /// <param name="point"></param>
        void UpdateCurveHandler(Point point)
        {
            Point p;
            this.Convert(point, out p);
            curve.Points.Add(p);

            if (i > 20)
            {
                TranslateTransform tt = new TranslateTransform(-i/5, 0);
                curve.RenderTransform = tt;
            }
            i += 2.0;
        }

        public void TrackTimeLine(Point point)
        {
            timeLine.X1 = timeLine.X2 = point.X;
        }

        private double Convert(double v)
        {
            double range = this.Max - this.Min;
            double pa = 0.0;
            double pb = 0.0;
            if (v <= this.Max && v >= this.Min)
            {
                pa = this.Max - v;
                pb = v - this.Min;
            }

            double pos = this.Height / (pa / pb + 1);
            double y = this.Height - pos;
            return y;
        }

        private void Convert(Point p, out Point po)
        {
            po = new Point(p.X, this.Convert(p.Y));
        }

        private void CanvasViewMouseMove(object sender, MouseEventArgs e)
        {
            // Point p = e.GetPosition((UIElement)sender);
            // this.UpdateTimeLine(p);
        }

        public UIElement View
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

        private void SetDisplayName(string displayName)
        {
            Border labelBorder = new Border();
            labelBorder.CornerRadius = new CornerRadius(3.0);
            labelBorder.Background = Brushes.White;
            labelBorder.BorderBrush = Brushes.Gray;
            labelBorder.Padding = new Thickness(3.0, 2.0, 3.0, 2.0);
            labelBorder.BorderThickness = new Thickness(1);
            labelBorder.Effect = new DropShadowEffect() { Direction = 300.0, Opacity= 0.5};

            labelBorder.SetValue(Canvas.RightProperty, 12.0);
            labelBorder.SetValue(Canvas.TopProperty, 12.0);

            TextBlock displayLabel = new TextBlock();
            displayLabel.Text = displayName;
            displayLabel.Background = Brushes.White;
            displayLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);

            this.CanvasView.Children.Add(labelBorder);
            labelBorder.Child = displayLabel;
        }


    }
}
