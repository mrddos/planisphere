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

namespace Scada.MainVision
{

    public class HerePaneItemData
    {
        public string Title
        {
            get;
            set;
        }

        public string Data1
        {
            get;
            set;
        }

        public string Data2
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for HerePaneItem.xaml
    /// </summary>
    public partial class HerePaneItem : UserControl
    {
        private HerePaneItemData data;

		private static SolidColorBrush HoverColorBrush = new SolidColorBrush(Colors.White);

		private static SolidColorBrush CommomColorBrush = new SolidColorBrush(Colors.AliceBlue);

        public HerePaneItem()
        {
            InitializeComponent();
        }

        public string Title
        {
            get;
            set;
        }

        private void ItemLoaded(object sender, RoutedEventArgs e)
        {
            this.data = new HerePaneItemData() { Title = this.Title };
            this.itemGrid.DataContext = this.data;

            data.Data1 = "31.4  ℃";
            data.Data2 = "12 m/s";
        }

        public TextBlock this[int i]
        {
            get
            {
                if (i == 0)
                {
                    return this.data1;
                }
                else if (i == 1)
                {
                    return this.data2;
                }
                else if (i == 2)
                {
                    return null;
                }
                return null;
            }
        }

        private void OnMouseEnterRect(object sender, MouseEventArgs e)
        {
			this.Rect.Fill = HoverColorBrush;
        }

        private void OnMouseLeaveRect(object sender, MouseEventArgs e)
        {
			this.Rect.Fill = CommomColorBrush;
        }


    }
}
