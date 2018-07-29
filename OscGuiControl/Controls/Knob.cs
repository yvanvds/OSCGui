using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	class Knob : yGuiWPF.Controls.Knob, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties => properties;

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		static Knob()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Minimum");
			properties.Add("Maximum");
			properties.Add("BrushColor", "Color", "Appearance");
			properties.Add("DisplayName", "Text", "Appearance");
			properties.Add("ShowValue", "Show Value", "Appearance");
			properties.Add("Targets", "Targets", "Events");
		}

		public Knob()
		{
			oscObject = new OscTree.Object(new OscTree.Address("Knob" + id++));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Value", (args) =>
			{
				Value = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Minimum", (args) =>
			{
				Minimum = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Maximum", (args) =>
			{
				Maximum = OscParser.ToFloat(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("ShowValue", (args) =>
			{
				ShowValue = OscParser.ToBool(args);
			}));

			OnValueChange += (s, e) =>
			{
				OscObject.Send(Value);				
			};
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
			OscJsonObject result = new OscJsonObject("Knob", ID, Name);
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			result.Color = BrushColor;
			result.Content = DisplayName;
			result.ShowValue = ShowValue;
			result.Targets = OscObject.Targets;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Minimum = json.Minimum;
			Maximum = json.Maximum;
			Color = new SolidColorBrush(json.Color);
			DisplayName = json.Content as string;
			ShowValue = json.ShowValue;
			OscObject.Targets = json.Targets;
			return true;
		}
	}
}
