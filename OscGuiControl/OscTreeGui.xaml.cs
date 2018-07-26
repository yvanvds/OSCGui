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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OscGuiControl
{

	public partial class OscTreeGui : UserControl
	{
		enum ButtonType
		{
			Tree,
			Object,
			Route,
		}

		private List<StackPanel> panels = new List<StackPanel>();

		public OscTreeGui()
		{
			InitializeComponent();
			addPanel(OscTree.Root);
		}

		public event Action OnRouteChanged = delegate { };

		private OscAddress selectedRoute;
		public OscAddress SelectedRoute
		{
			get => selectedRoute;
			set
			{
				selectedRoute = value;
			}
		}

		public void SetRoute(OscAddress address)
		{
			LevelGrid.Children.Clear();
			LevelGrid.ColumnDefinitions.Clear();
			panels.Clear();
			addPanel(OscTree.Root);

			if (address == null) return;
			if (address.Route.Count == 0) return;
			int level = 0;
			foreach(var id in address.Route)
			{
				foreach(var elm in panels[level].Children)
				{
					var button = elm as Button;
					ButtonType type = (ButtonType)button.Tag;
					if(type == ButtonType.Tree) 
					{
						var tree = button.DataContext as OscTree;
						if (tree.UID.Equals(address.Route[level + 1]))
						{
							level++;
							select(button);
							addPanel(tree);
							deselect(level);
							break;
						}
					} else if (type == ButtonType.Object)
					{
						var obj = button.DataContext as IOscObject;
						if(obj.UID.Equals(address.Route[level + 1]))
						{
							level++;
							select(button);
							addPanel(obj);
							deselect(level);
							break;
						}
					} else if (type == ButtonType.Route)
					{
						if((button.Content as string).Equals(address.Route[level + 1]))
						{
							select(button);
							break;
						} 
					}
				}
			}
		}

		private void addPanel(OscTree tree)
		{
			var column = new ColumnDefinition();
			column.Width = new GridLength(200);
			LevelGrid.ColumnDefinitions.Add(column);

			var panel = new StackPanel();
			
			panel.Orientation = Orientation.Vertical;
			Grid.SetColumn(panel, panels.Count);
			Grid.SetRow(panel, 0);
			LevelGrid.Children.Add(panel);
			panels.Add(panel);

			foreach (var elm in tree.Trees.Values)
			{
				var button = new Button();
				button.Content = elm.Name;
				button.DataContext = elm;
				button.Tag = ButtonType.Tree;
				button.BorderBrush = new SolidColorBrush(Colors.Green);
				panel.Children.Add(button);
				button.Click += OnTreeClick;
			}

			foreach (var obj in tree.Objects.Values)
			{
				var button = new Button();
				button.Content = obj.Name;
				button.DataContext = obj;
				button.Tag = ButtonType.Object;
				button.BorderBrush = new SolidColorBrush(Colors.YellowGreen);
				panel.Children.Add(button);
				button.Click += OnObjectClick;
			}
		}

		private void addPanel(IOscObject obj)
		{
			var column = new ColumnDefinition();
			column.Width = new GridLength(200);
			LevelGrid.ColumnDefinitions.Add(column);

			var panel = new StackPanel();

			panel.Orientation = Orientation.Vertical;
			Grid.SetColumn(panel, panels.Count);
			Grid.SetRow(panel, 0);
			LevelGrid.Children.Add(panel);
			panels.Add(panel);

			foreach(var route in obj.Routes.List)
			{
				var button = new Button();
				button.Content = route;
				button.DataContext = obj;
				button.Tag = ButtonType.Route;
				button.BorderBrush = new SolidColorBrush(Colors.Yellow);
				panel.Children.Add(button);
				button.Click += RouteClicked;
			}

		}

		private void RouteClicked(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var obj = button.DataContext as IOscObject;
			int level;
			if(OscTree.GetLevel(obj, out level))
			{
				level++; // it's an object route
				removeExtraPanels(level);
				deselect(level - 1);
				select(button);
				selectedRoute = new OscAddress();
				selectedRoute.UID = selectedRoute.Name = button.Content as string;
				selectedRoute.CreateRoute(obj.Address.Route);
				OnRouteChanged();
			}
		}

		private void OnObjectClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var obj = button.DataContext as IOscObject;

			int level;
			if(OscTree.GetLevel(obj, out level))
			{
				removeExtraPanels(level);
				deselect(level - 1);
				select(button);
				addPanel(obj);
				deselect(level);
				selectedRoute = null;
				OnRouteChanged();
			}
		}

		private void OnTreeClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var tree = button.DataContext as OscTree;

			int level;
			if(OscTree.GetLevel(tree, out level))
			{
				removeExtraPanels(level);
				deselect(level - 1);
				select(button);
				addPanel(tree);
				deselect(level);
				selectedRoute = null;
				OnRouteChanged();
			}
		}

		private void removeExtraPanels(int level)
		{
			while (panels.Count > level)
			{
				var panel = panels.Last();
				LevelGrid.Children.Remove(panel);
				panels.Remove(panel);
				LevelGrid.ColumnDefinitions.RemoveAt(LevelGrid.ColumnDefinitions.Count - 1);
			}
		}

		private void deselect(int level)
		{
			if (level >= panels.Count) return;
			foreach(Button button in panels[level].Children)
			{
				button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
				button.Background.Opacity = 1.0;
			}
		}

		private void select(Button button)
		{
			switch((ButtonType)button.Tag)
			{
				case ButtonType.Tree:
					Color g = Colors.Green;
					g.A = 128;
					button.Background = new SolidColorBrush(g);
					break;
				case ButtonType.Object:
					Color gy = Colors.GreenYellow;
					gy.A = 128;
					button.Background = new SolidColorBrush(gy);
					break;
				case ButtonType.Route:
					Color y = Colors.Yellow;
					y.A = 128;
					button.Background = new SolidColorBrush(y);
					break;
			}		
		}

	}
}
