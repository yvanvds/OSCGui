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
	public partial class TextBox : System.Windows.Controls.TextBox, OscTree.IOscObject, IJsonInterface, IPropertyInterface, IContextMenu
	{
		static private PropertyCollection properties = null;
		public PropertyCollection Properties { get => properties; }

		private OscTree.Object oscObject;
		public OscTree.Object OscObject => oscObject;
		public OscTree.Routes Targets => oscObject.Targets;

		static private int id = 1;

		private bool changed = false;
		public void Taint()
		{
			changed = true;
		}
		public bool HasChanged()
		{
			return changed;
		}

		private ContextMenu menu = new ContextMenu();
		public ContextMenu Menu => menu;

		static TextBox()
		{
			properties = new PropertyCollection();
			properties.Add("ObjName", "Name");
			properties.Add("FontSize");
			properties.Add("Targets", "Targets", "Events");
		}

		public TextBox()
		{
			InitializeComponent();
			Style = FindResource("TextBoxStyle") as Style;

			oscObject = new OscTree.Object(new OscTree.Address("TextBox" + id++), typeof(string));

			oscObject.Endpoints.Add(new OscTree.Endpoint("Text", (args) =>
			{
				Text = OscParser.ToString(args);
			}, typeof(string)));

			oscObject.Endpoints.Add(new OscTree.Endpoint("FontSize", (args) =>
			{
				FontSize = OscParser.ToInt(args);
			}, typeof(int)));

			foreach (var endpoint in oscObject.Endpoints.List)
			{
				var item = new MenuItem();
				item.Header = "Route to " + endpoint.Key;
				item.Click += (sender, e) =>
				{
					var route = oscObject.GetRouteString(OscTree.Route.RouteType.ID) + "/" + endpoint.Key;
					System.Windows.Clipboard.SetText(route);
				};
				menu.Items.Add(item);
			}
		}

		public string ObjName
		{
			get => OscObject.Address.Name;
			set
			{
				OscObject.Address.Name = value;
			}
		}

		public new string Name => ObjName;

		public string ID
		{
			get => OscObject.Address.ID;
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			OscObject.Send(Text);
		}

		public JObject ToJSON()
		{
			OscJsonObject result = new OscJsonObject("TextBox", ID, Name);
			result.Content = Text;
			result.FontSize = FontSize;
			result.Targets = OscObject.Targets;
			changed = false;
			return result.Get();
		}

		public bool LoadJSON(JObject obj)
		{
			OscJsonObject json = new OscJsonObject(obj);
			OscObject.Address.ID = json.UID;
			ObjName = json.Name;
			Text = json.Content as string;
			FontSize = json.FontSize;
			OscObject.Targets = json.Targets;
			changed = false;
			return true;
		}
	}
}
