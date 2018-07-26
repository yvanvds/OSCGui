using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OscGuiControl.Controls
{
	/// <summary>
	/// Interaction logic for TextBox.xaml
	/// </summary>
	public partial class TextBox : System.Windows.Controls.TextBox
		, IOscObject, ISender
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }


		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;
		static private int id = 1;

		static TextBox()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("FontSize");
			properties.Add("Receivers", "Receivers", "Events");
		}

		public TextBox()
		{
			InitializeComponent();
			Style = FindResource("TextBoxStyle") as Style;

			address.UID = OscTree.GenerateUID();
			address.Name = "TextBox" + id++;

			routes.Add("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			});

			routes.Add("FontSize", (args) =>
			{
				FontSize = OscParser.ToInt(args);
			});
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

		public OscAddressList Receivers { get; set; } = new OscAddressList();

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (Receivers != null)
			{
				foreach (var receiver in Receivers.List)
				{
					OscSender.Send(receiver, Text);
				}
			}
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("TextBox", UID, Name);
			result.Content = Text;
			result.FontSize = FontSize;
			result.Receivers = Receivers;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			Address.UID = json.UID;
			ObjName = json.Name;
			Text = json.Content as string;
			FontSize = json.FontSize;
			Receivers = json.Receivers;
			return true;
		}
	}
}
