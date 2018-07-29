using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OscGuiControl.Windows
{
	/// <summary>
	/// Interaction logic for TreeWindow.xaml
	/// </summary>
	public partial class TreeWindow : Window
	{
		public OscTree.Route CurrentRoute = null;

		public TreeWindow()
		{
			InitializeComponent();
		}

		public void SetRoot(OscTree.Tree root)
		{
			TreeGui.SetRoot(root);
			TreeGui.OnRouteChanged += () =>
			{
				OkButton.IsEnabled = (TreeGui.SelectedRoute != null);
				if (TreeGui.SelectedRoute != null)
				{
					TreeGui.SelectedRoute.CurrentStep = 0;
					TreeGui.SelectedRoute.ScreenName = root.GetNameOfRoute(TreeGui.SelectedRoute);
					CurrentRouteName.Content = TreeGui.SelectedRoute.ScreenName;
				}
				else
				{
					CurrentRouteName.Content = "";
				}
			};
		}

		public void SetRoute(OscTree.Route route)
		{
			CurrentRoute = route;
			TreeGui.SetRoute(route);
		}

		private void CancelClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OkClicked(object sender, RoutedEventArgs e)
		{
			CurrentRoute = TreeGui.SelectedRoute;
			Close();
		}
	}
}
