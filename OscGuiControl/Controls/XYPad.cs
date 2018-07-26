using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	public class XYPad : yGuiWPF.Controls.XYPad, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

		public OscAddressList Receivers { get; set; } = new OscAddressList();

		static private int id = 1;

		static XYPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Centered", "Centered");
			properties.Add("ShowValue");
		}

		public XYPad()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Slider" + id++;

			routes.Add("Coordinate", (args) =>
			{
				Value = OscParser.ToPoint(args);
			});

			routes.Add("Centered", (args) =>
			{
				Centered = OscParser.ToBool(args);
			});

			OnValueChanged += (s, e) =>
			{
				if (Receivers != null)
				{
					foreach (var receiver in Receivers.List)
					{
						OscSender.Send(receiver, new object[] { Value.X, Value.Y });
					}
				}
			};
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
			OscJsonObject result = new OscJsonObject("XYPad", UID, Name);
			result.Color = Color;
			result.Centered = Centered;
			result.ShowValue = ShowValue;
			result.Receivers = Receivers;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			Color = json.Color;
			Centered = json.Centered;
			ShowValue = json.ShowValue;
			Receivers = json.Receivers;
			return true;
		}
	}
}
