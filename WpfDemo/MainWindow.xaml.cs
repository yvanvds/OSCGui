using OscGuiControl;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		OscAddress lastAddress;

		public MainWindow()
		{
			InitializeComponent();

			
		}


		public override void EndInit()
		{
			base.EndInit();
			Gui1.SetInspector(GuiInspector);
			Gui2.SetInspector(GuiInspector);
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			File.WriteAllText("gui1.json", Gui1.ToJSON());
			File.WriteAllText("gui2.json", Gui2.ToJSON());
		}

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			Gui1.LoadJSON(File.ReadAllText("gui1.json"));
			Gui2.LoadJSON(File.ReadAllText("gui2.json"));
		}

		private void ShowTreeButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Tree();
			dialog.SetRoute(lastAddress);
			dialog.ShowDialog();
			lastAddress = dialog.CurrentAddress;
		}
	}
}
