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

        private Line timeLine = new Line();

        private List<Curve> curves = new List<Curve>();

        public CurveView()
        {
            InitializeComponent();
        }

        private void CurveViewLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Line l = new Line();
                l.X1 = l.X2 = i * 30;
                l.Y1 = 0;
                l.Y2 = GridViewHeight;
                
                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            for (int i = 0; i < 10; i++)
            {
                Line l = new Line();
                l.Y1 = l.Y2 = i * 30;
                l.X1 = 0;
                l.X2 = GridViewWidth;

                l.Stroke = new SolidColorBrush(Colors.LightGray);
                this.CanvasView.Children.Add(l);
            }

            for (int i = 0; i < 20; i++)
            {
                Line l = new Line();
                l.Y1 = l.Y2 = i * 10;
                l.X1 = 25;
                l.X2 = 30;

                l.Stroke = new SolidColorBrush(Colors.Gray);
                this.Graduation.Children.Add(l);
            }

            timeLine.Y1 = 0;
            timeLine.Y2 = GridViewHeight / 2;
            timeLine.Stroke = new SolidColorBrush(Colors.Gray);
            this.CanvasView.Children.Add(timeLine);
        }

        public Curve AddCurve(string curveName, string displayName)
        {
            Curve curve = new Curve(curveName);
            curves.Add(curve);
            return curve;
        }

        public void UpdateTimeLine(Point point)
        {
            timeLine.X1 = timeLine.X2 = point.X;
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
    }
}
