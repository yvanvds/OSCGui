using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OscGuiControl.Controls
{
	public class Slider : yGuiWPF.Controls.Slider, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

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
			properties.Add("Receivers", "Receivers", "Events");
		}

		public Slider()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Slider" + id++;

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

			Changed += (s, e) =>
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


		public OscAddressList Receivers { get; set; } = new OscAddressList();

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Slider", UID, Name);
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.Handle = Handle.Color;
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
			ForeGround = new SolidColorBrush(json.Color);
			Handle = new SolidColorBrush(json.Handle);
			BackGround = new SolidColorBrush(json.Background);
			Receivers = json.Receivers;
			return true;
		}
	}
}
