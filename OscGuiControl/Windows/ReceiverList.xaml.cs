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
	/// Interaction logic for ReceiverList.xaml
	/// </summary>
	public partial class ReceiverList : Window
	{
		private OscAddressList list;
		public OscAddressList List
		{
			get => list;
			set
			{
				list = value;
				ContentList.ItemsSource = list.List;
			}
		}

		public ReceiverList()
		{
			InitializeComponent();
			RemoveButton.IsEnabled = false;
			EditButton.IsEnabled = false;
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var window = new TreeWindow();
			window.ShowDialog();
			if(window.CurrentAddress != null)
			{
				list.List.Add(window.CurrentAddress);
				ContentList.Items.Refresh();
			}
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			OscAddress selected = ContentList.SelectedItem as OscAddress;
			list.List.Remove(selected);
			ContentList.Items.Refresh();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			OscAddress selected = ContentList.SelectedItem as OscAddress;
			var window = new TreeWindow();
			window.SetRoute(selected);
			window.ShowDialog();
			if(window.CurrentAddress != null)
			{
				int index = list.List.IndexOf(selected);
				if(index != -1)
				{
					list.List.RemoveAt(index);
					list.List.Insert(index, window.CurrentAddress);
					ContentList.Items.Refresh();
				}
				
			}
		}

		private void ContentList_Selected(object sender, RoutedEventArgs e)
		{
			if(ContentList.SelectedItem == null)
			{
				RemoveButton.IsEnabled = false;
				EditButton.IsEnabled = false;
			} else
			{
				RemoveButton.IsEnabled = true;
				EditButton.IsEnabled = true;
			}
		}
	}
}
