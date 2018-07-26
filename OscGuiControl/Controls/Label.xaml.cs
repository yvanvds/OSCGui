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
	/// Interaction logic for Label.xaml
	/// </summary>
	public partial class Label : System.Windows.Controls.Label
		, IOscObject
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscObjectRoutes routes = new OscObjectRoutes();
		public OscObjectRoutes Routes => routes;

		private OscAddress address = new OscAddress();
		public OscAddress Address => address;

		static private int id = 1;

		static Label()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("Content", "Text");
			properties.Add("FontSize");
			properties.Add("Color", "Color", "Appearance");
			properties.Add("HorizontalContentAlignment", "Alignment");
			properties.Add("FontWeight");
			properties.Add("FontStyle");
		}

		public Label()
		{
			InitializeComponent();
			this.Style = FindResource("LabelStyle") as Style;

			address.UID = OscTree.GenerateUID();
			address.Name = "Label" + id++;

			routes.Add("Text", (args) =>
			{
				Content = OscParser.ToString(args);
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


		public Color Color
		{
			get => (Foreground as SolidColorBrush).Color;
			set => Foreground = new SolidColorBrush(value);
		}

		public new bool IsEnabled
		{
			get => base.IsEnabled;
			set
			{
				var brush = Foreground;
				base.IsEnabled = value;
				Foreground = brush;
			}
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("Label", UID, Name);
			result.Content = Content;
			result.Color = Color;
			result.FontSize = FontSize;
			result.FontWeight = FontWeight;
			result.FontStyle = FontStyle;
			result.HAlign = HorizontalContentAlignment;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			address.UID = json.UID;
			ObjName = json.Name;
			Content = json.Content;
			Color = json.Color;
			FontSize = json.FontSize;
			FontWeight = json.FontWeight;
			FontStyle = json.FontStyle;
			HorizontalContentAlignment = json.HAlign;
			
			return true;
		}
	}
}
