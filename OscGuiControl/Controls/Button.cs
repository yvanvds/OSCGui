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
	public class Button : yGuiWPF.Controls.Button, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

		static private int id = 1;

		static Button()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Text");
			properties.Add("TextScale");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("Background", "Background", "Appearance");
			properties.Add("Receivers", "Receivers", "Events");
			properties.Add("IsToggle", "Is Toggle", "Events");
		}

		public Button()
		{
			address.UID = OscTree.GenerateUID();
			address.Name = "Button" + id++;

			routes.Add("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			});

			routes.Add("TextScale", (args) =>
			{
				TextScale = (TextScales)OscParser.ToInt(args);
			});

			ForeGround = new SolidColorBrush(System.Windows.Media.Colors.Green);
			Text = "BUTTON";

			Click += OnCick;
		}

		private void OnCick(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Receivers != null)
			{
				foreach (var receiver in Receivers.List)
				{
					if(IsToggle)
					{
						OscSender.Send(receiver, Toggled);
					} else
					{
						OscSender.Send(receiver);
					}
				}
			}
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

		private OscAddressList receivers = new OscAddressList();
		public OscAddressList Receivers
		{
			get => receivers;
			set => receivers = value;
		}


		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Button", UID, Name);
			result.Content = Text;
			result.Color = ForeGround.Color;
			result.Background = BackGround.Color;
			result.TextScale = TextScale;
			result.Receivers = Receivers;
			result.IsToggle = IsToggle;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			Text = json.Content as string;
			ForeGround = new SolidColorBrush(json.Color);
			BackGround = new SolidColorBrush(json.Background);
			TextScale = json.TextScale;
			Receivers = json.Receivers;
			IsToggle = json.IsToggle;
			return true;
		}
	}
}
