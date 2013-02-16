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

				//path.Data = Geometry.Parse(this.DrawPath());
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
		private string DrawPath(int windowWidth = 800)
		{
			string format =
@"
M 0,0 
V #Height#
H #SideWidth#
A 
L #B1C#
H #BumpWidth#
L #B2C#
H #WindowWidth#
V 0
Z
";
			double x = 0.0;
			double y = 0.0;
			const double Offset = 12.0; 

			double bumpWidth = windowWidth - (AZoneSideWidth + AtoBWidth) * 2;
			string ret = format;
			ret = ret.Replace("#Height#", APointV.ToString());
			ret = ret.Replace("#SideWidth#", (AZoneSideWidth - Offset).ToString());
			ret = ret.Replace("#WindowWidth#", windowWidth.ToString());
			ret = ret.Replace("#BumpWidth#", bumpWidth.ToString());

			string[] ps = { "#B1C#", "#B1E#", "#B2C#", "#B2E#", };// "#B1E#", "#B2C#", "#B2E#", "#B4S#", "#B4C#", "#B4E#" };

			foreach (string po in ps)
			{
				
				// double ctrlPointOffsetX = 0.0;
				// double ctrlPointOffsetY = 0.0;
				double startPointOffsetX = 0.0;
				double startPointOffsetY = 0.0;
				double endPointOffsetX = 0.0;
				double endPointOffsetY = 0.0;
				switch (po[2])
				{


					case '1':
						x = AZoneSideWidth + AtoBWidth;
						y = BPointV;
						
						//ctrlPointOffsetY = Offset / 2;
						startPointOffsetX = -Offset;
						startPointOffsetY = -Offset;
						endPointOffsetX = Offset;
						break;
					case '2':
						x = AZoneSideWidth + AtoBWidth + bumpWidth;
						y = BPointV;
						//ctrlPointOffsetX = Offset / 2;
						//ctrlPointOffsetY = Offset / 2;
						startPointOffsetX = -Offset;
						endPointOffsetX = Offset;
						endPointOffsetY = -Offset;
						break;

				}

				switch (po[3])
				{
					case 'C':
						//x += ctrlPointOffsetX;
						//y += ctrlPointOffsetY;
						break;

					case 'E':
						x += endPointOffsetX;
						y += endPointOffsetY;
						break;
					default:
						break;
				}

				ret = ret.Replace(po, string.Format("{0},{1}", x, y));
			}
			ret = ret.Replace("\r\n", " ");
			return ret;
		}



		
	}
}
