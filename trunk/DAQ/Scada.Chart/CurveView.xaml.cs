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
using System.Windows.Media.Animation;
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
        // Graduation Line
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

        // Graduation Label Text
        struct GraduationText
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

        public const double GridViewHeight = 1000.0;

        public const double GridViewWidth = 1000.0;

        private bool init = false;

        private Line timeLine = new Line();

        private Polyline curve = null;

        private double i = 0;

        private double currentScale = 1.0;

        private CurveDataContext dataContext;

        private double centerX = 0.0;

        private double centerY = 0.0;

        private TextBlock valueLabel;

        private Dictionary<int, GraduationLine> Graduations
        {
            get;
            set;
        }

        private Dictionary<int, GraduationText> GraduationTexts
        {
            get;
            set;
        }

        public CurveView()
        {
            InitializeComponent();
            this.Graduations = new Dictionary<int, GraduationLine>();
            this.GraduationTexts = new Dictionary<int, GraduationText>();
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
            this.CanvasView.Height = this.Height - ChartView.ViewGap;
            this.Graduation.Height = this.Height - ChartView.ViewGap;
            // Grid Line |||
            double canvasHeight = this.CanvasView.Height;
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
                l.Y1 = l.Y2 = canvasHeight - i * 30;
                l.X1 = 0;
                l.X2 = GridViewWidth;

                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            // Scale line
            double height = this.CanvasView.Height;

            double scaleWidth = 30;
            this.Graduation.ClipToBounds = true;
            int textCount = 0;

            double d = height / (this.Max - this.Min);
            // How many graduation?
            int dc = (int)height / 10;
            // What's the value aach graduation 
            double ev = (this.Max - this.Min) / dc;

            for (int i = 0; i < 50; i++)
            {
                double y = height - i * 10;

                if (y < 0)
                {
                    break;
                }
                
                Line l = new Line();
                this.Graduations.Add(i, new GraduationLine() { Line = l, Pos = y});
                l.Y1 = l.Y2 = y;
                l.X1 = (i % 5 != 0) ? scaleWidth - Charts.ScaleLength : scaleWidth - Charts.MainScaleLength;
                l.X2 = scaleWidth;

                l.Stroke = new SolidColorBrush(Colors.Gray);
                this.Graduation.Children.Add(l);

                double value = this.Min + i * ev;

                if (i % 5 == 0)
                {
                    TextBlock t = new TextBlock();
                    t.Foreground = new SolidColorBrush(Colors.Gray);
                    t.FontSize = 9;
                    double pos = (double)y - 10;
                    this.GraduationTexts.Add(textCount, new GraduationText()
                    {
                        Text = t, Pos = pos
                    });

                    if (value > 0)
                    {
                        t.Text = string.Format("{0}", (int)value);
                    }
                    else
                    {
                        t.Text = string.Format(".{0:00}", (double)value);
                    }
                    t.SetValue(Canvas.RightProperty, (double)10.0);
                    t.SetValue(Canvas.TopProperty, (double)pos);
                    this.Graduation.Children.Add(t);

                    textCount++;
                }
            }

            timeLine.Y1 = 0;
            timeLine.Y2 = GridViewHeight / 2;
            timeLine.Stroke = new SolidColorBrush(Colors.Gray);
            this.CanvasView.Children.Add(timeLine);
            this.CanvasView.ClipToBounds = true;

            this.AddCurveLine();

            this.SetDisplayName(this.DisplayName);

        }

        private void AddCurveLine()
        {
            this.curve = new Polyline();
            this.curve.Stroke = new SolidColorBrush(Colors.Green);
            
            this.CanvasView.Children.Add(this.curve);
        }

        public CurveDataContext CreateDataContext(string curveName, string displayName)
        {
            this.dataContext = new CurveDataContext(curveName);
            this.dataContext.UpdateView += this.UpdateViewHandler;
            this.dataContext.UpdateCurve += this.UpdateCurveHandler;
            this.dataContext.ClearCurve += this.ClearCurveHandler;

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

        private void UpdateViewHandler()
        {
            TranslateTransform tt = new TranslateTransform(0, 0);
            // tt.BeginAnimation(TranslateTransform.XProperty, AnimationTimeline.
            curve.RenderTransform = tt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void UpdateCurveHandler(Point point)
        {
            Point p;
            this.Convert(point, out p);
            curve.Points.Add(p);

            double aw = this.ActualWidth;
            if (p.X > aw * 3 / 4) 
            {
                TranslateTransform tt = new TranslateTransform(aw * 3 / 4 - p.X, 0);
                // tt.BeginAnimation(TranslateTransform.XProperty, AnimationTimeline.
                curve.RenderTransform = tt;
                // curve.RenderTransform = new ScaleTransform(this.currentScale, this.currentScale, centerX, centerY);
            }
            
        }

        private void ClearCurveHandler()
        {
            if (this.curve != null)
            {
                this.curve.Points.Clear();
                /// this.CanvasView.Children.Remove(this.curve);
            }
        }

        /// <summary>
        /// Scale [1.0, 3.0]
        /// </summary>
        /// <param name="scale"></param>
        public void UpdateCurveScale(double scale)
        {
            if (scale < 1.0 || scale > 3.0)
            {
                return;
            }

            if (Math.Abs(this.currentScale - scale) < double.Epsilon)
            {
                return;
            }
            this.currentScale = scale;

            if (curve != null)
            {
                this.centerX = timeLine.X1;
                this.centerY = this.GetY(this.centerX);
                curve.RenderTransform = new ScaleTransform(scale, scale, this.centerX, this.centerY);
            }

            //int i = 0;
            foreach (var g in this.Graduations)
            {
                Line l = g.Value.Line;
                l.Y1 = l.Y2 = (g.Value.Pos - centerY) * this.currentScale + centerY;
            }

            foreach (var g in this.GraduationTexts)
            {
                TextBlock l = g.Value.Text;
                
                double pos = (g.Value.Pos - centerY) * this.currentScale + centerY;
                l.SetValue(Canvas.TopProperty, (double)pos);
            }
        }

        public void TrackTimeLine(Point point)
        {
            timeLine.X1 = timeLine.X2 = point.X;
            // this.centerX = point.X;
            this.UpdateValue(point);
        }

        private void UpdateValue(Point point)
        {
            double x = point.X;

            double xo = (x - this.centerX) / this.currentScale + this.centerX;

            double y = this.GetY(xo);
            double v = this.GetValue(y);
            // TODO: Optimize.

            this.valueLabel.Text = string.Format("{0}", v);
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
            else
            {
                return 0.0;
            }

            double pos = this.Height / (pa / pb + 1);
            double y = this.Height - pos;
            return y;
        }

        private double GetY(double x)
        {
            Point a = default(Point);
            Point b = default(Point);
            foreach (var p in curve.Points)
            {
                if (p.X > x)
                {
                    b = p;
                    break;
                }
                a = p;
            }

            double v = 0.0;
            if (x - a.X < b.X - x)
            {
                return a.Y;
            }
            else
            {
                return b.Y;
            }
        }

        private double GetValue(double y)
        {
            double v = this.Max - (this.Max - this.Min) * y / this.Height;
            return v;
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


            // Value text Label.
            this.valueLabel = new TextBlock();
            this.valueLabel.SetValue(Canvas.RightProperty, 120.0);
            this.valueLabel.SetValue(Canvas.TopProperty, 12.0);
            this.CanvasView.Children.Add(this.valueLabel);
        }


    }
}
