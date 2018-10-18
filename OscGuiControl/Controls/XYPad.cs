using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	public class XYPad : yGuiWPF.Controls.XYPad, OscTree.IOscObject, IJsonInterface, IPropertyInterface, IContextMenu
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
		public bool HasChanged()
		{
			return changed;
		}

		private ContextMenu menu = new ContextMenu();
		public ContextMenu Menu => menu;

		static XYPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("BorderColor", "Border", "Appearance");
			properties.Add("Centered", "Centered");
			properties.Add("ShowValue");
			properties.Add("Targets", "Targets", "Events");
			properties.Add("Visible", "Visible", "Appearance");
		}

		public XYPad()
		{
			oscObject = new OscTree.Object(new OscTree.Address("XYPad" + id++), typeof(Point));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Coordinate", (args) =>
			{
				Value = OscParser.ToPoint(args);
			}, typeof(System.Windows.Point)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Centered", (args) =>
			{
				Centered = OscParser.ToBool(args);
			}, typeof(bool)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}, typeof(bool)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				Color = OscParser.ToColor(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BorderColor", (args) =>
			{
				BorderColor = OscParser.ToColor(args);
			}));

			OnValueChanged += (s, e) =>
			{
				OscObject.Send(new Point(Value.X, Value.Y));
			};

			foreach (var endpoint in oscObject.Endpoints.List)
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

		public Color BorderColor
		{
			get => (Border as SolidColorBrush).Color;
			set
			{
				Border = new SolidColorBrush(value);
			}
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("XYPad", ID, Name);
			result.Color = Color;
			result.Background = BorderColor;
			result.Centered = Centered;
			result.ShowValue = ShowValue;
			result.Targets = OscObject.Targets;
			result.Visible = Visible;
			changed = false;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Color = json.Color;
			Centered = json.Centered;
			ShowValue = json.ShowValue;
			OscObject.Targets = json.Targets;
			Visible = json.Visible;
			changed = false;
			return true;
		}
	}
}
