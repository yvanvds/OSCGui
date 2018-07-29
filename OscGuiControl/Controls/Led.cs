using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	class Led : yGuiWPF.Controls.Led, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

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
			oscObject = new OscTree.Object(new OscTree.Address("Led" + id++));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Blink", (args) =>
			{
				Blink(100);
			}));
		}

		public string ObjName
		{
			get => OscObject.Address.Name;
			set => OscObject.Address.Name = value;
		}

		public new string Name => ObjName;

		public string ID
		{
			get => OscObject.Address.ID;
		}

		public Color BrushColor
		{
			get => (Color as SolidColorBrush).Color;
			set => Color = new SolidColorBrush(value);
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Led", ID, Name);
			result.Color = BrushColor;
			result.Scale = Scale;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Color = new SolidColorBrush(json.Color);
			Scale = json.Scale;
			return true;
		}
	}
}
