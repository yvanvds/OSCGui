using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OscGuiControl.Controls
{
	public class MTPad : yGuiWPF.Controls.MTPad, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		static MTPad()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Targets", "Targets", "Events");
		}

		public MTPad()
		{
			oscObject = new OscTree.Object(new OscTree.Address("MTPad" + id++));
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

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("MTPad", ID, Name);
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.Targets = OscObject.Targets;
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
			return true;
		}
	}
}
