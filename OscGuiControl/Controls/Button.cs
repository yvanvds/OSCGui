using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using yGuiWPF;

namespace OscGuiControl.Controls
{
	public class Button : yGuiWPF.Controls.Button, OscTree.IOscObject, IJsonInterface, IPropertyInterface, IContextMenu
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets
		{
			get => oscObject.Targets;
			set
			{
				oscObject.Targets = value;
			}
		}

		static private int id = 1;

		private bool changed = false;
		public void Taint()
		{
			changed = true;
		}

		static Button()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Text");
			properties.Add("TextScale");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Targets", "Targets", "Events");
			properties.Add("IsToggle", "Is Toggle", "Events");
			properties.Add("Visible", "Visible", "Appearance");
		}

		public Button()
		{
			oscObject = new OscTree.Object(new OscTree.Address("Button" + id++), typeof(bool));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			}, typeof(string)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("TextScale", (args) =>
			{
				TextScale = (TextScales)OscParser.ToInt(args);
			}, typeof(int)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}, typeof(bool)));

			ForeGround = new SolidColorBrush(System.Windows.Media.Colors.Green);
			Text = "BUTTON";

			Click += OnCick;

			foreach(var endpoint in oscObject.Endpoints.List)
			{
				var item = new MenuItem();
				item.Header = "Route to " + endpoint.Key;
				item.Click += (sender, e) =>
				{
					var route = oscObject.GetRouteString(OscTree.Route.RouteType.ID) + "/" + endpoint.Key;
					System.Windows.Clipboard.SetText(route);
				};
				menu.Items.Add(item);
			}
		}

		private void Item_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OnCick(object sender, System.Windows.RoutedEventArgs e)
		{

			if(IsToggle)
			{
				OscObject.Send(Toggled);
			} else
			{
				OscObject.Send(true);
			}
		}

		public string ObjName
		{
			get => OscObject.Address.Name;
			set
			{
				OscObject.Address.Name = value;
			}
		}

		public new string Name => ObjName;

		public string ID
		{
			get => OscObject.Address.ID;
		}

		public Color Color
		{
			get => (ForeGround as SolidColorBrush).Color;
			set
			{
				ForeGround = new SolidColorBrush(value);
			}
		}

		public Color Background
		{
			get => (BackGround as SolidColorBrush).Color;
			set
			{
				BackGround = new SolidColorBrush(value);
			}
		}

		private ContextMenu menu = new ContextMenu();
		public ContextMenu Menu => menu;

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Button", ID, Name);
			result.Content = Text;
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.TextScale = TextScale;
			result.Targets = OscObject.Targets;
			result.IsToggle = IsToggle;
			result.Visible = Visible;
			changed = false;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Text = json.Content as string;
			ForeGround = new SolidColorBrush(json.Color);
			BackGround = new SolidColorBrush(json.Background);
			TextScale = json.TextScale;
			OscObject.Targets = json.Targets;
			IsToggle = json.IsToggle;
			Visible = json.Visible;
			changed = false;
			return true;
		}

		public bool HasChanged()
		{
			return changed;
		}
	}
}
