using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace OscGuiControl.Controls
{
	class Knob : yGuiWPF.Controls.Knob, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties => properties;

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

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
			properties.Add("Receivers", "Receivers", "Events");
		}

		public Knob()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Knob" + id++;

			routes.Add("Value", (args) =>
			{
				Value = OscParser.ToFloat(args);
			});

			routes.Add("Minimum", (args) =>
			{
				Minimum = OscParser.ToFloat(args);
			});

			routes.Add("Maximum", (args) =>
			{
				Maximum = OscParser.ToFloat(args);
			});

			routes.Add("ShowValue", (args) =>
			{
				ShowValue = OscParser.ToBool(args);
			});

			OnValueChange += (s, e) =>
			{
				if (Receivers != null)
				{
					foreach (var receiver in Receivers.List)
					{
						OscSender.Send(receiver, Value);
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

		public Color BrushColor
		{
			get => (Color as SolidColorBrush).Color;
			set => Color = new SolidColorBrush(value);
		}

		public OscAddressList Receivers { get; set; } = new OscAddressList();

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Knob", UID, Name);
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			result.Color = BrushColor;
			result.Content = DisplayName;
			result.ShowValue = ShowValue;
			result.Receivers = Receivers;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			Minimum = json.Minimum;
			Maximum = json.Maximum;
			Color = new SolidColorBrush(json.Color);
			DisplayName = json.Content as string;
			ShowValue = json.ShowValue;
			Receivers = json.Receivers;
			return true;
		}

		
	}
}
