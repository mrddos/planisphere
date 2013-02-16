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
	/// <summary>
	/// Interaction logic for HerePane.xaml
	/// </summary>
	public partial class HerePane : UserControl
	{
		public const double APointV = 20.0;

		public const double BPointV = 76.0;

		public const double AZoneSideWidth = 100;

		public const double AtoBWidth = 40.0;


		public HerePane()
		{
			InitializeComponent();
		}

		private void HerePaneLoaded(object sender, RoutedEventArgs e)
		{
			Path path = (Path)this.Control.Template.FindName("Path", this.Control);
			if (path != null)
			{

				path.Data = this.DrawPath();
				
			}
		}

		/// <summary>
		/// 
		/// 1					         6
		/// 2    b3                b4    5
		///            b1    b2	
		/// 
		/// </summary>
		/// <returns></returns>
		private Geometry DrawPath(int width = 800)
		{
			const double Radius = 5.0;
			const double Width1 = 100;
			const double Height1 = 100;
			const double Height2 = 70;
			double x = 10;
			double y = 10;
			GeometryGroup gg = new GeometryGroup();

			RectangleGeometry rect1 = new RectangleGeometry(new Rect(x, y, Width1, Height1), Radius, Radius);
			

			RectangleGeometry rect2 = new RectangleGeometry(new Rect(x, y, width, Height2), Radius, Radius);

			
			gg.FillRule = FillRule.Nonzero;
			gg.Children.Add(rect1);
			gg.Children.Add(rect2);

			return gg.GetOutlinedPathGeometry();
		}



		
	}
}
