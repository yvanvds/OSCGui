using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OscGuiControl.Controls
{
	public class Slider : yGuiWPF.Controls.Slider, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		static Slider()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Minimum");
			properties.Add("Maximum");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Accent", "Accent", "Appearance");
			properties.Add("Targets", "Targets", "Events");
		}

		public Slider()
		{
			oscObject = new OscTree.Object(new OscTree.Address("Slider" + id++));

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

			Changed += (s, e) =>
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

		public Color Color
		{
			get => (ForeGround as SolidColorBrush).Color;
			set => ForeGround = new SolidColorBrush(value);
		}

		public Color Background
		{
			get => (BackGround as SolidColorBrush).Color;
			set => BackGround = new SolidColorBrush(value);
		}

		public Color Accent
		{
			get => (Handle as SolidColorBrush).Color;
			set => Handle = new SolidColorBrush(value);
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Slider", ID, Name);
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.Handle = Handle.Color;
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
			ForeGround = new SolidColorBrush(json.Color);
			Handle = new SolidColorBrush(json.Handle);
			BackGround = new SolidColorBrush(json.Background);
			OscObject.Targets = json.Targets;
			return true;
		}
	}
}
