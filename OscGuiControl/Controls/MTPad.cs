using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace OscGuiControl.Controls
{
	public class MTPad : yGuiWPF.Controls.MTPad, OscTree.IOscObject, IJsonInterface, IPropertyInterface, IContextMenu
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

		static MTPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Targets", "Targets", "Events");
			properties.Add("Visible", "Visible", "Appearance");
		}

		public MTPad()
		{
			oscObject = new OscTree.Object(new OscTree.Address("MTPad" + id++), typeof(float));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Visible", (args) =>
			{
				Visible = OscParser.ToBool(args);
			}, typeof(bool)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ForegroundColor", (args) =>
			{
				Color = OscParser.ToColor(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("BackgroundColor", (args) =>
			{
				Background = OscParser.ToColor(args);
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

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("MTPad", ID, Name);
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
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
			ForeGround = new SolidColorBrush(json.Color);
			BackGround = new SolidColorBrush(json.Background);
			OscObject.Targets = json.Targets;
			Visible = json.Visible;
			changed = false;
			return true;
		}
	}
}
