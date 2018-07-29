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
			Endpoint,
		}

		private List<StackPanel> panels = new List<StackPanel>();

		public OscTreeGui()
		{
			InitializeComponent();
		}

		private OscTree.Tree root = null;
		public void SetRoot(OscTree.Tree root)
		{
			this.root = root;
			addPanel(root);
		}

		public event Action OnRouteChanged = delegate { };

		private OscTree.Route selectedRoute;
		public OscTree.Route SelectedRoute
		{
			get => selectedRoute;
			set
			{
				selectedRoute = value;
			}
		}

		public void SetRoute(OscTree.Route route)
		{
			LevelGrid.Children.Clear();
			LevelGrid.ColumnDefinitions.Clear();
			panels.Clear();
			addPanel(root);

			if (route == null) return;
			if (route.Steps.Count == 0) return;
			int level = 0;
			foreach(var id in route.Steps)
			{
				foreach(var elm in panels[level].Children)
				{
					var button = elm as Button;
					ButtonType type = (ButtonType)button.Tag;
					if(type == ButtonType.Tree) 
					{
						var tree = button.DataContext as OscTree.Tree;
						if (tree.Address.ID.Equals(route.Steps[level + 1]))
						{
							level++;
							select(button);
							addPanel(tree);
							deselect(level);
							break;
						}
					} else if (type == ButtonType.Object)
					{
						var obj = button.DataContext as OscTree.IOscNode;
						if(obj.Address.ID.Equals(route.Steps[level + 1]))
						{
							level++;
							select(button);
							addPanel(obj);
							deselect(level);
							break;
						}
					} else if (type == ButtonType.Endpoint)
					{
						if((button.Content as string).Equals(route.Steps[level + 1]))
						{
							select(button);
							break;
						} 
					}
				}
			}
		}

		private void addPanel(OscTree.Tree tree)
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

			foreach (var elm in tree.Children.List.Values)
			{
				var button = new Button();
				button.Content = elm.Address.Name;
				button.DataContext = elm;
				if(elm is OscTree.Tree)
				{
					button.Tag = ButtonType.Tree;
					button.BorderBrush = new SolidColorBrush(Colors.Green);
					button.Click += OnTreeClick;
				} else
				{
					button.Tag = ButtonType.Object;
					button.BorderBrush = new SolidColorBrush(Colors.YellowGreen);
					button.Click += OnObjectClick;
				}				
				panel.Children.Add(button);
			}

			foreach (var obj in tree.Endpoints.List.Values)
			{
				var button = new Button();
				button.Content = obj.Name;
				button.DataContext = obj;
				button.Tag = ButtonType.Endpoint;
				button.BorderBrush = new SolidColorBrush(Colors.Yellow);
				button.Click += RouteClicked;
				panel.Children.Add(button);
			}
		}

		private void addPanel(OscTree.IOscNode obj)
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

			foreach(var route in obj.Endpoints.List.Values)
			{
				var button = new Button();
				button.Content = route.Name;
				button.DataContext = obj;
				button.Tag = ButtonType.Endpoint;
				button.BorderBrush = new SolidColorBrush(Colors.Yellow);
				panel.Children.Add(button);
				button.Click += RouteClicked;
			}

		}

		private void RouteClicked(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var obj = button.DataContext as OscTree.IOscNode;
			int level = obj.Address.TreeLevel();

			level++; // it's an endpoint of this object
			removeExtraPanels(level);
			deselect(level - 1);
			select(button);
			string route = obj.GetRouteString(OscTree.Route.RouteType.ID);
			route += "/" + button.Content as string;
			selectedRoute = new OscTree.Route(route, OscTree.Route.RouteType.ID);
			OnRouteChanged();
		}

		private void OnObjectClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var obj = button.DataContext as OscTree.Object;

			int level = obj.Address.TreeLevel();

			removeExtraPanels(level);
			deselect(level - 1);
			select(button);
			addPanel(obj);
			deselect(level);
			selectedRoute = null;
			OnRouteChanged();
			
		}

		private void OnTreeClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var tree = button.DataContext as OscTree.Tree;

	
			int level = tree.Address.TreeLevel();
			removeExtraPanels(level);
			deselect(level - 1);
			select(button);
			addPanel(tree);
			deselect(level);
			selectedRoute = null;
			OnRouteChanged();
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
				case ButtonType.Endpoint:
					Color y = Colors.Yellow;
					y.A = 128;
					button.Background = new SolidColorBrush(y);
					break;
			}		
		}

	}
}
