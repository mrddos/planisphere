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
        

        public ChartView()
        {
            InitializeComponent();
        }

        public void AddCurveView()
        {
            CurveView curveView = new CurveView();
            this.ChartContainer.Children.Add(curveView);
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
    }
}
