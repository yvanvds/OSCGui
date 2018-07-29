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
		OscTree.Route lastRoute;
		OscTree.Tree Root;

		public MainWindow()
		{
			InitializeComponent();
			Root = new OscTree.Tree(new OscTree.Address("Root", "Root"));

			Root.Add(Gui1.OscTree);
			Root.Add(Gui2.OscTree);

			GuiInspector.OscRoot = Root;

			Gui1.SetInspector(GuiInspector);
			Gui2.SetInspector(GuiInspector);
			Gui1.SetGridSize(4, 4);
			Gui2.SetGridSize(8, 8);
		}


		public override void EndInit()
		{
			base.EndInit();
			
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
			dialog.SetRoute(lastRoute);
			dialog.ShowDialog();
			lastRoute = dialog.CurrentAddress;
		}
	}
}
