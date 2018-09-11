using OSCGui_Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OscGuiDemo
{
	public partial class MainPage : ContentPage
	{
		private OscTree.Tree Tree;
		public MainPage()
		{
			InitializeComponent();
			Tree = new OscTree.Tree(new OscTree.Address("Root", "Root"));

			Tree.Add(OscView.OscTree);

			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
			Stream stream = assembly.GetManifestResourceStream("OscGuiDemo.gui1.json");
			string text = string.Empty;
			using (var reader = new StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
			OscView.LoadJSON(text);
			OscView.LoadJSON(text);
		}
	}
}
