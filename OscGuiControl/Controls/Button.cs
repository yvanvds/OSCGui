using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using yGuiWPF;

namespace OscGuiControl.Controls
{
	public class Button : yGuiWPF.Controls.Button, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		static Button()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Text");
			properties.Add("TextScale");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Targets", "Targets", "Events");
			properties.Add("IsToggle", "Is Toggle", "Events");
		}

		public Button()
		{
			oscObject = new OscTree.Object(new OscTree.Address("Button" + id++));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("TextScale", (args) =>
			{
				TextScale = (TextScales)OscParser.ToInt(args);
			}));

			ForeGround = new SolidColorBrush(System.Windows.Media.Colors.Green);
			Text = "BUTTON";

			Click += OnCick;
		}

		private void OnCick(object sender, System.Windows.RoutedEventArgs e)
		{

			if(IsToggle)
			{
				OscObject.Send(Toggled);
			} else
			{
				OscObject.Send(null);
			}
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
			OscJsonObject result = new OscJsonObject("Button", ID, Name);
			result.Content = Text;
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.TextScale = TextScale;
			result.Targets = OscObject.Targets;
			result.IsToggle = IsToggle;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Text = json.Content as string;
			ForeGround = new SolidColorBrush(json.Color);
			BackGround = new SolidColorBrush(json.Background);
			TextScale = json.TextScale;
			OscObject.Targets = json.Targets;
			IsToggle = json.IsToggle;
			return true;
		}
	}
}
