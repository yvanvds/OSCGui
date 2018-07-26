﻿using System;
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
		public OscAddress CurrentAddress = null;

		public TreeWindow()
		{
			InitializeComponent();
			OscTree.OnRouteChanged += () =>
			{
				OkButton.IsEnabled = (OscTree.SelectedRoute != null);
				if (OscTree.SelectedRoute != null)
				{
					CurrentRoute.Content = OscTree.SelectedRoute.Address;
				}
				else
				{
					CurrentRoute.Content = "";
				}
			};
		}

		public void SetRoute(OscAddress address)
		{
			CurrentAddress = address;
			OscTree.SetRoute(address);
		}

		private void CancelClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OkClicked(object sender, RoutedEventArgs e)
		{
			CurrentAddress = OscTree.SelectedRoute;
			Close();
		}
	}
}
