using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	public class XYPad : yGuiWPF.Controls.XYPad, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		static XYPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Centered", "Centered");
			properties.Add("ShowValue");
			properties.Add("Targets", "Targets", "Events");
		}

		public XYPad()
		{
			oscObject = new OscTree.Object(new OscTree.Address("XYPad" + id++));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Coordinate", (args) =>
			{
				Value = OscParser.ToPoint(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Centered", (args) =>
			{
				Centered = OscParser.ToBool(args);
			}));

			OnValueChanged += (s, e) =>
			{
				OscObject.Send(new object[] { Value.X, Value.Y });
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

		public Color Color
		{
			get => (ForeGround as SolidColorBrush).Color;
			set
			{
				ForeGround = new SolidColorBrush(value);
				Border = ForeGround;
			}
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("XYPad", ID, Name);
			result.Color = Color;
			result.Centered = Centered;
			result.ShowValue = ShowValue;
			result.Targets = OscObject.Targets;
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
			return true;
		}
	}
}
