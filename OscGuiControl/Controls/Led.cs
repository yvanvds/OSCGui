using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	class Led : yGuiWPF.Controls.Led, IOscObject
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

		static private int id = 1;

		static Led()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("BrushColor", "Color", "Appearance");
			properties.Add("Scale", "Size", "Appearance");
		}

		public Led()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Led" + id++;

			routes.Add("Blink", (args) =>
			{
				Blink(100);
			});
		}

		public string ObjName
		{
			get => address.Name;
			set => address.Name = value;
		}

		public new string Name => ObjName;

		public string UID
		{
			get => address.UID;
		}

		public Color BrushColor
		{
			get => (Color as SolidColorBrush).Color;
			set => Color = new SolidColorBrush(value);
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Led", UID, Name);
			result.Color = BrushColor;
			result.Scale = Scale;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			Color = new SolidColorBrush(json.Color);
			Scale = json.Scale;
			return true;
		}
	}
}
