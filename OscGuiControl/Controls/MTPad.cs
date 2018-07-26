using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OscGuiControl.Controls
{
	public class MTPad : yGuiWPF.Controls.MTPad, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

		static private int id = 1;

		static MTPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Receivers", "Receivers", "Events");
		}

		public MTPad()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Slider" + id++;
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

		public OscAddressList Receivers { get; set; } = new OscAddressList();

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("MTPad", UID, Name);
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.Receivers = Receivers;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			ForeGround = new SolidColorBrush(json.Color);
			BackGround = new SolidColorBrush(json.Background);
			Receivers = json.Receivers;
			return true;
		}
	}
}
