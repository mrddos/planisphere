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

namespace Scada.Controls
{
	/// <summary>
	/// Interaction logic for ListViewPanel.xaml
	/// </summary>
	public partial class ListViewPanel : UserControl
	{
		public ListViewPanel()
		{
			InitializeComponent();
		}

		public Button ListViewContent
		{
			get
			{
				return null;
			}
			set
			{

				this.ListViewContainer.Content = value;
			}
		}
	}
}
