using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	class Led : yGuiWPF.Controls.Led, OscTree.IOscObject, IJsonInterface, IPropertyInterface, IContextMenu
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

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

		static Led()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("BrushColor", "Color", "Appearance");
			properties.Add("Scale", "Size", "Appearance");
			properties.Add("Visible", "Visible", "Appearance");
		}

		public Led()
		{
			oscObject = new OscTree.Object(new OscTree.Address("Led" + id++), typeof(int));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Blink", (args) =>
			{
				Blink(100);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}, typeof(bool)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Color", (args) =>
			{
				BrushColor = OscParser.ToColor(args);
			}));

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

		public Color BrushColor
		{
			get => (Color as SolidColorBrush).Color;
			set
			{
				Color = new SolidColorBrush(value);
			}
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Led", ID, Name);
			result.Color = BrushColor;
			result.Scale = Scale;
			result.Visible = Visible;
			changed = false;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Color = new SolidColorBrush(json.Color);
			Scale = json.Scale;
			Visible = json.Visible;
			changed = false;
			return true;
		}
	}
}
