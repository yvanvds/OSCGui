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
	public partial class Label : System.Windows.Controls.Label, OscTree.IOscObject, IJsonInterface, IPropertyInterface
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;

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

			oscObject = new OscTree.Object(new OscTree.Address("Label" + id++));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Content = OscParser.ToString(args);
			}));

			oscObject.Endpoints.Add(new OscTree.Endpoint("FontSize", (args) =>
			{
				FontSize = OscParser.ToInt(args);
			}));


		}

		public new double FontSize
		{
			get { return base.FontSize; }
			set
			{
				if (value > 0)
				{
					base.FontSize = value;
				}
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
			OscJsonObject result = new OscJsonObject("Label", ID, Name);
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
			OscObject.Address.ID = json.UID;
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
